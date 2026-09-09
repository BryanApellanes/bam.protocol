using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Server;
using Bam.UserAccounts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Wires actor authentication for both containers — the bamtk <see cref="ServiceRegistry"/> and
/// Microsoft.Extensions <see cref="IServiceCollection"/> — and inserts the middleware. Prerequisites
/// the HOST must already provide: <see cref="IPublicKeySetResolver"/>, <see cref="IPublicKeySetRegistrar"/>,
/// <see cref="IAccountConfirmation"/> (e.g. <c>AddDeviceKeyAccountConfirmation</c>), and, for hybrid
/// mode, an <see cref="INamedKeyStorage"/> holding the server key. Missing prerequisites fail fast
/// with the missing contract named, mirroring <c>AddBamAi</c>.
/// </summary>
public static class ActorAuthenticationRegistration
{
    /// <summary>
    /// Registers the adapter into a bamtk <see cref="ServiceRegistry"/>.
    /// </summary>
    /// <param name="registry">The host registry.</param>
    /// <param name="options">The options; defaults to <see cref="ActorAuthenticationOptions"/> defaults.</param>
    /// <returns>The registry, for chaining.</returns>
    public static ServiceRegistry AddActorAuthentication(this ServiceRegistry registry, ActorAuthenticationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ActorAuthenticationOptions effective = options ?? new ActorAuthenticationOptions();
        registry.For<ActorAuthenticationOptions>().UseSingleton(effective);
        registry.For<SignedActorTokenVerifier>().UseSingleton<SignedActorTokenVerifier>();
        registry.For<ServerIssuedTokenVerifier>().UseSingleton<ServerIssuedTokenVerifier>();
        registry.For<ServerActorTokenIssuer>().UseSingleton<ServerActorTokenIssuer>();
        if (effective.Mode == ActorAuthenticationMode.Hybrid)
        {
            registry.For<IActorTokenVerifier>().UseSingleton<ServerIssuedTokenVerifier>();
        }
        else
        {
            registry.For<IActorTokenVerifier>().UseSingleton<SignedActorTokenVerifier>();
        }

        registry.For<IRequestProof>().UseSingleton<BodySignatureProofVerifier>();
        registry.For<IActorAccessPolicy>().UseSingleton<ConfiguredActorAccessPolicy>();
        if (!registry.Contains<IAnonymousActorProvider>())
        {
            registry.For<IAnonymousActorProvider>().UseSingleton<AnonymousActorProvider>();
        }

        return registry;
    }

