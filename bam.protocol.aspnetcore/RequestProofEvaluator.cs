using System.Globalization;
using Bam.Protocol.Profile;
using Bam.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Checks a request's proof of key possession. The <c>X-Bam-Body-Signature</c> must verify, against the actor's
/// registered ECC key, over the <see cref="CanonicalRequest"/> built from this request: its method, the path and
/// query exactly as on the request line, the <c>X-Bam-Timestamp</c> and <c>X-Bam-Proof-Nonce</c> headers, and the
/// SHA-256 of the raw body bytes. The timestamp must be within <see cref="ActorAuthenticationOptions.RequestProofMaxAge"/>
/// of now, and the nonce must not have been used by the same key while the proof could still be accepted. A captured
/// proof therefore works only for the request it was made for, once, and only for a few minutes (bam.protocol#46).
/// <see cref="RequestProofEndpointFilter"/> calls this for minimal-API endpoints; a proxy or MVC host resolves it and
/// calls <see cref="EvaluateAsync"/> from its own pipeline.
/// </summary>
public sealed class RequestProofEvaluator
{
    /// <summary>The longest <see cref="ActorAuthenticationOptions.RequestProofMaxAge"/> accepted.</summary>
    public static readonly TimeSpan MaximumMaxAge = TimeSpan.FromDays(1);

    private readonly IRequestProof _proof;
    private readonly IRequestProofNonceStore _nonces;
    private readonly ActorAuthenticationOptions _options;
    private readonly TimeProvider _clock;
    private readonly ILogger? _logger;

    /// <summary>Creates the evaluator.</summary>
    /// <param name="proof">Verifies signatures.</param>
    /// <param name="nonces">Remembers used nonces.</param>
    /// <param name="options">Carries <see cref="ActorAuthenticationOptions.RequestProofMaxAge"/>.</param>
    /// <param name="clock">The time source; the system clock when null.</param>
    /// <param name="logger">Receives a warning when the nonce store refuses a proof for capacity; optional.</param>
    /// <exception cref="InvalidOperationException">The maximum age is not positive or exceeds <see cref="MaximumMaxAge"/>.</exception>
    public RequestProofEvaluator(IRequestProof proof, IRequestProofNonceStore nonces, ActorAuthenticationOptions options, TimeProvider? clock = null, ILogger<RequestProofEvaluator>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(proof);
        ArgumentNullException.ThrowIfNull(nonces);
        ArgumentNullException.ThrowIfNull(options);
        if (options.RequestProofMaxAge <= TimeSpan.Zero || options.RequestProofMaxAge > MaximumMaxAge)
        {
            throw new InvalidOperationException("ActorAuth:RequestProofMaxAge must be positive and at most one day.");
        }

        _proof = proof;
        _nonces = nonces;
        _options = options;
        _clock = clock ?? TimeProvider.System;
        _logger = logger;
    }

    /// <summary>
    /// Verifies the request's proof. The body is read from the start and rewound, so the request must be buffered
    /// (<see cref="ActorAuthenticationMiddleware"/> enables buffering). The nonce is recorded only once the signature
    /// verifies, so unsigned or forged requests can neither fill the nonce store nor burn a genuine proof's nonce.
    /// </summary>
    /// <param name="request">The request carrying the body and the proof headers.</param>
    /// <param name="eccPublicKeyPem">The actor's registered ECC public key, or null when it has none.</param>
    /// <returns>Success, or a failure with a reason that names no handle or key.</returns>
    public async Task<RequestProofResult> EvaluateAsync(HttpRequest request, string? eccPublicKeyPem)
    {
        ArgumentNullException.ThrowIfNull(request);
        string? signature = request.Headers[Headers.BodySignature].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(signature))
        {
            return RequestProofResult.Failed($"Request proof required: {Headers.BodySignature} header missing.");
        }

        if (string.IsNullOrWhiteSpace(eccPublicKeyPem))
        {
            return RequestProofResult.Failed("Request proof required but the caller has no registered ECC key.");
        }

        string? timestampText = request.Headers[CanonicalRequest.TimestampHeader].FirstOrDefault();
        if (!long.TryParse(timestampText, NumberStyles.None, CultureInfo.InvariantCulture, out long timestamp))
        {
            return RequestProofResult.Failed($"Request proof required: {CanonicalRequest.TimestampHeader} header missing or not Unix seconds.");
        }

        string? nonce = request.Headers[CanonicalRequest.NonceHeader].FirstOrDefault();
        if (!CanonicalRequest.IsWellFormedNonce(nonce))
        {
            return RequestProofResult.Failed($"Request proof required: {CanonicalRequest.NonceHeader} header missing or malformed.");
        }

        DateTimeOffset now = _clock.GetUtcNow();
        DateTimeOffset signedAt;
        try
        {
            signedAt = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        }
        catch (ArgumentOutOfRangeException)
        {
            return RequestProofResult.Failed("Request proof failed: the timestamp is out of range.");
        }

        if (signedAt < now - _options.RequestProofMaxAge || signedAt > now + _options.RequestProofMaxAge)
        {
            return RequestProofResult.Failed("Request proof failed: the timestamp is outside the accepted window.");
        }

        KeyValuePair<string, string> target = CanonicalRequest.SplitTarget(RequestTarget(request));
        string bodyDigest = await DigestBodyAsync(request).ConfigureAwait(false);
        string canonical = CanonicalRequest.Compose(request.Method, target.Key, target.Value, timestamp, nonce!, bodyDigest);
        string? algorithm = request.Headers[Headers.BodySignatureAlgorithm].FirstOrDefault();
        if (!_proof.Verify(canonical, signature, algorithm, eccPublicKeyPem))
        {
            return RequestProofResult.Failed("Request proof failed: the signature does not verify over this request with the registered ECC key.");
        }

        string scope = PublicKeyFingerprint.Of(eccPublicKeyPem) ?? eccPublicKeyPem;
        switch (_nonces.TryClaim(scope, nonce!, signedAt + _options.RequestProofMaxAge, now))
        {
            case NonceClaimResult.Claimed:
                return RequestProofResult.Verified;
            case NonceClaimResult.AlreadyUsed:
                return RequestProofResult.Failed("Request proof failed: the nonce was already used.");
            default:
                _logger?.LogWarning("Request proof refused: the nonce store is at capacity for this signing key or in total; raise ActorAuth:RequestProofNoncesPerKey or RequestProofNonceCapacity if this is legitimate load.");
                return RequestProofResult.Failed("Request proof refused: the host cannot record another proof for this key right now; retry later.");
        }
    }

    /// <summary>
    /// The request target as it appeared on the request line, which is what the client signed. Falls back to the
    /// escaped path base, path and query when the server does not expose the raw target.
    /// </summary>
    private static string RequestTarget(HttpRequest request)
    {
        string? raw = request.HttpContext.Features.Get<IHttpRequestFeature>()?.RawTarget;
        if (!string.IsNullOrEmpty(raw))
        {
            return raw;
        }

        return request.PathBase.Add(request.Path).ToUriComponent() + request.QueryString.Value;
    }

    private static async Task<string> DigestBodyAsync(HttpRequest request)
    {
        if (!request.Body.CanSeek)
        {
            request.EnableBuffering();
        }

        request.Body.Position = 0;
        string digest = await CanonicalRequest.BodyDigestAsync(request.Body).ConfigureAwait(false);
        request.Body.Position = 0;
        return digest;
    }
}
