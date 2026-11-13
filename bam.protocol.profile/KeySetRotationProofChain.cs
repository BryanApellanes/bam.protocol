using Bam.Encryption;
using Bam.Logging;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Profile;

/// <summary>
/// Verifies a key-set rotation with the host's trusted proofs (bam.protocol#72): the rotation is
/// accepted when one proof that anchors the current key set verifies it.  Proofs are tried in
/// order; a proof that throws counts as a failure and never stops a later proof from being tried,
/// and a later failure never undoes an earlier success.
/// </summary>
public class KeySetRotationProofChain : IKeySetRotationVerifier
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeySetRotationProofChain"/> class.
    /// </summary>
    /// <param name="proofs">The proofs this host trusts.</param>
    public KeySetRotationProofChain(KeySetRotationProofs proofs)
    {
        ArgumentNullException.ThrowIfNull(proofs);
        this.Proofs = proofs;
    }

    /// <summary>
    /// Returns the framework's default proofs: RSA (SHA512WITHRSA by the current RSA key), then
    /// ECDSA (SHA256WITHECDSA by the current ECC key, for ECC-only key sets).
    /// </summary>
    /// <returns>A new proof list a host may add to.</returns>
    public static KeySetRotationProofs DefaultProofs()
    {
        return new KeySetRotationProofs(
            new RsaKeySetRotationVerifier(new RsaSignatureProvider()),
            new EcdsaKeySetRotationProof(new EccSignatureProvider()));
    }

    /// <summary>
    /// Returns a chain over <see cref="DefaultProofs"/>.
    /// </summary>
    /// <returns>A new chain.</returns>
    public static KeySetRotationProofChain Default()
    {
        return new KeySetRotationProofChain(DefaultProofs());
    }

    /// <summary>
    /// Gets the proofs this chain tries.
    /// </summary>
    public KeySetRotationProofs Proofs { get; }

    /// <inheritdoc />
    public ISignatureVerification Verify(PublicKeySetData current, PublicKeySetData proposed, byte[] rotationSignature)
    {
        ArgumentNullException.ThrowIfNull(current);
        ArgumentNullException.ThrowIfNull(proposed);
        IReadOnlyList<IKeySetRotationProof> anchoring = Proofs.Anchoring(current);
        if (anchoring.Count == 0)
        {
            return Failed("No trusted rotation proof can prove a rotation away from this key set.");
        }

        List<string> failures = new List<string>();
        foreach (IKeySetRotationProof proof in anchoring)
        {
            ISignatureVerification verification;
            try
            {
                verification = proof.Verify(current, proposed, rotationSignature ?? Array.Empty<byte>());
            }
            catch (Exception ex)
            {
                Log.Warn("Rotation proof '{0}' threw while verifying a rotation for handle '{1}': {2}", proof.Name, current.KeySetHandle, ex.Message);
                failures.Add($"{proof.Name}: {ex.Message}");
                continue;
            }

            if (verification.Success)
            {
                return new KeySetRotationVerification
                {
                    Success = true,
                    ProofName = proof.Name,
                    Message = string.Empty,
                    Signature = verification.Signature,
                    IssuerPublicKey = verification.IssuerPublicKey
                };
            }
            failures.Add(string.IsNullOrEmpty(verification.Message) ? $"{proof.Name}: refused" : $"{proof.Name}: {verification.Message}");
        }
        return Failed(string.Join("; ", failures));
    }

    /// <inheritdoc />
    public bool CanRotateFrom(PublicKeySetData keySet)
    {
        ArgumentNullException.ThrowIfNull(keySet);
        return Proofs.AnyAnchors(keySet);
    }

    private static KeySetRotationVerification Failed(string message)
    {
        return new KeySetRotationVerification
        {
            Success = false,
            ProofName = null,
            Message = message,
            Signature = null!,
            IssuerPublicKey = null!
        };
    }
}