    /// <summary>
    /// Registers the adapter into an <see cref="IServiceCollection"/>, binding options from
    /// <paramref name="configuration"/>'s <paramref name="sectionName"/> section.
    /// </summary>
    /// <param name="services">The host's service collection.</param>
    /// <param name="configuration">Configuration root holding the section.</param>
    /// <param name="sectionName">The section bound to <see cref="ActorAuthenticationOptions"/>; defaults to <c>ActorAuth</c>.</param>
    /// <returns>The service collection, for chaining.</returns>
    public static IServiceCollection AddActorAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = ActorAuthenticationOptions.DefaultSectionName)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ActorAuthenticationOptions options = configuration.GetSection(sectionName).Get<ActorAuthenticationOptions>() ?? new ActorAuthenticationOptions();
        return services.AddActorAuthentication(options);
    }

    /// <summary>
    /// Registers the adapter into an <see cref="IServiceCollection"/> with explicit options.
    /// </summary>
    /// <param name="services">The host's service collection.</param>
    /// <param name="options">The options.</param>
    /// <returns>The service collection, for chaining.</returns>
    public static IServiceCollection AddActorAuthentication(this IServiceCollection services, ActorAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);
        services.AddSingleton(options);
        services.AddSingleton<SignedActorTokenVerifier>();
        services.AddSingleton<ServerIssuedTokenVerifier>();
        services.AddSingleton<ServerActorTokenIssuer>();
        services.AddSingleton<IActorTokenVerifier>(provider => options.Mode == ActorAuthenticationMode.Hybrid
            ? provider.GetRequiredService<ServerIssuedTokenVerifier>()
            : provider.GetRequiredService<SignedActorTokenVerifier>());
        services.AddSingleton<IRequestProof, BodySignatureProofVerifier>();
        services.AddSingleton<IActorAccessPolicy, ConfiguredActorAccessPolicy>();
        services.TryAddSingleton<IAnonymousActorProvider, AnonymousActorProvider>();
        return services;
    }

    /// <summary>
    /// Inserts <see cref="ActorAuthenticationMiddleware"/> when <see cref="ActorAuthenticationOptions.Enabled"/>
    /// is true, after verifying the host provides every prerequisite. When disabled, the pipeline is
    /// left exactly as it was.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder, for chaining.</returns>
    /// <exception cref="InvalidOperationException">A prerequisite contract or the server key is missing.</exception>
    public static IApplicationBuilder UseActorAuthentication(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        ActorAuthenticationOptions options = app.ApplicationServices.GetService<ActorAuthenticationOptions>()
            ?? throw new InvalidOperationException("Call AddActorAuthentication before UseActorAuthentication.");
        if (!options.Enabled)
        {
            return app;
        }

        VerifyPrerequisites(app.ApplicationServices, options);
        return app.UseMiddleware<ActorAuthenticationMiddleware>();
    }

    /// <summary>
    /// Marks a route as a protected endpoint: records <see cref="RequiredAccessAttribute"/> metadata and
    /// attaches the request-proof and access filters (proof first, then access).
    /// </summary>
    /// <param name="builder">The route handler.</param>
    /// <param name="access">The access the endpoint requires.</param>
    /// <returns>The route handler, for chaining.</returns>
    public static RouteHandlerBuilder RequireActorAccess(this RouteHandlerBuilder builder, BamAccess access)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder
            .WithMetadata(new RequiredAccessAttribute(access))
            .AddEndpointFilter<RequestProofEndpointFilter>()
            .AddEndpointFilter<ActorAccessEndpointFilter>();
    }

    /// <summary>
    /// Provisions the server signing key when none is stored under
    /// <see cref="ActorAuthenticationOptions.ServerKeyName"/>: generates an ECC key pair and saves
    /// its PEM bytes. Idempotent — an existing key is never replaced (rotate deliberately by saving a
    /// new key under the name). Hosts call this at startup, before <see cref="UseActorAuthentication"/>,
    /// so the key is born on the host and never leaves it.
    /// </summary>
    /// <param name="keys">Named key storage.</param>
    /// <param name="options">Carries the key name.</param>
    /// <returns>True when a new key was generated; false when one already existed.</returns>
    public static bool EnsureServerKey(INamedKeyStorage keys, ActorAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(keys);
        ArgumentNullException.ThrowIfNull(options);
        byte[]? existing = keys.GetNamedKey(options.ServerKeyName);
        if (existing is not null && existing.Length > 0)
        {
            return false;
        }

        using EccKeyPair generated = new EccKeyPair();
        Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair pair = new Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair(generated.PublicKey.Value, generated.PrivateKey.Value);
        if (!keys.SaveNamedKey(options.ServerKeyName, pair.ToPem(System.Text.Encoding.UTF8)))
        {
            throw new InvalidOperationException($"Named key storage refused to save the server signing key under '{options.ServerKeyName}'.");
        }

        return true;
    }

    /// <summary>
    /// Fails fast when the host did not provide a prerequisite contract, or when hybrid mode has no
    /// server key in storage.
    /// </summary>
    /// <param name="services">The host's service provider.</param>
    /// <param name="options">The options in force.</param>
    /// <exception cref="InvalidOperationException">A prerequisite is missing.</exception>
    public static void VerifyPrerequisites(IServiceProvider services, ActorAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);
        List<string> missing = new List<string>();
        if (services.GetService<IPublicKeySetResolver>() is null)
        {
            missing.Add(nameof(IPublicKeySetResolver));
        }

        if (services.GetService<IPublicKeySetRegistrar>() is null)
        {
            missing.Add(nameof(IPublicKeySetRegistrar));
        }

        if (services.GetService<IAccountConfirmation>() is null)
        {
            missing.Add(nameof(IAccountConfirmation));
        }

        INamedKeyStorage? keys = services.GetService<INamedKeyStorage>();
        if (options.Mode == ActorAuthenticationMode.Hybrid && keys is null)
        {
            missing.Add(nameof(INamedKeyStorage));
        }

        if (missing.Count > 0)
        {
            throw new InvalidOperationException(
                $"Actor authentication requires the host to register: {string.Join(", ", missing)}. Register the key-set store (IPublicKeySetResolver/IPublicKeySetRegistrar), the confirmation flow (AddDeviceKeyAccountConfirmation), and named key storage before UseActorAuthentication.");
        }

        if (options.Mode == ActorAuthenticationMode.Hybrid)
        {
            byte[]? serverKey = keys!.GetNamedKey(options.ServerKeyName);
            if (serverKey is null || serverKey.Length == 0)
            {
                throw new InvalidOperationException(
                    $"Hybrid actor authentication needs a server signing key stored under '{options.ServerKeyName}' (ActorAuthenticationOptions.ServerKeyName); provision an ECC key pair PEM in INamedKeyStorage, or select ActorAuthenticationMode.ClientSignedOnly.");
            }
        }
    }
}
