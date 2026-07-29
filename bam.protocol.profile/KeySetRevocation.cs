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
    public KeySetRevocation(ObjectDataRepository repository, IRevocationAuthority revocationAuthority)
    {
        this.Repository = repository;
        this.RevocationAuthority = revocationAuthority;
    }

    /// <summary>
    /// Gets the object-data repository the key sets are persisted in.
    /// </summary>
    protected ObjectDataRepository Repository { get; }

    /// <summary>
    /// Gets the authority that verifies break-glass admin proofs.
    /// </summary>
    protected IRevocationAuthority RevocationAuthority { get; }

    /// <inheritdoc />
    public PublicKeySetData Revoke(string keySetHandle, byte[] adminProof, string? authorizedSuccessorFingerprint)
    {
        if (string.IsNullOrWhiteSpace(keySetHandle))
        {
            throw new ArgumentException("A key set handle is required.", nameof(keySetHandle));
        }
        ArgumentNullException.ThrowIfNull(adminProof);

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
                // The successor fingerprint is part of the signed payload, so the admin proof both
                // authorizes the revocation and binds the successor in one indivisible signature — a
                // caller cannot substitute a different successor than the admin signed (bam.protocol#21).
                verification = RevocationAuthority.Verify(active, adminProof, authorizedSuccessorFingerprint);
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
            // Record the key that ACTUALLY authorized this revocation, taken from the verification
            // result, not a separately-injected admin-key source — so the audit trail stays correct
            // once bam.protocol#17 introduces more than one valid admin key (review SF1 / T5).
            active.RevokedBy = verification.IssuerPublicKey?.Pem?.Sha256();
            // Bind the admin-authorized successor onto the tombstone so Register can gate re-registration
            // of the freed handle on it (bam.protocol#21); null leaves the handle openly re-registrable.
            active.AuthorizedSuccessorFingerprint = authorizedSuccessorFingerprint;
            PublicKeySetData tombstoned = Repository.Update(active);
            if (string.IsNullOrEmpty(authorizedSuccessorFingerprint))
            {
                Log.Info("Revoked key set for handle '{0}' (authorized by admin key {1}); no successor bound — handle is openly re-registrable.", keySetHandle, tombstoned.RevokedBy);
            }
            else
            {
                Log.Info("Revoked key set for handle '{0}' (authorized by admin key {1}); bound successor {2}.", keySetHandle, tombstoned.RevokedBy, authorizedSuccessorFingerprint);
            }
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
