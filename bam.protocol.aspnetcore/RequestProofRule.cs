using System.Text;
using Bam.Protocol.Server;
using Bam.Web;
using Microsoft.AspNetCore.Http;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// The per-request proof rule, independent of how a host runs it. <see cref="RequestProofEndpointFilter"/>
/// calls it for minimal-API endpoints; a proxy or MVC host calls it directly from its own pipeline. Whether
/// a request needs proof comes from its endpoint's metadata (<see cref="RequiresProof"/>); the proof itself
/// is the <c>X-Bam-Body-Signature</c> over the buffered raw body, verified against the actor's registered
/// ECC key (<see cref="EvaluateAsync"/>).
/// </summary>
public static class RequestProofRule
{
    /// <summary>Decides whether an endpoint requires a body signature.</summary>
    /// <param name="endpoint">The endpoint, or null when routing matched none.</param>
    /// <param name="options">The proof threshold.</param>
    /// <returns>True when the endpoint requires proof.</returns>
    public static bool RequiresProof(Endpoint? endpoint, ActorAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        AnonymousAccessAttribute? anonymous = endpoint?.Metadata.GetMetadata<AnonymousAccessAttribute>();
        if (anonymous is not null && anonymous.AllowAnonymous)
        {
            return false;
        }

        RequireRequestProofAttribute? explicitRule = endpoint?.Metadata.GetMetadata<RequireRequestProofAttribute>();
        if (explicitRule is not null)
        {
            return explicitRule.Required;
        }

        return ActorAccessEndpointFilter.RequiredAccessOf(endpoint) >= options.ProofRequiredAtOrAbove;
    }

    /// <summary>
    /// Verifies the request's body signature. The body is read from the start and the stream is rewound,
    /// so the request must be buffered (<see cref="ActorAuthenticationMiddleware"/> enables buffering).
    /// </summary>
    /// <param name="request">The request carrying the body and the signature headers.</param>
    /// <param name="proof">Verifies body signatures.</param>
    /// <param name="eccPublicKeyPem">The actor's registered ECC public key, or null when it has none.</param>
    /// <returns>Success, or a failure with a reason that names no handle or key.</returns>
    public static async Task<RequestProofResult> EvaluateAsync(HttpRequest request, IRequestProof proof, string? eccPublicKeyPem)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(proof);
        string? signature = request.Headers[Headers.BodySignature].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(signature))
        {
            return RequestProofResult.Failed($"Request proof required: {Headers.BodySignature} header missing.");
        }

        if (string.IsNullOrWhiteSpace(eccPublicKeyPem))
        {
            return RequestProofResult.Failed("Request proof required but the caller has no registered ECC key.");
        }

        string body = await ReadBodyAsync(request).ConfigureAwait(false);
        string? algorithm = request.Headers[Headers.BodySignatureAlgorithm].FirstOrDefault();
        if (!proof.Verify(body, signature, algorithm, eccPublicKeyPem))
        {
            return RequestProofResult.Failed("Request proof failed: body signature does not verify against the registered ECC key.");
        }

        return RequestProofResult.Verified;
    }

    private static async Task<string> ReadBodyAsync(HttpRequest request)
    {
        if (!request.Body.CanSeek)
        {
            request.EnableBuffering();
        }

        request.Body.Position = 0;
        using StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
        string body = await reader.ReadToEndAsync().ConfigureAwait(false);
        request.Body.Position = 0;
        return body;
    }
}

/// <summary>The outcome of <see cref="RequestProofRule.EvaluateAsync"/>.</summary>
public sealed record RequestProofResult
{
    /// <summary>The successful outcome.</summary>
    public static RequestProofResult Verified { get; } = new RequestProofResult { Success = true };

    /// <summary>Gets whether the proof verified.</summary>
    public required bool Success { get; init; }

    /// <summary>Gets the reason the proof failed, or null on success.</summary>
    public string? Reason { get; init; }

    /// <summary>Creates a failed outcome.</summary>
    /// <param name="reason">Why the proof failed.</param>
    /// <returns>The failed outcome.</returns>
    public static RequestProofResult Failed(string reason)
    {
        return new RequestProofResult { Success = false, Reason = reason };
    }
}
