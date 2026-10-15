using System.Globalization;
using System.Security.Cryptography;
using Bam.Encryption;
using Bam.Web;

namespace Bam.Protocol;

/// <summary>
/// The client side of the request proof: signs the <see cref="CanonicalRequest"/> for one request with the actor's
/// ECC private key and returns the headers to send. Each call draws a fresh nonce, so each signed request is
/// accepted once. It lives in the core protocol assembly, not the ASP.NET Core adapter, so any client can use it.
/// </summary>
public static class RequestProofSigner
{
    /// <summary>Signs a request.</summary>
    /// <param name="signingKey">The actor's ECC private key.</param>
    /// <param name="method">The HTTP method.</param>
    /// <param name="requestUri">
    /// The request URI. Its escaped path and query (<see cref="Uri.AbsolutePath"/> and <see cref="Uri.Query"/>) are
    /// what <c>HttpClient</c> puts on the request line, so that is what is signed. A relative URI is read as a path
    /// and query.
    /// </param>
    /// <param name="body">The exact body bytes that will be sent; empty for a bodiless request.</param>
    /// <param name="now">The signing time.</param>
    /// <param name="algorithm">The signature algorithm; it must be on the host's allow-list.</param>
    /// <returns>The proof headers.</returns>
    public static RequestProofHeaders Sign(IPrivateKeyProvider signingKey, string method, Uri requestUri, byte[] body, DateTimeOffset now, string algorithm = CanonicalRequest.DefaultAlgorithm)
    {
        ArgumentNullException.ThrowIfNull(signingKey);
        ArgumentNullException.ThrowIfNull(requestUri);
        ArgumentNullException.ThrowIfNull(body);
        Uri absolute = requestUri.IsAbsoluteUri ? requestUri : new Uri(new Uri("http://localhost"), requestUri);
        long timestamp = now.ToUnixTimeSeconds();
        string nonce = Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(16));
        string canonical = CanonicalRequest.Compose(method, absolute.AbsolutePath, absolute.Query, timestamp, nonce, CanonicalRequest.BodyDigest(body));
        ISignature signature = new EccSignatureProvider().Sign(signingKey, canonical, algorithm);
        return new RequestProofHeaders(signature.SignatureBase64, algorithm, timestamp.ToString(CultureInfo.InvariantCulture), nonce);
    }
}

/// <summary>The headers that carry a request proof.</summary>
/// <param name="Signature">The value of <c>X-Bam-Body-Signature</c>: the signature over the canonical request.</param>
/// <param name="Algorithm">The value of <c>X-Bam-Body-Signature-Algorithm</c>.</param>
/// <param name="Timestamp">The value of <c>X-Bam-Timestamp</c>.</param>
/// <param name="Nonce">The value of <c>X-Bam-Proof-Nonce</c>.</param>
public sealed record RequestProofHeaders(string Signature, string Algorithm, string Timestamp, string Nonce)
{
    /// <summary>The headers as name and value pairs.</summary>
    /// <returns>Each header name with its value.</returns>
    public IReadOnlyList<KeyValuePair<string, string>> ToHeaders()
    {
        return new[]
        {
            new KeyValuePair<string, string>(Headers.BodySignature, Signature),
            new KeyValuePair<string, string>(Headers.BodySignatureAlgorithm, Algorithm),
            new KeyValuePair<string, string>(CanonicalRequest.TimestampHeader, Timestamp),
            new KeyValuePair<string, string>(CanonicalRequest.NonceHeader, Nonce),
        };
    }

    /// <summary>Adds the headers to an outgoing request.</summary>
    /// <param name="request">The request to send.</param>
    public void ApplyTo(HttpRequestMessage request)
    {
        ArgumentNullException.ThrowIfNull(request);
        foreach (KeyValuePair<string, string> header in ToHeaders())
        {
            request.Headers.Remove(header.Key);
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
    }
}
