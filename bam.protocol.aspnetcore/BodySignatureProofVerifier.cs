using Bam.Encryption;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// The ASP.NET binding of the existing body-signature convention: the signature is
/// <c>SHA256WITHECDSA</c> (by default) over the UTF-8 request body, base64 in
/// <c>X-Bam-Body-Signature</c>, made with the actor's ECC private key and verified here with the
/// registered ECC public key through <see cref="EccSignatureProvider"/>. No new cryptography.
/// </summary>
public sealed class BodySignatureProofVerifier : IRequestProof
{
    /// <summary>The algorithm assumed when <c>X-Bam-Body-Signature-Algorithm</c> is absent.</summary>
    public const string DefaultAlgorithm = "SHA256WITHECDSA";

    private readonly EccSignatureProvider _signatures = new EccSignatureProvider();

    /// <inheritdoc />
    public bool Verify(string body, string signatureBase64, string? algorithm, string eccPublicKeyPem)
    {
        if (body is null || string.IsNullOrWhiteSpace(signatureBase64) || string.IsNullOrWhiteSpace(eccPublicKeyPem))
        {
            return false;
        }

        try
        {
            Signature signature = new Signature
            {
                SignatureBytes = Convert.FromBase64String(signatureBase64),
                Data = body,
                Algorithm = string.IsNullOrWhiteSpace(algorithm) ? DefaultAlgorithm : algorithm,
            };
            EccPublicKey publicKey = new EccPublicKey(eccPublicKeyPem);
            return _signatures.VerifySignature(signature, publicKey).Success;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
