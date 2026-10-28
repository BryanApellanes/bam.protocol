using System.Collections.Frozen;
using Bam.DependencyInjection;

namespace Bam.Protocol.AspNetCore;

/// <summary>
/// The <see cref="ServiceRegistry"/> whose types the <c>/invoke</c> bridge may dispatch to, with the host-written
/// allow-list of invocable types frozen when the bridge is registered.
/// </summary>
/// <remarks>
/// A host hands its dispatch registry and the list of invocable types to
/// <see cref="BamInvocationRegistration.AddBamInvocation(Microsoft.Extensions.DependencyInjection.IServiceCollection, ServiceRegistry, BamInvocationOptions?)"/>;
/// this wrapper gives them a distinct identity in the ASP.NET container so they never collide with any other
/// <see cref="ServiceRegistry"/> the host registers. <see cref="InvocableTypes"/> is the complete allow-list and is
/// never derived from the registry: <see cref="Bam.DependencyInjection.DependencyProvider.MappedTypes"/> is a
/// construction cache that also holds the implementation class behind an interface key and every constructor
/// dependency the registry resolved, whether before or after the bridge was registered, so none of those is
/// invocable unless the host lists it. Each listed type must be registered.
/// <see cref="Bam.DependencyInjection.DependencyProvider.Get(Type)"/> supplies the instance a non-static method runs
/// on. A host that wants <c>[WebService]</c> enforcement passes a <c>Bam.Server.WebServiceRegistry</c>, whose
/// <c>Get</c> refuses unadorned types (static methods take no instance, so that check does not run for them; see
/// bam.protocol#70).
/// </remarks>
public sealed class BamInvocationDispatchRegistry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamInvocationDispatchRegistry"/> class.
    /// </summary>
    /// <param name="registry">The registry that supplies instances of the invocable types.</param>
    /// <param name="invocableTypes">The exact types that may be invoked; at least one, each held by <paramref name="registry"/> (a type the registry only constructed and cached counts, since the host listed it).</param>
    /// <exception cref="ArgumentException">No type is listed, an entry is null, or <paramref name="registry"/> does not hold a listed type.</exception>
    public BamInvocationDispatchRegistry(ServiceRegistry registry, IEnumerable<Type> invocableTypes)
    {
        Args.ThrowIfNull(registry, nameof(registry));
        Args.ThrowIfNull(invocableTypes, nameof(invocableTypes));
        this.Registry = registry;

        HashSet<Type> listed = new HashSet<Type>(invocableTypes);
        if (listed.Count == 0)
        {
            throw new ArgumentException("At least one invocable type must be listed; the bridge never derives the allow-list from the registry.", nameof(invocableTypes));
        }

        foreach (Type type in listed)
        {
            if (type is null)
            {
                throw new ArgumentException("The invocable type list contains a null entry.", nameof(invocableTypes));
            }

            if (!registry.Contains(type))
            {
                throw new ArgumentException($"'{type.FullName}' is listed as invocable but the dispatch registry does not hold it.", nameof(invocableTypes));
            }
        }

        this.InvocableTypes = listed.ToFrozenSet();
    }

    /// <summary>
    /// Gets the registry that supplies instances of the invocable types.
    /// </summary>
    public ServiceRegistry Registry { get; }

    /// <summary>
    /// Gets the immutable allow-list of types an operation identifier may name.
    /// </summary>
    public FrozenSet<Type> InvocableTypes { get; }
}
