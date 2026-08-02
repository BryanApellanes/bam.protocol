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
/// The registrar additionally: stamps the persisted creation time, composite-key identifiers,
/// and canonical key-material fingerprints server-side; enforces that public key material maps
/// to exactly one handle (compared by canonical identity, never raw PEM strings — see
/// <see cref="KeyMaterialIdentity"/>); blocklists revoked material while freeing revoked
/// handles (bam.protocol#11); validates key material is parseable before persisting; and logs
/// rejected registrations and rotations.  Handle equality is ordinal and case-sensitive.
/// </para>
/// <para>
/// Uniqueness checks resolve through the composed <see cref="IPublicKeySetResolver"/>, and
/// every admission decision takes the UNION of the resolver's lookup and one shared full-scan
/// snapshot before anything is admitted.  This scan-confirmation is LOAD-BEARING and must not
/// be removed on the belief that the resolver's lookups are index-backed: the store's search
/// index is NOT authoritative (it serves a query only when the queried column has an index
/// directory and the value is non-null, else it falls back to a scan — see
/// <see cref="IPublicKeySetResolver"/>'s indexed-vs-scan contract), so an index-only admission
/// could fail open on a legacy, unmigrated, or partially-indexed store.  The union removes the
/// question in both directions (empty AND non-empty indexed results).  Registration and
/// rotation are rare, lock-serialized operations, so they pay at most one scan per admission
/// while hot read paths keep the single-pass material lookup
/// (bam.data.objects#3 security review condition 5; bam.protocol#24 review SF5 + round-2 C1/C2).
/// </para>
/// </summary>
public class PublicKeySetRegistrar : IPublicKeySetRegistrar
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PublicKeySetRegistrar"/> class with the
    /// default resolution authority — a <see cref="PublicKeySetResolver"/> over the same
    /// repository.
    /// </summary>
    /// <param name="repository">The object-data repository the key sets are persisted in.</param>
    /// <param name="rotationVerifier">The verifier that decides whether a rotation signature proves possession of the current key.</param>
    public PublicKeySetRegistrar(ObjectDataRepository repository, IKeySetRotationVerifier rotationVerifier)
        : this(repository, rotationVerifier, new PublicKeySetResolver(repository))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PublicKeySetRegistrar"/> class.
    /// </summary>
    /// <param name="repository">The object-data repository the key sets are persisted in.</param>
    /// <param name="rotationVerifier">The verifier that decides whether a rotation signature proves possession of the current key.</param>
    /// <param name="resolver">The resolution authority used for handle-exists and key-material-uniqueness checks; see <see cref="IPublicKeySetResolver"/>.</param>
    public PublicKeySetRegistrar(ObjectDataRepository repository, IKeySetRotationVerifier rotationVerifier, IPublicKeySetResolver resolver)
    {
        this.Repository = repository;
        this.RotationVerifier = rotationVerifier;
        this.Resolver = resolver;
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

    /// <summary>
    /// Gets the resolution authority the registrar's uniqueness checks run through, so
    /// registration-time checks and read-path resolution can never disagree.
    /// </summary>
    protected IPublicKeySetResolver Resolver { get; }

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
            // One lazy snapshot serves every scan-confirmation this admission needs
            // (bam.protocol#24 review SF5) — at most one full materialization per admission.
            List<PublicKeySetData>? snapshot = null;
            List<PublicKeySetData> Snapshot() => snapshot ??= Repository.RetrieveAll<PublicKeySetData>().ToList();

            // First-registration-wins over ACTIVE rows only: a revoked tombstone frees its
            // handle for re-registration (bam.protocol#11).
            if (FindActiveRowByHandleConfirmed(handle, Snapshot) != null)
            {
                Log.Warn("Rejected key-set registration for handle '{0}': a key set is already registered.", handle);
                throw new PublicKeySetConflictException(handle);
            }

            // One-handle-to-one-key-material, by canonical identity. Revoked matches are
            // asymmetric: the material stays blocklisted under ANY handle (bam.protocol#11).
            PublicKeySetData? materialClaim = FindMaterialClaimConfirmed(publicKeySetData, excludeOwnActiveRow: false, Snapshot);
            if (materialClaim != null)
            {
                if (materialClaim.RevokedUtc != null)
                {
                    Log.Warn("Rejected key-set registration for handle '{0}': key material is revoked and blocklisted.", handle);
                    throw new RevokedKeyMaterialException(handle);
                }
                Log.Warn("Rejected key-set registration for handle '{0}': key material already registered under handle '{1}'.", handle, materialClaim.KeySetHandle);
                throw new PublicKeySetKeyMaterialConflictException(handle, materialClaim.KeySetHandle);
            }

            // Successor gate (bam.protocol#21): runs AFTER the active-handle and material-blocklist
            // checks (so revoked material can never pose as a "successor"). If the handle's governing
            // revocation bound an authorized successor, the freed handle can only be re-registered with
            // that exact key — otherwise the revoke→re-register window would let any first caller hijack
            // the handle. An unbound tombstone (null fingerprint) leaves the handle openly
            // re-registrable, unchanged. Compared over the candidate's RSA identity key via the shared
            // PublicKeyFingerprint (recomputed from ground truth, never a stamped fingerprint) so the
            // basis matches what the admin signed.
            PublicKeySetData? governingTombstone = FindGoverningTombstone(handle, Snapshot);
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
            List<PublicKeySetData>? snapshot = null;
            List<PublicKeySetData> Snapshot() => snapshot ??= Repository.RetrieveAll<PublicKeySetData>().ToList();

            PublicKeySetData? current = FindActiveRowByHandleConfirmed(handle, Snapshot);
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
            // challenger B1). Only the handle's own ACTIVE row is exempt (rotating to your own
            // current material remains a permitted no-op) — a revoked tombstone of the same
            // handle is NOT skipped, so rotating a handle back to its own revoked/blocklisted
            // material is caught (bam.protocol#18 B1).
            PublicKeySetData? materialClaim = FindMaterialClaimConfirmed(newKeySet, excludeOwnActiveRow: true, Snapshot);
            if (materialClaim != null)
            {
                if (materialClaim.RevokedUtc != null)
                {
                    Log.Warn("Rejected key-set rotation for handle '{0}': proposed key material is revoked and blocklisted.", handle);
                    throw new RevokedKeyMaterialException(handle);
                }
                Log.Warn("Rejected key-set rotation for handle '{0}': key material already registered under handle '{1}'.", handle, materialClaim.KeySetHandle);
                throw new PublicKeySetKeyMaterialConflictException(handle, materialClaim.KeySetHandle);
            }

            current.PublicRsaKey = newKeySet.PublicRsaKey;
            current.PublicEccKey = newKeySet.PublicEccKey;
            KeyMaterialIdentity.StampFingerprints(current);
            return Repository.Update(current);
        }
    }

    /// <inheritdoc />
    public PublicKeySetData? Resolve(string keySetHandle)
    {
        return Resolver.ResolveByHandle(keySetHandle);
    }

    /// <summary>
    /// Finds the handle's authoritative ACTIVE row as the UNION of the resolver's indexed
    /// lookup and the shared admission scan, earliest row winning.  The union — not
    /// indexed-first-scan-on-empty — is deliberate: an empty indexed result is not proof of
    /// absence, and a NON-empty indexed result over a partially-indexed store (a crash-window
    /// row, or a store never migrated via RebuildAsync) can name a LATER row than the true
    /// earliest — either way admission must decide from ground truth
    /// (bam.data.objects#3 security review condition 5; bam.protocol#24 security review
    /// residual 1 — Rotate's proof-of-possession anchor must not be displaceable by a partial
    /// index).  The scan cost is one shared snapshot per admission (rare, lock-serialized).
    /// </summary>
    private PublicKeySetData? FindActiveRowByHandleConfirmed(string handle, Func<List<PublicKeySetData>> snapshot)
    {
        PublicKeySetData? resolved = Resolver.ResolveByHandle(handle);
        PublicKeySetData? scanned = snapshot()
            .Where(keySet => keySet.KeySetHandle == handle && keySet.RevokedUtc == null)
            .OrderBy(keySet => keySet.Created)
            .ThenBy(keySet => keySet.Id)
            .FirstOrDefault();

        if (resolved == null || scanned == null)
        {
            return resolved ?? scanned;
        }

        return new[] { resolved, scanned }
            .OrderBy(keySet => keySet.Created)
            .ThenBy(keySet => keySet.Id)
            .First();
    }

    /// <summary>
    /// Finds the row whose material claim blocks the candidate, by canonical identity, as the
    /// UNION of the resolver's indexed claims and the shared admission scan (see
    /// <see cref="FindActiveRowByHandleConfirmed"/> for why admission never trusts the index
    /// alone): the earliest ACTIVE row sharing any of the candidate's material (a uniqueness
    /// conflict), or — only when no active claim exists — the earliest REVOKED row sharing it
    /// (blocklisted material; the caller distinguishes via
    /// <see cref="PublicKeySetData.RevokedUtc"/>).  When <paramref name="excludeOwnActiveRow"/>
    /// is true (rotation), the candidate handle's own ACTIVE row is exempt; its revoked
    /// tombstones are never exempt.
    /// </summary>
    private PublicKeySetData? FindMaterialClaimConfirmed(PublicKeySetData candidate, bool excludeOwnActiveRow, Func<List<PublicKeySetData>> snapshot)
    {
        IReadOnlyList<string> candidateIdentities = KeyMaterialIdentity.CandidateIdentities(candidate);
        Dictionary<string, PublicKeySetData> claimsByUuid = new Dictionary<string, PublicKeySetData>();
        foreach (PublicKeySetData claim in Resolver.FindKeyMaterialClaims(candidate))
        {
            claimsByUuid[claim.Uuid] = claim;
        }
        foreach (PublicKeySetData claim in snapshot()
                     .Where(keySet => keySet.KeySetHandle != candidate.KeySetHandle || keySet.RevokedUtc != null)
                     .Where(keySet => KeyMaterialIdentity.SharesMaterial(keySet, candidateIdentities)))
        {
            claimsByUuid[claim.Uuid] = claim;
        }

        IEnumerable<PublicKeySetData> eligible = claimsByUuid.Values
            .Where(keySet => !(excludeOwnActiveRow && keySet.KeySetHandle == candidate.KeySetHandle && keySet.RevokedUtc == null));

        return eligible
            .OrderBy(keySet => keySet.RevokedUtc != null)
            .ThenBy(keySet => keySet.Created)
            .ThenBy(keySet => keySet.Id)
            .FirstOrDefault();
    }

    /// <summary>
    /// Selects the <i>governing</i> tombstone for a handle — the same-handle revoked row with the most
    /// recent <see cref="PublicKeySetData.RevokedUtc"/> (<see cref="Bam.Data.Repositories.RepoData.Id"/>
    /// as a deterministic tiebreak) — or null when the handle has no revoked rows.  Its
    /// <see cref="PublicKeySetData.AuthorizedSuccessorFingerprint"/> is the binding a re-registration
    /// must satisfy, so a later revocation always supersedes an earlier one (bam.protocol#21).  Read
    /// from the shared admission snapshot (ground truth), consistent with the other admission checks.
    /// </summary>
    private static PublicKeySetData? FindGoverningTombstone(string handle, Func<List<PublicKeySetData>> snapshot)
    {
        return snapshot()
            .Where(keySet => keySet.KeySetHandle == handle && keySet.RevokedUtc != null)
            .OrderByDescending(keySet => keySet.RevokedUtc)
            .ThenByDescending(keySet => keySet.Id)
            .FirstOrDefault();
    }

    /// <summary>
    /// Stamps the fields the registrar owns rather than the caller: the creation time, the
    /// composite-key identifiers (<see cref="Bam.Data.Repositories.RepoData.Uuid"/> /
    /// <see cref="Bam.Data.Repositories.RepoData.Cuid"/>), and the canonical key-material
    /// fingerprints.  <c>Created</c> is the primary resolution key and <c>Uuid</c>/<c>Cuid</c>
    /// derive the <c>Id</c> tiebreak, so leaving any of them caller-controlled would let a
    /// caller influence which duplicate wins resolution (bam.protocol#8 review C3); the
    /// fingerprints are the indexed canonical identity (bam.protocol#24 review B1).
    /// </summary>
    private static void NormalizeServerControlledFields(PublicKeySetData publicKeySetData)
    {
        publicKeySetData.Created = DateTime.UtcNow;
        publicKeySetData.Uuid = Guid.NewGuid().ToString();
        publicKeySetData.Cuid = Bam.Cuid.Generate();
        KeyMaterialIdentity.StampFingerprints(publicKeySetData);
        // A fresh registration is definitionally active and never a tombstone: clear any
        // caller-supplied revocation state so a caller cannot PLANT a governing tombstone
        // (RevokedUtc + AuthorizedSuccessorFingerprint) that would then authorize re-registering the
        // handle with an attacker's own key, or brick the handle outright (bam.protocol#25 review B1 —
        // the security-auditor's caller-planted-tombstone finding; the #8 review C3 established the
        // same server-owns-these-fields principle for Created/Uuid/Cuid).
        publicKeySetData.RevokedUtc = null;
        publicKeySetData.RevokedBy = null;
        publicKeySetData.AuthorizedSuccessorFingerprint = null;
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
