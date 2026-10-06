using Bam.Web;
using Microsoft.AspNetCore.Http;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Enforces the per-request proof of key possession. Whether an endpoint requires it is derived from
/// its access ladder — <see cref="RequiredAccessAttribute"/> at or above
/// <see cref="ActorAuthenticationOptions.ProofRequiredAtOrAbove"/> — unless
/// <see cref="RequireRequestProofAttribute"/> forces or waives it; anonymous-marked endpoints never
/// require it. When required, <see cref="RequestProofRule.EvaluateAsync"/> re-reads the raw body (the
/// middleware enabled buffering) and the <c>X-Bam-Body-Signature</c> header must verify against the
/// actor's registered ECC key.
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

        RequestProofResult result = await RequestProofRule.EvaluateAsync(http.Request, _proof, http.GetActorEccPublicKeyPem()).ConfigureAwait(false);
        if (!result.Success)
        {
            return Unauthorized(result.Reason ?? "Request proof failed.");
        }

        return await next(context).ConfigureAwait(false);
    }

    private static IResult Unauthorized(string reason)
    {
        return Results.Problem(detail: reason, statusCode: StatusCodes.Status401Unauthorized);
    }
}
