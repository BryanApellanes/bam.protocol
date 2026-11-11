using Bam.Web;
using Microsoft.AspNetCore.Http;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Enforces the per-request proof of key possession. Whether an endpoint requires it is derived from
/// its access ladder — <see cref="RequiredAccessAttribute"/> at or above
/// <see cref="ActorAuthenticationOptions.ProofRequiredAtOrAbove"/> — unless
/// <see cref="RequireRequestProofAttribute"/> forces or waives it; anonymous-marked endpoints never
/// require it. When required, <see cref="RequestProofEvaluator"/> checks the request's signed canonical
/// form (method, path and query, timestamp, single-use nonce, raw body digest) against the actor's
/// registered ECC key. A proof that fails is answered 401; a proof the host could not check or record (a
/// nonce-store outage, or the store at capacity) is answered 503 with <c>Retry-After</c>
/// (<see cref="RequestProofRefusal"/>).
/// </summary>
public sealed class RequestProofEndpointFilter : IEndpointFilter
{
    private readonly RequestProofEvaluator _evaluator;
    private readonly ActorAuthenticationOptions _options;

    /// <summary>Creates the filter.</summary>
    /// <param name="evaluator">Checks request proofs.</param>
    /// <param name="options">Carries the proof threshold.</param>
    public RequestProofEndpointFilter(RequestProofEvaluator evaluator, ActorAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(evaluator);
        ArgumentNullException.ThrowIfNull(options);
        _evaluator = evaluator;
        _options = options;
    }

    /// <summary>Decides whether an endpoint requires a body signature; forwards to <see cref="RequestProofRule.RequiresProof"/>.</summary>
    /// <param name="endpoint">The endpoint, or null when routing matched none.</param>
    /// <param name="options">The proof threshold.</param>
    /// <returns>True when the endpoint requires proof.</returns>
    public static bool RequiresProof(Endpoint? endpoint, ActorAuthenticationOptions options)
    {
        return RequestProofRule.RequiresProof(endpoint, options);
    }

    /// <inheritdoc />
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        HttpContext http = context.HttpContext;
        if (!RequestProofRule.RequiresProof(http.GetEndpoint(), _options))
        {
            return await next(context).ConfigureAwait(false);
        }

        RequestProofResult result = await _evaluator.EvaluateAsync(http.Request, http.GetActorEccPublicKeyPem()).ConfigureAwait(false);
        if (!result.Success)
        {
            // 401 for a bad proof; 503 with Retry-After when the nonce store is down or full (bam.protocol#58).
            return RequestProofRefusal.ToProblem(http, result, _options);
        }

        return await next(context).ConfigureAwait(false);
    }
}
