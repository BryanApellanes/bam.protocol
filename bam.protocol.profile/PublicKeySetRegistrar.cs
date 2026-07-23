using Bam.Data.Objects;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Profile;

/// <summary>
/// Enforces the public key-set registration policy that anchors device-key account
/// confirmation (bam.protocol#8): first registration wins, rotation requires proof of
/// possession of the currently registered key and updates the existing row in place, and
/// resolution is deterministic (earliest-created row wins over any duplicates).  Composed
/// into <see cref="EncryptedProfileRepository"/> so the policy stays independently testable.
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
        lock (_registrationLock)
        {
            PublicKeySetData existing = Resolve(publicKeySetData.KeySetHandle);
            if (existing != null)
            {
                throw new PublicKeySetConflictException(publicKeySetData.KeySetHandle);
            }
            return Repository.Create(publicKeySetData);
        }
    }

    /// <inheritdoc />
    public PublicKeySetData Rotate(PublicKeySetData newKeySet, byte[] rotationSignature)
    {
        lock (_registrationLock)
        {
            PublicKeySetData current = Resolve(newKeySet.KeySetHandle);
            if (current == null)
            {
                throw new InvalidKeySetRotationException(newKeySet.KeySetHandle,
                    $"No key set is registered for handle '{newKeySet.KeySetHandle}'. Rotation replaces an existing key set; use registration for a first key set.");
            }

            ISignatureVerification verification;
            try
            {
                verification = RotationVerifier.Verify(current, newKeySet, rotationSignature);
            }
            catch (Exception ex)
            {
                throw new InvalidKeySetRotationException(newKeySet.KeySetHandle,
                    $"Rotation proof could not be verified for handle '{newKeySet.KeySetHandle}': {ex.Message}");
            }

            if (!verification.Success)
            {
                throw new InvalidKeySetRotationException(newKeySet.KeySetHandle,
                    $"Rotation signature does not prove possession of the currently registered key for handle '{newKeySet.KeySetHandle}'.");
            }

            current.PublicRsaKey = newKeySet.PublicRsaKey;
            current.PublicEccKey = newKeySet.PublicEccKey;
            return Repository.Update(current);
        }
    }

    /// <inheritdoc />
    public PublicKeySetData Resolve(string keySetHandle)
    {
        return Repository.Query<PublicKeySetData>(p => p.KeySetHandle == keySetHandle)
            .OrderBy(p => p.Created ?? DateTime.MaxValue)
            .ThenBy(p => p.Id)
            .FirstOrDefault()!;
    }
}
