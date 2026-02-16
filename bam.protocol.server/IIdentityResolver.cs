namespace Bam.Protocol.Server;

/// <summary>
/// Defines a component that resolves an identity from a server context.
/// </summary>
public interface IIdentityResolver
{
    /// <summary>
    /// Resolves the identity associated with the specified server context.
    /// </summary>
    /// <param name="serverContext">The server context to resolve the identity from.</param>
    /// <returns>The resolved identity.</returns>
    IIdentity ResolveIdentity(IBamServerContext serverContext);
}