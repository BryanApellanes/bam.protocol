using Bam.Protocol.Server;
using Microsoft.AspNetCore.Http;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Decides whether a request needs a proof of key possession, from its endpoint's metadata
/// (<see cref="RequiresProof"/>). The proof itself is checked by <see cref="RequestProofEvaluator"/>.
/// <see cref="RequestProofEndpointFilter"/> uses both for minimal-API endpoints; a proxy or MVC host calls
/// them from its own pipeline.
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
}

/// <summary>The outcome of <see cref="RequestProofEvaluator.EvaluateAsync"/>.</summary>
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
