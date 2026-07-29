using Bam;
using Bam.Data.Objects;
using Bam.Encryption;
using Bam.Logging;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Org.BouncyCastle.Crypto;

namespace Bam.Protocol.Profile;

/// <summary>
/// Enforces the public key-set registration policy that anchors device-key account
/// confirmation (bam.protocol#8): first registration wins, rotation requires proof of
/// possession of the currently registered key and updates the existing row in place, and
/// resolution is deterministic (earliest-created row wins over any duplicates).  Composed
/// into <see cref="EncryptedProfileRepository"/> so the policy stays independently testable.
/// <para>
/// The registrar additionally: stamps the persisted creation time server-side so a
/// caller-supplied <c>Created</c> cannot decide earliest-registration-wins; enforces that
/// public key material maps to exactly one handle; validates key material is parseable before
/// persisting; and logs rejected registrations and rotations.  Handle equality is ordinal and
/// case-sensitive.
/// </para>
/// </summary>
public class PublicKeySetRegistrar : IPublicKeySetRegistrar
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PublicKeySetRegistrar"/> class.
    /// </summary>
    /// <param name="repository">The object-data repository the key sets are persisted in.</param>
    /// <param name="rotationVerifier">The verifier that decides whether a rotation signature proves possession of the current key.</param>
    public PublicKeySetRegistrar(ObjectDataRepository repository, IKeySetRotationVerifier rotationVerifier)
    {
        this.Repository = repository;
        this.RotationVerifier = rotationVerifier;
    }

    /// <summary>
    /// Gets the object-data repository the key sets are persisted in.
    /// </summary>
    protected ObjectDataRepository Repository { get; }

    /// <summary>
    /// Gets the verifier that decides whether a rotation signature proves possession of the
    /// currently registered key.
    /// </summary>
    protected IKeySetRotationVerifier RotationVerifier { get; }

    /// <inheritdoc />
    public PublicKeySetData Register(PublicKeySetData publicKeySetData)
    {
        ArgumentNullException.ThrowIfNull(publicKeySetData);
        string handle = publicKeySetData.KeySetHandle;
        if (string.IsNullOrWhiteSpace(handle))
        {
            throw new ArgumentException("A key set handle is required.", nameof(publicKeySetData));
        }
        if (string.IsNullOrEmpty(publicKeySetData.PublicRsaKey) && string.IsNullOrEmpty(publicKeySetData.PublicEccKey))
        {
            Log.Warn("Rejected key-set registration for handle '{0}': no public key material.", handle);
            throw new InvalidPublicKeySetException(handle,
                $"A key set for handle '{handle}' must contain at least one public key (RSA or ECC).", null);
        }

        ValueTuple<string, Exception>? unparseable = FindUnparseableKey(publicKeySetData);
        if (unparseable != null)
        {
            Log.Warn("Rejected key-set registration for handle '{0}': unparseable {1} public key.", handle, unparseable.Value.Item1);
            throw new InvalidPublicKeySetException(handle,
                $"The {unparseable.Value.Item1} public key presented for handle '{handle}' is not a parseable public key.",
                unparseable.Value.Item2);
        }

        lock (KeySetRegistrationLock.Sync)
        {
            // Single materialization: check both first-registration-wins (handle already taken)
            // and one-handle-to-one-key-material in one pass, rather than a Resolve scan plus a
            // separate uniqueness scan (bam.protocol#8 review S6). True indexed lookup is a
            // store-level concern tracked in bam.protocol#13. Revoked tombstones (bam.protocol#11)
            // are asymmetric here: a revoked row frees its handle for re-registration, but its
            // key material stays blocklisted from re-registration under any handle.
            bool activeHandleExists = false;
            PublicKeySetData? activeMaterialConflict = null;
            PublicKeySetData? revokedMaterialConflict = null;
            PublicKeySetData? governingTombstone = null;
            foreach (PublicKeySetData existing in Repository.RetrieveAll<PublicKeySetData>())
            {
                bool sameHandle = existing.KeySetHandle == handle;
                if (sameHandle && existing.RevokedUtc == null)
                {
                    activeHandleExists = true;
                }
                else
                {
                    // Material blocklist/uniqueness applies to every OTHER row — including a
                    // same-handle revoked tombstone, so re-registering a handle's own revoked
                    // material stays blocked (bam.protocol#18 B1).
                    if (SharesKeyMaterial(existing, publicKeySetData))
                    {
                        if (existing.RevokedUtc == null)
                        {
                            activeMaterialConflict ??= existing;
                        }
                        else
                        {
                            revokedMaterialConflict ??= existing;
                        }
                    }

                    // Successor gate (bam.protocol#21): track the governing tombstone for this handle,
                    // the same-handle revoked row with the latest RevokedUtc (Id tiebreak), mirroring
                    // Resolve's deterministic ordering. If it bound a successor, only that key may
                    // re-register the freed handle.
                    if (sameHandle && existing.RevokedUtc != null && IsMoreRecentTombstone(existing, governingTombstone))
                    {
                        governingTombstone = existing;
                    }
                }
            }

            if (activeHandleExists)
            {
                Log.Warn("Rejected key-set registration for handle '{0}': a key set is already registered.", handle);
                throw new PublicKeySetConflictException(handle);
            }
            if (activeMaterialConflict != null)
            {
                Log.Warn("Rejected key-set registration for handle '{0}': key material already registered under handle '{1}'.", handle, activeMaterialConflict.KeySetHandle);
                throw new PublicKeySetKeyMaterialConflictException(handle, activeMaterialConflict.KeySetHandle);
            }
            if (revokedMaterialConflict != null)
            {
                Log.Warn("Rejected key-set registration for handle '{0}': key material is revoked and blocklisted.", handle);
                throw new RevokedKeyMaterialException(handle);
            }

            // If the handle's governing revocation bound an authorized successor, the freed handle
            // can only be re-registered with that exact key — otherwise the revoke→re-register window
            // would let any first caller hijack the handle (bam.protocol#21). An unbound tombstone
            // (null fingerprint) leaves the handle openly re-registrable, unchanged. Compared over the
            // candidate's RSA identity key via the shared PublicKeyFingerprint so the basis matches
            // what the admin signed.
            if (governingTombstone?.AuthorizedSuccessorFingerprint != null)
            {
                string? candidateFingerprint = PublicKeyFingerprint.Of(publicKeySetData.PublicRsaKey);
                if (candidateFingerprint == null || candidateFingerprint != governingTombstone.AuthorizedSuccessorFingerprint)
                {
                    Log.Warn("Rejected key-set registration for handle '{0}': presented key is not the successor authorized by the revocation that freed it.", handle);
                    throw new UnauthorizedSuccessorException(handle);
                }
            }

            NormalizeServerControlledFields(publicKeySetData);
            return Repository.Create(publicKeySetData);
        }
    }

    /// <inheritdoc />
    public PublicKeySetData Rotate(PublicKeySetData newKeySet, byte[] rotationSignature)
    {
        ArgumentNullException.ThrowIfNull(newKeySet);
        string handle = newKeySet.KeySetHandle;
        if (string.IsNullOrWhiteSpace(handle))
        {
            throw new ArgumentException("A key set handle is required.", nameof(newKeySet));
        }

        if (string.IsNullOrEmpty(newKeySet.PublicRsaKey))
        {
            Log.Warn("Rejected key-set rotation for handle '{0}': proposed RSA public key is empty.", handle);
            throw new InvalidKeySetRotationException(handle,
                $"A rotation for handle '{handle}' must specify a non-empty RSA public key; rotating to an empty RSA key would leave the handle permanently unrotatable.");
        }

        ValueTuple<string, Exception>? unparseable = FindUnparseableKey(newKeySet);
        if (unparseable != null)
        {
            Log.Warn("Rejected key-set rotation for handle '{0}': unparseable {1} public key.", handle, unparseable.Value.Item1);
            throw new InvalidKeySetRotationException(handle,
                $"The proposed {unparseable.Value.Item1} public key for handle '{handle}' is not a parseable public key.",
                unparseable.Value.Item2);
        }

        lock (KeySetRegistrationLock.Sync)
        {
            PublicKeySetData? current = Resolve(handle);
            if (current == null)
            {
                throw new InvalidKeySetRotationException(handle,
                    $"No key set is registered for handle '{handle}'. Rotation replaces an existing key set; use registration for a first key set.");
            }

            ISignatureVerification verification;
            try
            {
                verification = RotationVerifier.Verify(current, newKeySet, rotationSignature);
            }
            catch (Exception ex)
            {
                Log.Warn("Rejected key-set rotation for handle '{0}': rotation proof could not be verified ({1}).", handle, ex.Message);
                throw new InvalidKeySetRotationException(handle,
                    $"Rotation proof could not be verified for handle '{handle}': {ex.Message}", ex);
            }

            if (!verification.Success)
            {
                Log.Warn("Rejected key-set rotation for handle '{0}': signature does not prove possession of the currently registered key.", handle);
                throw new InvalidKeySetRotationException(handle,
                    $"Rotation signature does not prove possession of the currently registered key for handle '{handle}'.");
            }

            // Enforce one-handle-to-one-key-material on the rotation path too, not just Register:
            // otherwise an attacker rotates a handle they control to a victim's public key, and
            // FindProfileByPublicKey misattributes the victim's session (bam.protocol#8 review C4 /
            // challenger B1). The helper skips the handle being rotated, so rotating to your own
            // current material remains a permitted no-op. A revoked match means the proposed
            // material is blocklisted (bam.protocol#11).
            PublicKeySetData? materialConflict = FindRowRegisteringSameKeyMaterial(newKeySet);
            if (materialConflict != null)
            {
                if (materialConflict.RevokedUtc != null)
                {
                    Log.Warn("Rejected key-set rotation for handle '{0}': proposed key material is revoked and blocklisted.", handle);
                    throw new RevokedKeyMaterialException(handle);
                }
                Log.Warn("Rejected key-set rotation for handle '{0}': key material already registered under handle '{1}'.", handle, materialConflict.KeySetHandle);
                throw new PublicKeySetKeyMaterialConflictException(handle, materialConflict.KeySetHandle);
            }

            current.PublicRsaKey = newKeySet.PublicRsaKey;
            current.PublicEccKey = newKeySet.PublicEccKey;
            return Repository.Update(current);
        }
    }

    /// <inheritdoc />
    public PublicKeySetData? Resolve(string keySetHandle)
    {
        // Skip revoked tombstones (bam.protocol#11): a revoked key set is no longer authoritative,
        // so FindPublicKeySetByHandle and device-key confirmation stop honoring it.
        return Repository.Query<PublicKeySetData>(p => p.KeySetHandle == keySetHandle && p.RevokedUtc == null)
            .OrderBy(p => p.Created)
            .ThenBy(p => p.Id)
            .FirstOrDefault();
    }

    /// <summary>
    /// True when <paramref name="candidate"/> is a later revocation than <paramref name="incumbent"/>
    /// — a strictly-greater <see cref="PublicKeySetData.RevokedUtc"/>, or an equal timestamp with a
    /// greater <see cref="Bam.Data.Repositories.RepoData.Id"/> as a deterministic tiebreak.  Selects
    /// the <i>governing</i> tombstone (most recent revocation) among multiple tombstones for a handle,
    /// so the successor binding a caller must satisfy is always the one from the latest revocation
    /// (bam.protocol#21).  Any incumbent-null candidate wins.  Both are assumed same-handle revoked
    /// rows (<c>RevokedUtc != null</c>).
    /// </summary>
    private static bool IsMoreRecentTombstone(PublicKeySetData candidate, PublicKeySetData? incumbent)
    {
        if (incumbent == null)
        {
            return true;
        }
        DateTime candidateRevoked = candidate.RevokedUtc ?? DateTime.MinValue;
        DateTime incumbentRevoked = incumbent.RevokedUtc ?? DateTime.MinValue;
        if (candidateRevoked != incumbentRevoked)
        {
            return candidateRevoked > incumbentRevoked;
        }
        return candidate.Id > incumbent.Id;
    }

    /// <summary>
    /// Stamps the fields the registrar owns rather than the caller: the creation time and the
    /// composite-key identifiers (<see cref="Bam.Data.Repositories.RepoData.Uuid"/> /
    /// <see cref="Bam.Data.Repositories.RepoData.Cuid"/>).  All three are publicly settable on
    /// <c>RepoData</c>; <c>Created</c> is the primary resolution key and <c>Uuid</c>/<c>Cuid</c>
    /// derive the <c>Id</c> tiebreak, so leaving any of them caller-controlled would let a
    /// caller influence which duplicate wins resolution (bam.protocol#8 review C3).
    /// </summary>
    private static void NormalizeServerControlledFields(PublicKeySetData publicKeySetData)
    {
        publicKeySetData.Created = DateTime.UtcNow;
        publicKeySetData.Uuid = Guid.NewGuid().ToString();
        publicKeySetData.Cuid = Bam.Cuid.Generate();
    }

    /// <summary>
    /// Returns the first row (active or revoked) under a <i>different</i> handle that shares this
    /// key set's public key material, or null when the material is not registered elsewhere.
    /// Enforces the one handle-to-one key-set invariant that <c>FindProfileByPublicKey</c> relies
    /// on; used by the rotation path (Register uses a combined single-pass scan). The caller
    /// inspects <see cref="PublicKeySetData.RevokedUtc"/> to distinguish an active conflict from
    /// blocklisted (revoked) material.
    /// </summary>
    private PublicKeySetData? FindRowRegisteringSameKeyMaterial(PublicKeySetData keySet)
    {
        foreach (PublicKeySetData existing in Repository.RetrieveAll<PublicKeySetData>())
        {
            // Skip only the handle's own ACTIVE row (rotating to your own current material is a
            // permitted no-op). A revoked tombstone of the same handle is NOT skipped, so rotating
            // a handle back to its own revoked/blocklisted material is caught (bam.protocol#18 B1).
            if (existing.KeySetHandle == keySet.KeySetHandle && existing.RevokedUtc == null)
            {
                continue;
            }
            if (SharesKeyMaterial(existing, keySet))
            {
                return existing;
            }
        }
        return null;
    }

    /// <summary>
    /// True when <paramref name="existing"/> carries the same non-empty RSA or ECC public key
    /// material as <paramref name="candidate"/>, compared over the parsed key's <b>canonical</b>
    /// DER encoding rather than the raw PEM string.  Ordinal PEM comparison is bypassable: a
    /// trivial re-encoding (an appended newline, CRLF line endings, trailing spaces) parses to the
    /// identical key while being string- and SHA-unequal, which would let a revoked/compromised key
    /// slip past the blocklist and uniqueness checks (bam.protocol#18 review, condition C1 / T1).
    /// </summary>
    private static bool SharesKeyMaterial(PublicKeySetData existing, PublicKeySetData candidate)
    {
        if (!string.IsNullOrEmpty(candidate.PublicRsaKey) && SameCanonicalKey(existing.PublicRsaKey, candidate.PublicRsaKey))
        {
            return true;
        }
        if (!string.IsNullOrEmpty(candidate.PublicEccKey) && SameCanonicalKey(existing.PublicEccKey, candidate.PublicEccKey))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// True when two PEM strings parse to the same public key (equal canonical DER
    /// <c>SubjectPublicKeyInfo</c>).  Returns false when either side is empty or unparseable.
    /// Both fingerprints come from the shared <see cref="PublicKeyFingerprint.Of"/> so the equality
    /// basis stays identical to the successor gate in <see cref="Register"/>.
    /// </summary>
    private static bool SameCanonicalKey(string? existingPem, string? candidatePem)
    {
        string? existingFingerprint = PublicKeyFingerprint.Of(existingPem);
        string? candidateFingerprint = PublicKeyFingerprint.Of(candidatePem);
        return existingFingerprint != null && existingFingerprint == candidateFingerprint;
    }

    /// <summary>
    /// Returns the first non-empty key field (RSA or ECC) that does not parse as a public key,
    /// paired with the parsing exception, or null when all present key material parses.
    /// </summary>
    private static ValueTuple<string, Exception>? FindUnparseableKey(PublicKeySetData keySet)
    {
        ValueTuple<string, Exception>? rsaFailure = ParseFailure("RSA", keySet.PublicRsaKey);
        if (rsaFailure != null)
        {
            return rsaFailure;
        }
        return ParseFailure("ECC", keySet.PublicEccKey);
    }

    /// <summary>
    /// Returns a (field, error) pair when the PEM does not parse to a key, or null when the
    /// field is empty or parses.  Note the framework's <c>PemToKey</c> returns null (rather than
    /// throwing) for input that contains no PEM object, so a null parse result is treated as a
    /// failure, not just a thrown exception.
    /// </summary>
    private static ValueTuple<string, Exception>? ParseFailure(string fieldName, string? pem)
    {
        if (string.IsNullOrEmpty(pem))
        {
            return null;
        }
        try
        {
            AsymmetricKeyParameter parsedKey = pem.PemToKey();
            if (parsedKey == null)
            {
                return new ValueTuple<string, Exception>(fieldName, new FormatException($"The {fieldName} key material did not contain a PEM-encoded public key."));
            }
        }
        catch (Exception ex)
        {
            return new ValueTuple<string, Exception>(fieldName, ex);
        }
        return null;
    }
}
