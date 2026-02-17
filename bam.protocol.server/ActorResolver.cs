using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Profile;

namespace Bam.Protocol.Server;

/// <summary>
/// Resolves actors from server contexts by looking up profiles using the client's public key.
/// </summary>
public class ActorResolver : IActorResolver
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActorResolver"/> class.
    /// </summary>
    /// <param name="profileManager">The profile manager for looking up actor profiles.</param>
    public ActorResolver(IProfileManager profileManager)
    {
        this.ProfileManager = profileManager;
    }

    protected IProfileManager ProfileManager { get; set; }

    /// <summary>
    /// Resolves the actor from the server context using the client's public key stored in session state.
    /// </summary>
    /// <param name="context">The server context to resolve the actor from.</param>
    /// <returns>The resolved actor, or null if the public key or profile is not found.</returns>
    public IActor ResolveActor(IBamServerContext context)
    {
        string clientPublicKey = context.ServerSessionState?.Get<string>("ClientPublicKey")!;
        if (string.IsNullOrEmpty(clientPublicKey))
        {
            return null!;
        }

        IProfile profile = ProfileManager.FindProfileByPublicKey(clientPublicKey.Sha256());
        if (profile == null)
        {
            return null!;
        }

        return new ActorData { Handle = profile.PersonHandle, Name = profile.Name };
    }
}
