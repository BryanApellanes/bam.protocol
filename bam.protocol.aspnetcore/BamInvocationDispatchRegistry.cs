using Bam.DependencyInjection;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// The <see cref="ServiceRegistry"/> whose types the <c>/invoke</c> bridge may dispatch to, with the allow-list of
/// invocable types frozen when the bridge is registered.
/// </summary>
/// <remarks>
/// A host hands its dispatch registry to
/// <see cref="BamInvocationRegistration.AddBamInvocation(Microsoft.Extensions.DependencyInjection.IServiceCollection, ServiceRegistry, BamInvocationOptions?)"/>;
/// this wrapper gives it a distinct identity in the ASP.NET container so it never collides with any other
/// <see cref="ServiceRegistry"/> the host registers. <see cref="InvocableTypes"/> is the complete allow-list, taken
/// once at construction: either the explicit list the host passed or every key the registry held at that moment.
/// It is a snapshot on purpose. <see cref="Bam.DependencyInjection.DependencyProvider.MappedTypes"/> is a live view
/// that grows with every construction (the implementation class behind an interface key, an unregistered concrete
/// constructor dependency), so reading it per request would admit types the host never registered as soon as the
/// first call constructed them. Register every invocable type before <c>AddBamInvocation</c>.
/// <see cref="Bam.DependencyInjection.DependencyProvider.Get(Type)"/> supplies the instance a non-static method runs
/// on. A host that wants <c>[WebService]</c> enforcement passes a <c>Bam.Server.WebServiceRegistry</c>, whose
/// <c>Get</c> refuses unadorned types.
/// </remarks>
public sealed class BamInvocationDispatchRegistry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamInvocationDispatchRegistry"/> class.
    /// </summary>
    /// <param name="registry">The registry holding the invocable types.</param>
    /// <param name="invocableTypes">
    /// The exact types that may be invoked, each of which must be registered in <paramref name="registry"/>; or null to
    /// take every type registered at this moment.
    /// </param>
    /// <exception cref="ArgumentException">A listed type is not registered in <paramref name="registry"/>.</exception>
    public BamInvocationDispatchRegistry(ServiceRegistry registry, IEnumerable<Type>? invocableTypes = null)
    {
        Args.ThrowIfNull(registry, nameof(registry));
        this.Registry = registry;

        HashSet<Type> snapshot = new HashSet<Type>(invocableTypes ?? registry.MappedTypes);
        foreach (Type type in snapshot)
        {
            if (!registry.Contains(type))
            {
                throw new ArgumentException($"'{type.FullName}' is listed as invocable but is not registered in the dispatch registry.", nameof(invocableTypes));
            }
        }

        this.InvocableTypes = snapshot;
    }

    /// <summary>
    /// Gets the registry that supplies instances of the invocable types.
    /// </summary>
    public ServiceRegistry Registry { get; }

    /// <summary>
    /// Gets the allow-list of types an operation identifier may name, frozen at construction.
    /// </summary>
    public IReadOnlySet<Type> InvocableTypes { get; }
}
