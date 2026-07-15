namespace Bam.Protocol.Server;

/// <summary>
/// Supplies the well-known actor identity assigned to anonymous-access requests.
/// </summary>
public interface IAnonymousActorProvider
{
    /// <summary>
    /// Gets the well-known anonymous actor.
    /// </summary>
    /// <returns>The anonymous actor.</returns>
    IActor GetAnonymousActor();
}
