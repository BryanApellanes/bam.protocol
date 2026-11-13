using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Profile;

/// <summary>
/// Proves a rotation away from a key set that has an ECC key and no RSA key: a SHA256WITHECDSA
/// signature over <see cref="KeySetRotationPayload.ComposeFor"/>, made with the private key
/// matching the current public ECC key (bam.protocol#72).  A key set with an RSA key is anchored
/// by the RSA proof instead, so this proof never widens who can rotate an RSA key set.
/// </summary>
public class EcdsaKeySetRotationProof : IKeySetRotationProof
{
    /// <summary>
    /// The BouncyCastle signature algorithm this proof verifies.
    /// </summary>
    public const string Algorithm = "SHA256WITHECDSA";

    /// <summary>
    /// The name recorded when this proof authorizes a rotation.
    /// </summary>
    public const string ProofName = "ecdsa";

    /// <summary>
    /// Initializes a new instance of the <see cref="EcdsaKeySetRotationProof"/> class.
    /// </summary>
    /// <param name="signatureProvider">The provider that verifies the signature.</param>
    public EcdsaKeySetRotationProof(ISignatureProvider signatureProvider)
    {
        ArgumentNullException.ThrowIfNull(signatureProvider);
        this.SignatureProvider = signatureProvider;
    }

    /// <summary>
    /// Gets the provider that verifies the signature.
    /// </summary>
    protected ISignatureProvider SignatureProvider { get; }

    /// <inheritdoc />
    public string Name => ProofName;

    /// <inheritdoc />
    public bool Anchors(PublicKeySetData current)
    {
        return current is not null && string.IsNullOrEmpty(current.PublicRsaKey) && !string.IsNullOrEmpty(current.PublicEccKey);
    }

    /// <inheritdoc />
    public ISignatureVerification Verify(PublicKeySetData current, PublicKeySetData proposed, byte[] proof)
    {
        if (!Anchors(current))
        {
            return new SignatureVerification { Success = false, Message = "The current key set is not ECC-only.", Signature = null!, IssuerPublicKey = null! };
        }
        Signature signature = new Signature
        {
            SignatureBytes = proof,
            Data = KeySetRotationPayload.ComposeFor(current, proposed),
            Algorithm = Algorithm
        };
        EccPublicKey currentPublicKey = new EccPublicKey(current.PublicEccKey);
        return SignatureProvider.VerifySignature(signature, currentPublicKey);
    }
}
