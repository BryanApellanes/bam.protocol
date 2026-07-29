using Bam.Encryption;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Data;

/// <summary>
/// Decides whether a break-glass admin proof authorizes revoking a specific target key set.
/// The proof must be a signature over the target-bound <see cref="RevocationPayload"/>, verified
/// against the configured admin public key (<see cref="IAdminPublicKeySource"/>) — proving
/// <i>admin authority</i>, the revocation analogue of the rotation proof's key possession.
/// </summary>
public interface IRevocationAuthority
{
    /// <summary>
    /// Verifies that <paramref name="adminProof"/> is a valid break-glass signature over
    /// <see cref="RevocationPayload.Compose"/> of <paramref name="target"/> bound to
    /// <paramref name="authorizedSuccessorFingerprint"/>, made with the private key matching the
    /// configured admin public key.  Because the successor is part of the signed payload, a proof
    /// authorizes revoking the target <i>and</i> names the successor in one indivisible signature.
    /// </summary>
    /// <param name="target">The registered key set proposed for revocation.</param>
    /// <param name="adminProof">The raw admin signature bytes.</param>
    /// <param name="authorizedSuccessorFingerprint">
    /// The canonical fingerprint of the key authorized to re-register the freed handle, or null to
    /// bind no successor (the proof then authorizes an open re-registration).
    /// </param>
    /// <returns>The verification result; <see cref="ISignatureVerification.Success"/> is true only for a valid, authorized proof. Fails closed when no admin key is configured.</returns>
    ISignatureVerification Verify(PublicKeySetData target, byte[] adminProof, string? authorizedSuccessorFingerprint);
}
