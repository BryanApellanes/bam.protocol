using Bam;
using Bam.Data.Objects;
using Bam.Encryption;
using Bam.Logging;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Profile;

/// <summary>
/// Revokes a registered key set under a verified break-glass admin proof (bam.protocol#11).
/// Resolves the active (non-revoked) row for the handle, verifies the admin proof via
/// <see cref="IRevocationAuthority"/>, then tombstones the row in place
/// (<see cref="PublicKeySetData.RevokedUtc"/> / <see cref="PublicKeySetData.RevokedBy"/>).  The
/// handle is thereby freed for re-registration while the revoked key material stays blocklisted.
/// Composed into <see cref="EncryptedProfileRepository"/> alongside
/// <see cref="PublicKeySetRegistrar"/>, sharing <see cref="KeySetRegistrationLock"/> so the
/// tombstone is atomic against a concurrent registration.
/// </summary>
public class KeySetRevocation : IKeySetRevocation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeySetRevocation"/> class.
    /// </summary>
    /// <param name="repository">The object-data repository the key sets are persisted in.</param>
    /// <param name="revocationAuthority">The authority that verifies break-glass admin proofs.</param>
    /// <param name="adminPublicKeySource">The source of the admin public key, used to record which key authorized a revocation.</param>
    public KeySetRevocation(ObjectDataRepository repository, IRevocationAuthority revocationAuthority, IAdminPublicKeySource adminPublicKeySource)
    {
        this.Repository = repository;
        this.RevocationAuthority = revocationAuthority;
        this.AdminPublicKeySource = adminPublicKeySource;
    }

    /// <summary>
    /// Gets the object-data repository the key sets are persisted in.
    /// </summary>
    protected ObjectDataRepository Repository { get; }

    /// <summary>
    /// Gets the authority that verifies break-glass admin proofs.
    /// </summary>
    protected IRevocationAuthority RevocationAuthority { get; }

    /// <summary>
    /// Gets the source of the admin public key.
    /// </summary>
    protected IAdminPublicKeySource AdminPublicKeySource { get; }

    /// <inheritdoc />
    public PublicKeySetData Revoke(string keySetHandle, byte[] adminProof)
    {
        if (string.IsNullOrWhiteSpace(keySetHandle))
        {
            throw new ArgumentException("A key set handle is required.", nameof(keySetHandle));
        }

        lock (KeySetRegistrationLock.Sync)
        {
            PublicKeySetData? active = ResolveActive(keySetHandle);
            if (active == null)
            {
                throw new KeySetRevocationException(keySetHandle,
                    $"No active key set is registered for handle '{keySetHandle}'.");
            }

            ISignatureVerification verification;
            try
            {
                verification = RevocationAuthority.Verify(active, adminProof);
            }
            catch (Exception ex)
            {
                Log.Warn("Rejected revocation for handle '{0}': admin proof could not be verified ({1}).", keySetHandle, ex.Message);
                throw new UnauthorizedRevocationException(keySetHandle, ex);
            }

            if (!verification.Success)
            {
                Log.Warn("Rejected revocation for handle '{0}': admin proof did not authorize the revocation.", keySetHandle);
                throw new UnauthorizedRevocationException(keySetHandle);
            }

            active.RevokedUtc = DateTime.UtcNow;
            active.RevokedBy = (AdminPublicKeySource.AdminPublicRsaKey ?? string.Empty).Sha256();
            PublicKeySetData tombstoned = Repository.Update(active);
            Log.Info("Revoked key set for handle '{0}' (authorized by admin key {1}).", keySetHandle, tombstoned.RevokedBy);
            return tombstoned;
        }
    }

    /// <summary>
    /// Resolves the active (non-revoked) key set for a handle deterministically (earliest-created
    /// wins), or null when none is active.
    /// </summary>
    private PublicKeySetData? ResolveActive(string keySetHandle)
    {
        return Repository.Query<PublicKeySetData>(p => p.KeySetHandle == keySetHandle && p.RevokedUtc == null)
            .OrderBy(p => p.Created)
            .ThenBy(p => p.Id)
            .FirstOrDefault();
    }
}
