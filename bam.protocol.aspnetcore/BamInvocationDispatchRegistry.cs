using Bam.DependencyInjection;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// The <see cref="ServiceRegistry"/> whose registered types the <c>/invoke</c> bridge may dispatch to.
/// </summary>
/// <remarks>
/// A host hands its dispatch registry to
/// <see cref="BamInvocationRegistration.AddBamInvocation(Microsoft.Extensions.DependencyInjection.IServiceCollection, ServiceRegistry, BamInvocationOptions?)"/>;
/// this wrapper gives it a distinct identity in the ASP.NET container so it never collides with any other
/// <see cref="ServiceRegistry"/> the host registers. The registered type keys (<see cref="Bam.DependencyInjection.DependencyProvider.MappedTypes"/>)
/// are the complete allow-list of invocable types, and <see cref="Bam.DependencyInjection.DependencyProvider.Get(Type)"/> supplies the
/// instance a non-static method runs on. A host that wants <c>[WebService]</c> enforcement passes a
/// <c>Bam.Server.WebServiceRegistry</c>, whose <c>Get</c> refuses unadorned types.
/// </remarks>
public sealed class BamInvocationDispatchRegistry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamInvocationDispatchRegistry"/> class.
    /// </summary>
    /// <param name="registry">The registry holding the invocable types.</param>
    public BamInvocationDispatchRegistry(ServiceRegistry registry)
    {
        Args.ThrowIfNull(registry, nameof(registry));
        this.Registry = registry;
    }

    /// <summary>
    /// Gets the registry holding the invocable types.
    /// </summary>
    public ServiceRegistry Registry { get; }
}
