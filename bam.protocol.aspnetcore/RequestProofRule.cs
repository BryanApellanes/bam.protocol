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

    /// <summary>
    /// Gets what kind of failure this is, so a host can answer an outage or a capacity refusal differently from a
    /// bad proof without reading <see cref="Reason"/>. <see cref="RequestProofFailure.None"/> on success.
    /// </summary>
    public RequestProofFailure Failure { get; init; }

    /// <summary>
    /// Gets whether the refusal is the host's condition, not the caller's proof: the same request may succeed if
    /// sent again later. True for <see cref="RequestProofFailure.Unavailable"/> and <see cref="RequestProofFailure.Capacity"/>.
    /// </summary>
    public bool IsRetryable => Failure == RequestProofFailure.Unavailable || Failure == RequestProofFailure.Capacity;

    /// <summary>Creates a failed outcome for a proof that is missing, malformed, forged, stale or replayed.</summary>
    /// <param name="reason">Why the proof failed.</param>
    /// <returns>The failed outcome, of kind <see cref="RequestProofFailure.Invalid"/>.</returns>
    public static RequestProofResult Failed(string reason)
    {
        return Failed(reason, RequestProofFailure.Invalid);
    }

    /// <summary>Creates a failed outcome of a given kind.</summary>
    /// <param name="reason">Why the proof failed.</param>
    /// <param name="failure">The kind of failure; never <see cref="RequestProofFailure.None"/>.</param>
    /// <returns>The failed outcome.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="failure"/> is <see cref="RequestProofFailure.None"/>.</exception>
    public static RequestProofResult Failed(string reason, RequestProofFailure failure)
    {
        if (failure == RequestProofFailure.None)
        {
            throw new ArgumentOutOfRangeException(nameof(failure), "A failed outcome needs a failure kind.");
        }

        return new RequestProofResult { Success = false, Reason = reason, Failure = failure };
    }
}

/// <summary>Why <see cref="RequestProofEvaluator.EvaluateAsync"/> refused a proof (bam.protocol#58).</summary>
public enum RequestProofFailure
{
    /// <summary>The proof verified.</summary>
    None = 0,

    /// <summary>
    /// The proof itself is unacceptable: missing, malformed, signed by another key, outside the time window, or a
    /// replay. An authentication failure; a host answers 401.
    /// </summary>
    Invalid = 1,

    /// <summary>
    /// The nonce store could not be reached, so the host cannot tell whether the nonce is new and fails closed. An
    /// outage of the host, not a judgement on the proof; a host answers 503.
    /// </summary>
    Unavailable = 2,

    /// <summary>
    /// The nonce store is at its total or per-key limit and cannot record another nonce. Load, not an attack on this
    /// request; a host answers 503.
    /// </summary>
    Capacity = 3,
}
