using System.Text;
using Bam.Web;
using Microsoft.AspNetCore.Http;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Enforces the per-request proof of key possession. Whether an endpoint requires it is derived from
/// its access ladder — <see cref="RequiredAccessAttribute"/> at or above
/// <see cref="ActorAuthenticationOptions.ProofRequiredAtOrAbove"/> — unless
/// <see cref="RequireRequestProofAttribute"/> forces or waives it; anonymous-marked endpoints never
/// require it. When required, the raw body is re-read (the middleware enabled buffering) and the
/// <c>X-Bam-Body-Signature</c> header must verify against the actor's registered ECC key.
/// </summary>
public sealed class RequestProofEndpointFilter : IEndpointFilter
{
    private readonly IRequestProof _proof;
    private readonly ActorAuthenticationOptions _options;

    /// <summary>Creates the filter.</summary>
    /// <param name="proof">Verifies body signatures.</param>
    /// <param name="options">Carries the proof threshold.</param>
    public RequestProofEndpointFilter(IRequestProof proof, ActorAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(proof);
        ArgumentNullException.ThrowIfNull(options);
        _proof = proof;
        _options = options;
    }

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

    /// <inheritdoc />
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        HttpContext http = context.HttpContext;
        if (!RequiresProof(http.GetEndpoint(), _options))
        {
            return await next(context).ConfigureAwait(false);
        }

        string? signature = http.Request.Headers[Headers.BodySignature].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(signature))
        {
            return Unauthorized($"Request proof required: {Headers.BodySignature} header missing.");
        }

        string? eccPublicKeyPem = http.GetActorEccPublicKeyPem();
        if (string.IsNullOrWhiteSpace(eccPublicKeyPem))
        {
            return Unauthorized("Request proof required but the caller has no registered ECC key.");
        }

        string body = await ReadBodyAsync(http.Request).ConfigureAwait(false);
        string? algorithm = http.Request.Headers[Headers.BodySignatureAlgorithm].FirstOrDefault();
        if (!_proof.Verify(body, signature, algorithm, eccPublicKeyPem))
        {
            return Unauthorized("Request proof failed: body signature does not verify against the registered ECC key.");
        }

        return await next(context).ConfigureAwait(false);
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

    private static IResult Unauthorized(string reason)
    {
        return Results.Problem(detail: reason, statusCode: StatusCodes.Status401Unauthorized);
    }
}
