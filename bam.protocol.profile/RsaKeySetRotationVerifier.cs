using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Profile;

/// <summary>
/// Verifies key-set rotation signatures produced by the framework RSA signing convention
/// (SHA512WITHRSA): the signed payload is <see cref="KeySetRotationPayload.Compose"/> of the
/// proposed key set, and the signature is verified against the public RSA key of the
/// currently registered key set — so only the holder of the current private key can rotate.
/// Mirrors the challenge-verification pattern established by bam.useraccounts'
/// RsaChallengeSignatureVerifier.
/// </summary>
public class RsaKeySetRotationVerifier : IKeySetRotationVerifier
{
    /// <summary>
    /// The BouncyCastle signature algorithm used by the framework RSA signing convention.
    /// </summary>
    public const string Algorithm = "SHA512WITHRSA";

    /// <summary>
    /// Initializes a new instance of the <see cref="RsaKeySetRotationVerifier"/> class.
    /// </summary>
    /// <param name="signatureProvider">The signature provider used to verify rotation signatures.</param>
    public RsaKeySetRotationVerifier(ISignatureProvider signatureProvider)
    {
        this.SignatureProvider = signatureProvider;
    }

    /// <summary>
    /// Gets the signature provider used to verify rotation signatures.
    /// </summary>
    protected ISignatureProvider SignatureProvider { get; }

    /// <inheritdoc />
    public ISignatureVerification Verify(PublicKeySetData current, PublicKeySetData proposed, byte[] rotationSignature)
    {
        Signature signature = new Signature
        {
            SignatureBytes = rotationSignature,
            Data = KeySetRotationPayload.Compose(proposed),
            Algorithm = Algorithm
        };
        RsaPublicKey currentPublicKey = new RsaPublicKey(current.PublicRsaKey);
        return SignatureProvider.VerifySignature(signature, currentPublicKey);
    }
}
