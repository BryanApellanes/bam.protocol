using Bam.Encryption;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// The ASP.NET binding of the existing body-signature convention: the signature is
/// <c>SHA256WITHECDSA</c> over the UTF-8 request body, base64 in <c>X-Bam-Body-Signature</c>, made with
/// the actor's ECC private key and verified here with the registered ECC public key through
/// <see cref="EccSignatureProvider"/>. No new cryptography. The algorithm is pinned: an
/// <c>X-Bam-Body-Signature-Algorithm</c> outside <see cref="ActorAuthenticationOptions.AllowedBodySignatureAlgorithms"/>
/// fails verification, because a verifier that lets the signer pick the scheme can be handed one that
/// doesn't hash the input.
/// </summary>
public sealed class BodySignatureProofVerifier : IRequestProof
{
    /// <summary>The algorithm assumed when <c>X-Bam-Body-Signature-Algorithm</c> is absent.</summary>
    public const string DefaultAlgorithm = "SHA256WITHECDSA";

    private readonly EccSignatureProvider _signatures = new EccSignatureProvider();
    private readonly IReadOnlyList<string> _allowedAlgorithms;

    /// <summary>Creates a verifier that accepts <see cref="DefaultAlgorithm"/> only.</summary>
    public BodySignatureProofVerifier()
        : this(new ActorAuthenticationOptions())
    {
    }

    /// <summary>Creates a verifier that accepts the algorithms the options allow.</summary>
    /// <param name="options">Carries <see cref="ActorAuthenticationOptions.AllowedBodySignatureAlgorithms"/>.</param>
    public BodySignatureProofVerifier(ActorAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _allowedAlgorithms = options.AllowedBodySignatureAlgorithms ?? Array.Empty<string>();
    }

    /// <summary>Whether an algorithm name is on the allow-list (a missing name means <see cref="DefaultAlgorithm"/>).</summary>
    /// <param name="algorithm">The algorithm named by the request, or null.</param>
    /// <returns>The algorithm to verify with, or null when it is not allowed.</returns>
    public string? AllowedAlgorithm(string? algorithm)
    {
        string requested = string.IsNullOrWhiteSpace(algorithm) ? DefaultAlgorithm : algorithm.Trim();
        foreach (string allowed in _allowedAlgorithms)
        {
            if (string.Equals(allowed, requested, StringComparison.OrdinalIgnoreCase))
            {
                return allowed;
            }
        }

        return null;
    }

    /// <inheritdoc />
    public bool Verify(string body, string signatureBase64, string? algorithm, string eccPublicKeyPem)
    {
        if (body is null || string.IsNullOrWhiteSpace(signatureBase64) || string.IsNullOrWhiteSpace(eccPublicKeyPem))
        {
            return false;
        }

        string? allowedAlgorithm = AllowedAlgorithm(algorithm);
        if (allowedAlgorithm is null)
        {
            return false;
        }

        try
        {
            Signature signature = new Signature
            {
                SignatureBytes = Convert.FromBase64String(signatureBase64),
                Data = body,
                Algorithm = allowedAlgorithm,
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
