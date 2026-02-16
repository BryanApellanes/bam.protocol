namespace Bam.Protocol.Server;

/// <summary>
/// Defines a provider that determines the access level for a given server context.
/// </summary>
public interface IAccessLevelProvider
{
    /// <summary>
    /// Gets the access level for the specified server context.
    /// </summary>
    /// <param name="context">The server context to evaluate.</param>
    /// <returns>The calculated access level.</returns>
    BamAccess GetAccessLevel(IBamServerContext context);
}
