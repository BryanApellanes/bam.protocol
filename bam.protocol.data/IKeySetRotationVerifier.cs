using Bam.Encryption;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Data;

/// <summary>
/// Decides whether a rotation signature proves possession of the currently registered key for
/// a handle.  A key set may only be replaced when the caller demonstrates control of the key
/// it replaces; this is the possession proof that keeps rotation from becoming an
/// unauthenticated overwrite of the confirmation trust anchor.
/// </summary>
public interface IKeySetRotationVerifier
{
    /// <summary>
    /// Verifies that <paramref name="rotationSignature"/> is a valid signature over the
    /// canonical payload of <paramref name="proposed"/> (see
    /// <see cref="KeySetRotationPayload.Compose"/>), made with the private key matching
    /// <paramref name="current"/>'s registered public RSA key.
    /// </summary>
    /// <param name="current">The currently registered key set whose public RSA key verifies the proof.</param>
    /// <param name="proposed">The key set being rotated to; its canonical payload is the signed data.</param>
    /// <param name="rotationSignature">The raw signature bytes presented as proof of possession.</param>
    /// <returns>The verification result; <see cref="ISignatureVerification.Success"/> is true only for a valid proof.</returns>
    ISignatureVerification Verify(PublicKeySetData current, PublicKeySetData proposed, byte[] rotationSignature);

    /// <summary>
    /// Returns whether this verifier could later verify a rotation away from
    /// <paramref name="keySet"/>.  The registrar refuses to rotate into a key set no verifier
    /// can rotate away from, which would strand the handle.  The default is the original rule,
    /// an RSA key, so a verifier written before bam.protocol#72 keeps its behaviour;
    /// <c>KeySetRotationProofChain</c> answers from its trusted proofs.
    /// </summary>
    /// <param name="keySet">A proposed key set.</param>
    /// <returns>True when a rotation away from <paramref name="keySet"/> could be proven.</returns>
    bool CanRotateFrom(PublicKeySetData keySet) => !string.IsNullOrEmpty(keySet.PublicRsaKey);
}
