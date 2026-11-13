using Bam.Encryption;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Data;

/// <summary>
/// One way of proving that a key-set rotation was authorized.  A host trusts a list of these
/// (<see cref="KeySetRotationProofs"/>); a rotation is accepted when one proof that
/// <see cref="Anchors"/> the current key set verifies it (bam.protocol#72).  Each proof must be
/// sound on its own: the chain never combines partial results.
/// </summary>
public interface IKeySetRotationProof
{
    /// <summary>
    /// Gets the short name recorded in the audit trail when this proof authorizes a rotation
    /// (for example <c>rsa</c>, <c>ecdsa</c> or <c>passkey</c>).
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Returns whether this proof can prove a rotation away from <paramref name="current"/>.
    /// A proof that does not anchor a key set is never asked to verify a rotation from it.
    /// <para>
    /// This is also asked about a <i>proposed</i> key set that isn't registered yet, to refuse a
    /// rotation no trusted proof could ever rotate away from
    /// (<see cref="IKeySetRotationVerifier.CanRotateFrom"/>).  A proof whose answer depends on
    /// stored state (which credential holds the key set, whether a recovery is due) must answer
    /// correctly for such a set, or answer from the key material alone.  It must anchor only key
    /// sets bound to the credential it verifies, and only for that credential's handle.
    /// </para>
    /// </summary>
    /// <param name="current">The key set a rotation would move away from.</param>
    /// <returns>True when this proof can prove a rotation from <paramref name="current"/>.</returns>
    bool Anchors(PublicKeySetData current);

    /// <summary>
    /// Verifies that <paramref name="proof"/> authorizes rotating <paramref name="current"/>
    /// to <paramref name="proposed"/>.  Returns a failed verification, rather than throwing,
    /// for a proof in a format this proof does not read.
    /// </summary>
    /// <param name="current">The currently registered key set.</param>
    /// <param name="proposed">The key set being rotated to.</param>
    /// <param name="proof">The proof bytes the caller presented.</param>
    /// <returns>The verification result; <see cref="ISignatureVerification.Success"/> is true only for a valid proof.</returns>
    ISignatureVerification Verify(PublicKeySetData current, PublicKeySetData proposed, byte[] proof);
}
