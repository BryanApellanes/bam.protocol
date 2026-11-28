using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Profile;
using Microsoft.Extensions.DependencyInjection;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Wires the in-memory, bounded stores actor authentication needs (bam.protocol#119): one
/// <see cref="BoundedMemoryKeySetStore"/> as both <see cref="IPublicKeySetResolver"/> and
/// <see cref="IPublicKeySetRegistrar"/>, and a <see cref="MemoryNamedKeyStorage"/> holding a server signing key
/// generated now under <see cref="ActorAuthenticationOptions.ServerKeyName"/>.
/// <para>
/// For single-process hosts whose actors live for one visit, such as a public demo. Nothing persists: a
/// restart forgets every key set and generates a new server key, so tokens issued before it stop verifying.
/// The host still registers an <see cref="Bam.UserAccounts.IAccountConfirmation"/> (for example
/// <c>AddDeviceKeyAccountConfirmation</c>), which <c>UseActorAuthentication</c> requires.
/// </para>
/// </summary>
public static class InMemoryActorStoreRegistration
{
    /// <summary>
    /// Registers the in-memory stores into an <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The host's service collection.</param>
    /// <param name="actorOptions">The actor-auth options, for the server key name.</param>
    /// <param name="storeOptions">The key-set store's bounds; defaults when null.</param>
    /// <param name="timeProvider">The clock for key-set lifetimes; <see cref="TimeProvider.System"/> when null.</param>
    /// <returns>The service collection, for chaining.</returns>
    public static IServiceCollection AddInMemoryActorStores(this IServiceCollection services, ActorAuthenticationOptions actorOptions, BoundedMemoryKeySetStoreOptions? storeOptions = null, TimeProvider? timeProvider = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(actorOptions);
        BoundedMemoryKeySetStore store = new BoundedMemoryKeySetStore(storeOptions, timeProvider);
        MemoryNamedKeyStorage keys = CreateKeyStorage(actorOptions);
        services.AddSingleton(store);
        services.AddSingleton<IPublicKeySetResolver>(store);
        services.AddSingleton<IPublicKeySetRegistrar>(store);
        services.AddSingleton<INamedKeyStorage>(keys);
        return services;
    }

    /// <summary>
    /// Registers the in-memory stores into a <see cref="ServiceRegistry"/>.
    /// </summary>
    /// <param name="registry">The host registry.</param>
    /// <param name="actorOptions">The actor-auth options, for the server key name.</param>
    /// <param name="storeOptions">The key-set store's bounds; defaults when null.</param>
    /// <param name="timeProvider">The clock for key-set lifetimes; <see cref="TimeProvider.System"/> when null.</param>
    /// <returns>The registry, for chaining.</returns>
    public static ServiceRegistry AddInMemoryActorStores(this ServiceRegistry registry, ActorAuthenticationOptions actorOptions, BoundedMemoryKeySetStoreOptions? storeOptions = null, TimeProvider? timeProvider = null)
    {
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(actorOptions);
        BoundedMemoryKeySetStore store = new BoundedMemoryKeySetStore(storeOptions, timeProvider);
        MemoryNamedKeyStorage keys = CreateKeyStorage(actorOptions);
        registry.For<BoundedMemoryKeySetStore>().UseSingleton(store);
        registry.For<IPublicKeySetResolver>().UseSingleton<IPublicKeySetResolver>(store);
        registry.For<IPublicKeySetRegistrar>().UseSingleton<IPublicKeySetRegistrar>(store);
        registry.For<INamedKeyStorage>().UseSingleton<INamedKeyStorage>(keys);
        return registry;
    }

    private static MemoryNamedKeyStorage CreateKeyStorage(ActorAuthenticationOptions actorOptions)
    {
        MemoryNamedKeyStorage keys = new MemoryNamedKeyStorage();
        ActorAuthenticationRegistration.EnsureServerKey(keys, actorOptions);
        return keys;
    }
}
