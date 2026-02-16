using Bam.Protocol.Data;

namespace Bam.Protocol.Server;

/// <summary>
/// Defines a component that resolves an actor from a server context.
/// </summary>
public interface IActorResolver
{
    /// <summary>
    /// Resolves the actor associated with the specified server context.
    /// </summary>
    /// <param name="context">The server context to resolve the actor from.</param>
    /// <returns>The resolved actor, or null if no actor could be resolved.</returns>
    IActor ResolveActor(IBamServerContext context);
}