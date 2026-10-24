using Bam.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Registers and maps the <c>/invoke</c> bridge that runs <see cref="MethodInvocationRequest"/>s behind actor
/// authentication.
/// </summary>
/// <remarks>
/// Host wiring, after <see cref="ActorAuthenticationRegistration.AddActorAuthentication(IServiceCollection, ActorAuthenticationOptions)"/>:
/// <code>
/// builder.Services.AddBamInvocation(webServiceRegistry);   // the registry whose types may be invoked
/// app.UseActorAuthentication();
/// app.MapBamInvocation();                                   // POST /invoke
/// </code>
/// The endpoint is never anonymous: the middleware demands a verified, admitted actor for every request to it, and
/// <see cref="BamInvocationHandler"/> applies the target method's own access and proof rules on top.
/// </remarks>
public static class BamInvocationRegistration
{
    /// <summary>
    /// Registers the bridge's services and names the <see cref="ServiceRegistry"/> whose types may be invoked.
    /// </summary>
    /// <param name="services">The host's service collection.</param>
    /// <param name="dispatchRegistry">The registry holding the invocable types; pass a <c>WebServiceRegistry</c> to enforce <c>[WebService]</c>.</param>
    /// <param name="options">Endpoint options, or null for the defaults.</param>
    /// <returns>The service collection, for chaining.</returns>
    /// <remarks>
    /// Also registers an <see cref="AmbientActorContext"/> as <see cref="IActorContext"/> in
    /// <paramref name="dispatchRegistry"/> unless the host already registered one, so invoked services can take the
    /// verified caller by constructor.
    /// </remarks>
    public static IServiceCollection AddBamInvocation(this IServiceCollection services, ServiceRegistry dispatchRegistry, BamInvocationOptions? options = null)
    {
        Args.ThrowIfNull(services, nameof(services));
        Args.ThrowIfNull(dispatchRegistry, nameof(dispatchRegistry));

        BamInvocationOptions effective = options ?? new BamInvocationOptions();
        AmbientActorContext ambient = new AmbientActorContext();
        if (!dispatchRegistry.Contains<IActorContext>())
        {
            dispatchRegistry.For<IActorContext>().Use(ambient);
        }

        services.AddSingleton(effective);
        services.AddSingleton(new BamInvocationDispatchRegistry(dispatchRegistry));
        services.TryAddSingleton<IInvocationTargetResolver, RegisteredTypeInvocationTargetResolver>();
        services.TryAddSingleton<IActorContext>(ambient);
        services.AddSingleton<BamInvocationHandler>();
        return services;
    }

    /// <summary>
    /// Maps <c>POST {pattern}</c> to the invocation bridge.
    /// </summary>
    /// <param name="routes">The route builder.</param>
    /// <param name="pattern">The route pattern, or null for <see cref="BamInvocationOptions.Pattern"/>.</param>
    /// <returns>The route handler, for chaining.</returns>
    /// <exception cref="InvalidOperationException">
    /// <see cref="AddBamInvocation"/> or <c>AddActorAuthentication</c> was not called, so the bridge would run
    /// without authentication.
    /// </exception>
    public static RouteHandlerBuilder MapBamInvocation(this IEndpointRouteBuilder routes, string? pattern = null)
    {
        Args.ThrowIfNull(routes, nameof(routes));

        BamInvocationOptions? options = routes.ServiceProvider.GetService<BamInvocationOptions>();
        if (options is null || routes.ServiceProvider.GetService<BamInvocationDispatchRegistry>() is null)
        {
            throw new InvalidOperationException("MapBamInvocation requires AddBamInvocation(dispatchRegistry) on the service collection.");
        }

        if (routes.ServiceProvider.GetService<ActorAuthenticationOptions>() is null)
        {
            throw new InvalidOperationException("MapBamInvocation requires AddActorAuthentication: the invocation endpoint is never served without actor authentication.");
        }

        // Resolve the handler only once both prerequisites are known present, so a missing registration surfaces
        // as one of the messages above rather than as a container resolution error.
        _ = routes.ServiceProvider.GetRequiredService<BamInvocationHandler>();

        return routes
            .MapPost(pattern ?? options.Pattern, (HttpContext http, BamInvocationHandler handler) => handler.HandleAsync(http))
            .WithMetadata(new AnonymousAccessAttribute(false));
    }
}
