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
    private static readonly object _registrationLock = new object();

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

        lock (_registrationLock)
        {
            // Single materialization: check both first-registration-wins (handle already taken)
            // and one-handle-to-one-key-material in one pass, rather than a Resolve scan plus a
            // separate uniqueness scan (bam.protocol#8 review S6). True indexed lookup is a
            // store-level concern tracked in bam.protocol#13.
            bool handleExists = false;
            string? conflictingHandle = null;
            foreach (PublicKeySetData existing in Repository.RetrieveAll<PublicKeySetData>())
            {
                if (existing.KeySetHandle == handle)
                {
                    handleExists = true;
                    continue;
                }
                if (conflictingHandle == null && SharesKeyMaterial(existing, publicKeySetData))
                {
                    conflictingHandle = existing.KeySetHandle;
                }
            }

            if (handleExists)
            {
                Log.Warn("Rejected key-set registration for handle '{0}': a key set is already registered.", handle);
                throw new PublicKeySetConflictException(handle);
            }
            if (conflictingHandle != null)
            {
                Log.Warn("Rejected key-set registration for handle '{0}': key material already registered under handle '{1}'.", handle, conflictingHandle);
                throw new PublicKeySetKeyMaterialConflictException(handle, conflictingHandle);
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

        lock (_registrationLock)
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
            // current material remains a permitted no-op.
            string? conflictingHandle = FindHandleRegisteringSameKeyMaterial(newKeySet);
            if (conflictingHandle != null)
            {
                Log.Warn("Rejected key-set rotation for handle '{0}': key material already registered under handle '{1}'.", handle, conflictingHandle);
                throw new PublicKeySetKeyMaterialConflictException(handle, conflictingHandle);
            }

            current.PublicRsaKey = newKeySet.PublicRsaKey;
            current.PublicEccKey = newKeySet.PublicEccKey;
            return Repository.Update(current);
        }
    }

    /// <inheritdoc />
    public PublicKeySetData? Resolve(string keySetHandle)
    {
        return Repository.Query<PublicKeySetData>(p => p.KeySetHandle == keySetHandle)
            .OrderBy(p => p.Created)
            .ThenBy(p => p.Id)
            .FirstOrDefault();
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
    /// Returns the handle a piece of the key set's public key material is already registered
    /// under (RSA or ECC) by a <i>different</i> handle, or null when the material is not
    /// registered elsewhere.  Enforces the one handle-to-one key-set invariant that
    /// <c>FindProfileByPublicKey</c> relies on; used by the rotation path (Register uses a
    /// combined single-pass scan).
    /// </summary>
    private string? FindHandleRegisteringSameKeyMaterial(PublicKeySetData keySet)
    {
        foreach (PublicKeySetData existing in Repository.RetrieveAll<PublicKeySetData>())
        {
            if (existing.KeySetHandle == keySet.KeySetHandle)
            {
                continue;
            }
            if (SharesKeyMaterial(existing, keySet))
            {
                return existing.KeySetHandle;
            }
        }
        return null;
    }

    /// <summary>
    /// True when <paramref name="existing"/> carries the same non-empty RSA or ECC public key
    /// material as <paramref name="candidate"/>.
    /// </summary>
    private static bool SharesKeyMaterial(PublicKeySetData existing, PublicKeySetData candidate)
    {
        if (!string.IsNullOrEmpty(candidate.PublicRsaKey) && existing.PublicRsaKey == candidate.PublicRsaKey)
        {
            return true;
        }
        if (!string.IsNullOrEmpty(candidate.PublicEccKey) && existing.PublicEccKey == candidate.PublicEccKey)
        {
            return true;
        }
        return false;
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
