using Bam.Protocol.Server;
using Bam.Web;
using Microsoft.AspNetCore.Http;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Gates every request: endpoints marked <see cref="AnonymousAccessAttribute"/> pass through carrying
/// the anonymous sentinel; every other endpoint requires a bearer token that
/// <see cref="IActorTokenVerifier"/> accepts, otherwise the request ends with 401 and the failure
/// reasons. On success the actor and its registered ECC key PEM are recorded on the context
/// (<see cref="ActorHttpContext"/>) for the endpoint filters. Request-body buffering is enabled so
/// <see cref="RequestProofEndpointFilter"/> can re-read the raw body after model binding.
/// </summary>
public sealed class ActorAuthenticationMiddleware
{
    private const string BearerScheme = "Bearer ";

    private readonly RequestDelegate _next;

    /// <summary>Creates the middleware.</summary>
    /// <param name="next">The next delegate in the pipeline.</param>
    public ActorAuthenticationMiddleware(RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(next);
        _next = next;
    }

    /// <summary>
    /// Authenticates the request, then continues the pipeline or ends it with 401.
    /// </summary>
    /// <param name="context">The request context.</param>
    /// <param name="verifier">The token verifier bound for the host's mode.</param>
    /// <param name="anonymousActors">Supplies the anonymous sentinel.</param>
    /// <returns>A task that completes when the pipeline has run.</returns>
    public async Task InvokeAsync(HttpContext context, IActorTokenVerifier verifier, IAnonymousActorProvider anonymousActors)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(verifier);
        ArgumentNullException.ThrowIfNull(anonymousActors);

        context.Request.EnableBuffering();

        AnonymousAccessAttribute? anonymous = context.GetEndpoint()?.Metadata.GetMetadata<AnonymousAccessAttribute>();
        if (anonymous is not null && anonymous.AllowAnonymous)
        {
            context.SetActor(anonymousActors.GetAnonymousActor(), null);
            await _next(context).ConfigureAwait(false);
            return;
        }

        string? authorization = context.Request.Headers[Headers.Authorization].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(authorization))
        {
            await RejectAsync(context, "Authorization header missing.").ConfigureAwait(false);
            return;
        }

        if (!authorization.StartsWith(BearerScheme, StringComparison.OrdinalIgnoreCase))
        {
            await RejectAsync(context, "Authorization header must use the Bearer scheme.").ConfigureAwait(false);
            return;
        }

        ActorTokenVerification verification = verifier.Verify(authorization.Substring(BearerScheme.Length).Trim());
        if (!verification.Success || verification.Actor is null)
        {
            await RejectAsync(context, string.Join(" ", verification.Messages)).ConfigureAwait(false);
            return;
        }

        context.SetActor(verification.Actor, verification.EccPublicKeyPem);
        await _next(context).ConfigureAwait(false);
    }

    private static async Task RejectAsync(HttpContext context, string reason)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync(reason).ConfigureAwait(false);
    }
}
