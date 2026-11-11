using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Turns a refused <see cref="RequestProofResult"/> into the HTTP answer, in one place, so every caller of
/// <see cref="RequestProofEvaluator"/> in this library answers the same way: 401 for a proof that is itself
/// unacceptable, and 503 with <c>Retry-After</c> when the host could not check it (a nonce-store outage) or
/// could not record it (capacity). Without the distinction a client reads an outage as "my key is bad" and a
/// 401-rate alert reads it as an attack (bam.protocol#58). A host with its own error format calls
/// <see cref="StatusCodeOf"/> or reads <see cref="RequestProofResult.Failure"/> directly.
/// </summary>
public static class RequestProofRefusal
{
    /// <summary>The status code a refused proof is answered with.</summary>
    /// <param name="result">The evaluator's outcome; must be a failure.</param>
    /// <returns>503 when <see cref="RequestProofResult.IsRetryable"/>, otherwise 401.</returns>
    /// <exception cref="ArgumentException"><paramref name="result"/> is a success.</exception>
    public static int StatusCodeOf(RequestProofResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.Success)
        {
            throw new ArgumentException("A verified proof has no refusal status.", nameof(result));
        }

        return result.IsRetryable ? StatusCodes.Status503ServiceUnavailable : StatusCodes.Status401Unauthorized;
    }

    /// <summary>
    /// Builds the problem response for a refused proof. For a retryable refusal it also sets <c>Retry-After</c> on
    /// <paramref name="http"/>'s response, in whole seconds from
    /// <see cref="ActorAuthenticationOptions.RequestProofRetryAfter"/> (at least one).
    /// </summary>
    /// <param name="http">The request being refused; its response receives the <c>Retry-After</c> header.</param>
    /// <param name="result">The evaluator's outcome; must be a failure.</param>
    /// <param name="options">Carries <see cref="ActorAuthenticationOptions.RequestProofRetryAfter"/>.</param>
    /// <returns>A problem result carrying the reason and <see cref="StatusCodeOf"/>.</returns>
    public static IResult ToProblem(HttpContext http, RequestProofResult result, ActorAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(http);
        ArgumentNullException.ThrowIfNull(options);
        int status = StatusCodeOf(result);
        if (status == StatusCodes.Status503ServiceUnavailable)
        {
            long seconds = Math.Max(1L, (long)Math.Ceiling(options.RequestProofRetryAfter.TotalSeconds));
            http.Response.Headers.RetryAfter = seconds.ToString(CultureInfo.InvariantCulture);
        }

        return Results.Problem(detail: result.Reason ?? "Request proof failed.", statusCode: status);
    }
}
