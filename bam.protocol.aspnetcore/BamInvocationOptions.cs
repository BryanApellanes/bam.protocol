namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Settings for the <c>/invoke</c> bridge mapped by <see cref="BamInvocationRegistration.MapBamInvocation"/>.
/// </summary>
/// <remarks>
/// Everything about <em>who</em> may invoke comes from <see cref="ActorAuthenticationOptions"/>; these options
/// shape the endpoint itself and name <em>what</em> may be invoked. A host passes an instance to
/// <see cref="BamInvocationRegistration.AddBamInvocation(Microsoft.Extensions.DependencyInjection.IServiceCollection, Bam.DependencyInjection.ServiceRegistry, BamInvocationOptions?)"/>
/// with <see cref="InvocableTypes"/> set; the other members have defaults.
/// </remarks>
public sealed record BamInvocationOptions
{
    /// <summary>The default route pattern, matching what <c>BamClient&lt;T&gt;</c> posts to.</summary>
    public const string DefaultPattern = "/invoke";

    /// <summary>The default cap on a request body, in bytes (1 MiB).</summary>
    public const long DefaultMaxBodyBytes = 1024 * 1024;

    /// <summary>
    /// Gets the route pattern the invocation endpoint is mapped at. Default <see cref="DefaultPattern"/>.
    /// </summary>
    public string Pattern { get; init; } = DefaultPattern;

    /// <summary>
    /// Gets the largest request body the endpoint reads, in bytes; a larger body answers 413 before anything is
    /// deserialized. Default <see cref="DefaultMaxBodyBytes"/>.
    /// </summary>
    public long MaxBodyBytes { get; init; } = DefaultMaxBodyBytes;

    /// <summary>
    /// Gets the exact types the bridge may dispatch to. Required: <see cref="BamInvocationRegistration.AddBamInvocation"/>
    /// throws when it is null or empty, and each listed type must be registered in the dispatch registry. The bridge
    /// never derives this list from the registry, whose type list is a construction cache (interface implementations,
    /// constructor dependencies) rather than a declaration of what may be invoked.
    /// </summary>
    public IReadOnlyCollection<Type>? InvocableTypes { get; init; }
}
