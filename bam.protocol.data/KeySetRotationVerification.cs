using Bam.Encryption;

namespace Bam.Protocol.Data;

/// <summary>
/// A rotation verification that also names the proof that decided it, so the registrar can
/// record which kind of proof authorized a rotation.
/// </summary>
public class KeySetRotationVerification : SignatureVerification
{
    /// <summary>
    /// Gets or sets the <see cref="IKeySetRotationProof.Name"/> of the proof that verified the
    /// rotation, or null when none did.
    /// </summary>
    public string? ProofName { get; set; }
}
