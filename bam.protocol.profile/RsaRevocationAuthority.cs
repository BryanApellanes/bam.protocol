using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Profile;

/// <summary>
/// Authorizes key-set revocation by verifying a break-glass admin signature (SHA512WITHRSA) over
/// the target-bound <see cref="RevocationPayload"/> against the configured admin public key
/// (<see cref="IAdminPublicKeySource"/>).  Mirrors <see cref="RsaKeySetRotationVerifier"/>, but
/// proves <i>admin authority</i> rather than key possession.  Fails closed when no admin key is
/// configured, and when the admin key material is malformed.
/// </summary>
public class RsaRevocationAuthority : IRevocationAuthority
{
    /// <summary>
    /// The BouncyCastle signature algorithm used by the framework RSA signing convention.
    /// </summary>
    public const string Algorithm = "SHA512WITHRSA";

    /// <summary>
    /// Initializes a new instance of the <see cref="RsaRevocationAuthority"/> class.
    /// </summary>
    /// <param name="signatureProvider">The signature provider used to verify admin proofs.</param>
    /// <param name="adminPublicKeySource">The source of the break-glass admin public key.</param>
    public RsaRevocationAuthority(ISignatureProvider signatureProvider, IAdminPublicKeySource adminPublicKeySource)
    {
        this.SignatureProvider = signatureProvider;
        this.AdminPublicKeySource = adminPublicKeySource;
    }

    /// <summary>
    /// Gets the signature provider used to verify admin proofs.
    /// </summary>
    protected ISignatureProvider SignatureProvider { get; }

    /// <summary>
    /// Gets the source of the break-glass admin public key.
    /// </summary>
    protected IAdminPublicKeySource AdminPublicKeySource { get; }

    /// <inheritdoc />
    public ISignatureVerification Verify(PublicKeySetData target, byte[] adminProof)
    {
        string? adminPublicRsaKey = AdminPublicKeySource.AdminPublicRsaKey;
        if (string.IsNullOrEmpty(adminPublicRsaKey))
        {
            // Fail closed: with no break-glass key configured, no revocation can be authorized.
            return new SignatureVerification
            {
                Success = false,
                Message = "No break-glass admin key is configured; revocation cannot be authorized."
            };
        }

        RsaPublicKey adminPublicKey;
        try
        {
            adminPublicKey = new RsaPublicKey(adminPublicRsaKey);
        }
        catch (Exception ex)
        {
            // Fail closed on malformed admin key material rather than throwing, so the contract
            // holds for any direct consumer of IRevocationAuthority (review nit).
            return new SignatureVerification
            {
                Success = false,
                Message = $"The configured break-glass admin key is not a parseable RSA public key: {ex.Message}"
            };
        }

        Signature signature = new Signature
        {
            SignatureBytes = adminProof,
            Data = RevocationPayload.Compose(target),
            Algorithm = Algorithm
        };
        return SignatureProvider.VerifySignature(signature, adminPublicKey);
    }
}
