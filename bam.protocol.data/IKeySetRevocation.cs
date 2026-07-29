using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Data;

/// <summary>
/// Revokes a registered key set under a break-glass admin proof — the recovery path the
/// registration trust anchor otherwise lacks (bam.protocol#11).  Revocation tombstones the row
/// (<see cref="PublicKeySetData.RevokedUtc"/>) rather than deleting it: the handle becomes free
/// to re-register, but the revoked key material stays blocklisted so a compromised key cannot be
/// re-registered.  A revoked key set is no longer authoritative for resolution or device-key
/// confirmation.
/// </summary>
public interface IKeySetRevocation
{
    /// <summary>
    /// Revokes the active key set registered under <paramref name="keySetHandle"/>, provided
    /// <paramref name="adminProof"/> is a valid break-glass signature over the target-bound
    /// <see cref="RevocationPayload"/> for that key set, bound to
    /// <paramref name="authorizedSuccessorFingerprint"/>.  Tombstones the row in place and records
    /// the authorized successor on the tombstone.
    /// </summary>
    /// <param name="keySetHandle">The handle whose active key set is being revoked.</param>
    /// <param name="adminProof">The raw admin signature bytes authorizing the revocation.</param>
    /// <param name="authorizedSuccessorFingerprint">
    /// The canonical fingerprint (<see cref="Bam.Protocol.Profile.PublicKeyFingerprint"/>) of the key
    /// the admin authorizes to re-register the freed handle, or null to bind no successor (leaving the
    /// handle openly re-registrable — the pre-#21 behavior).  Whichever is supplied MUST match the
    /// value the admin signed, or the proof fails verification (bam.protocol#21).
    /// </param>
    /// <returns>The tombstoned key set.</returns>
    /// <exception cref="System.ArgumentException">The handle is null, empty, or whitespace.</exception>
    /// <exception cref="KeySetRevocationException">No active key set is registered for the handle.</exception>
    /// <exception cref="UnauthorizedRevocationException">The admin proof does not authorize the revocation.</exception>
    PublicKeySetData Revoke(string keySetHandle, byte[] adminProof, string? authorizedSuccessorFingerprint);
}
