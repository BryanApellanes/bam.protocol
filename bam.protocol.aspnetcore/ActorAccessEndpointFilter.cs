using Microsoft.AspNetCore.Http;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Enforces <see cref="RequiredAccessAttribute"/> endpoint metadata with the same semantics as the
/// Bam-native <c>AuthorizationCalculator</c>: an anonymous-marked endpoint is always allowed, otherwise
/// the actor's access from <see cref="IActorAccessPolicy"/> must be at or above the required level.
/// An endpoint that carries this filter (through <c>RequireActorAccess</c>) but no
/// <see cref="RequiredAccessAttribute"/> requires <see cref="BamAccess.Execute"/>. Endpoints mapped
/// without the filter get no access check at all, only the middleware's authentication.
/// </summary>
public sealed class ActorAccessEndpointFilter : IEndpointFilter
{
    /// <summary>The access assumed for an endpoint that carries this filter but declares no <see cref="RequiredAccessAttribute"/>.</summary>
    public const BamAccess DefaultRequiredAccess = BamAccess.Execute;

    private readonly IActorAccessPolicy _policy;

    /// <summary>Creates the filter.</summary>
    /// <param name="policy">Maps actors to access levels.</param>
    public ActorAccessEndpointFilter(IActorAccessPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(policy);
        _policy = policy;
    }

    /// <summary>Resolves the access an endpoint requires from its metadata.</summary>
    /// <param name="endpoint">The endpoint, or null when routing matched none.</param>
    /// <returns>The required access.</returns>
    public static BamAccess RequiredAccessOf(Endpoint? endpoint)
    {
        RequiredAccessAttribute? required = endpoint?.Metadata.GetMetadata<RequiredAccessAttribute>();
        return required?.Access ?? DefaultRequiredAccess;
    }

    /// <inheritdoc />
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        Endpoint? endpoint = context.HttpContext.GetEndpoint();
        AnonymousAccessAttribute? anonymous = endpoint?.Metadata.GetMetadata<AnonymousAccessAttribute>();
        if (anonymous is not null && anonymous.AllowAnonymous)
        {
            return await next(context).ConfigureAwait(false);
        }

        IActor? actor = context.HttpContext.GetActor();
        if (actor is null)
        {
            return Results.StatusCode(StatusCodes.Status401Unauthorized);
        }

        BamAccess required = RequiredAccessOf(endpoint);
        BamAccess held = _policy.GetAccess(actor);
        if (held >= required)
        {
            return await next(context).ConfigureAwait(false);
        }

        return Results.Problem(
            detail: $"Actor '{actor.Handle}' has {held} access but the endpoint requires {required}.",
            statusCode: StatusCodes.Status403Forbidden);
    }
}
