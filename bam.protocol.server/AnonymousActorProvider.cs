using Bam.Protocol.Data.Common;

namespace Bam.Protocol.Server;

/// <summary>
/// Default <see cref="IAnonymousActorProvider"/> implementation that supplies a well-known, non-persisted anonymous actor.
/// </summary>
public class AnonymousActorProvider : IAnonymousActorProvider
{
    /// <summary>
    /// The well-known handle assigned to the anonymous actor.
    /// </summary>
    public const string AnonymousHandle = "UNKNOWN";

    /// <summary>
    /// The well-known display name assigned to the anonymous actor.
    /// </summary>
    public const string AnonymousName = "Anonymous";

    /// <summary>
    /// Gets the well-known anonymous actor.
    /// </summary>
    /// <returns>A non-persisted <see cref="ActorData"/> sentinel identifying the anonymous actor.</returns>
    public IActor GetAnonymousActor()
    {
        return new ActorData { Handle = AnonymousHandle, Name = AnonymousName };
    }
}
