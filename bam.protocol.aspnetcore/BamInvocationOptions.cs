namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Settings for the <c>/invoke</c> bridge mapped by <see cref="BamInvocationRegistration.MapBamInvocation"/>.
/// </summary>
/// <remarks>
/// Everything about <em>who</em> may invoke comes from <see cref="ActorAuthenticationOptions"/>; these options only
/// shape the endpoint itself. Bound from configuration when a host calls
/// <see cref="BamInvocationRegistration.AddBamInvocation(Microsoft.Extensions.DependencyInjection.IServiceCollection, Bam.DependencyInjection.ServiceRegistry, BamInvocationOptions?)"/>
/// with an explicit instance; otherwise the defaults apply.
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
}
