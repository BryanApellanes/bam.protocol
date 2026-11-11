using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Turns a refused <see cref="RequestProofResult"/> into the HTTP answer, in one place, so every caller of
/// <see cref="RequestProofEvaluator"/> in this library answers the same way: 401 for a proof that is itself
/// unacceptable; 503 with <c>Retry-After</c> when the host could not check it (a nonce-store outage) or could not
/// record it (the store is full); and 429 with <c>Retry-After</c> when one signing key is over its own nonce
/// quota. Without the distinction a client reads an outage as "my key is bad" and a 401-rate alert reads it as an
/// attack, and answering one key's quota with a 5xx would make a healthy host look down (bam.protocol#58). A host
/// with its own error format calls
/// <see cref="StatusCodeOf"/> or reads <see cref="RequestProofResult.Failure"/> directly.
/// </summary>
public static class RequestProofRefusal
{
    /// <summary>The status code a refused proof is answered with.</summary>
    /// <param name="result">The evaluator's outcome; must be a failure.</param>
    /// <returns>
    /// 429 for <see cref="RequestProofFailure.KeyQuota"/>, 503 for <see cref="RequestProofFailure.Unavailable"/> and
    /// <see cref="RequestProofFailure.Capacity"/>, otherwise 401.
    /// </returns>
    /// <exception cref="ArgumentException"><paramref name="result"/> is a success.</exception>
    public static int StatusCodeOf(RequestProofResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.Success)
        {
            throw new ArgumentException("A verified proof has no refusal status.", nameof(result));
        }

        switch (result.Failure)
        {
            case RequestProofFailure.KeyQuota:
                return StatusCodes.Status429TooManyRequests;
            case RequestProofFailure.Unavailable:
            case RequestProofFailure.Capacity:
                return StatusCodes.Status503ServiceUnavailable;
            default:
                return StatusCodes.Status401Unauthorized;
        }
    }

    /// <summary>
    /// Builds the problem response for a refused proof. For a retryable refusal (429 or 503) it also sets
    /// <c>Retry-After</c> on <paramref name="http"/>'s response, in whole seconds from
    /// <see cref="ActorAuthenticationOptions.RequestProofRetryAfter"/> (at least one).
    /// </summary>
    /// <remarks>
    /// The header is set when this method is called, not when the returned result is written. Call it before the
    /// response has started (setting a header afterwards throws <see cref="InvalidOperationException"/>), and return
    /// the result: if it is discarded, the header stays on whatever response is sent instead.
    /// </remarks>
    /// <param name="http">The request being refused; its response receives the <c>Retry-After</c> header.</param>
    /// <param name="result">The evaluator's outcome; must be a failure.</param>
    /// <param name="options">Carries <see cref="ActorAuthenticationOptions.RequestProofRetryAfter"/>.</param>
    /// <returns>A problem result carrying the reason and <see cref="StatusCodeOf"/>.</returns>
    public static IResult ToProblem(HttpContext http, RequestProofResult result, ActorAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(http);
        ArgumentNullException.ThrowIfNull(options);
        int status = StatusCodeOf(result);
        if (result.IsRetryable)
        {
            long seconds = Math.Max(1L, (long)Math.Ceiling(options.RequestProofRetryAfter.TotalSeconds));
            http.Response.Headers.RetryAfter = seconds.ToString(CultureInfo.InvariantCulture);
        }

        return Results.Problem(detail: result.Reason ?? "Request proof failed.", statusCode: status);
    }
}
