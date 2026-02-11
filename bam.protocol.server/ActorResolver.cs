using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Profile;

namespace Bam.Protocol.Server;

public class ActorResolver : IActorResolver
{
    public ActorResolver(IProfileManager profileManager)
    {
        this.ProfileManager = profileManager;
    }

    protected IProfileManager ProfileManager { get; set; }

    public IActor ResolveActor(IBamServerContext context)
    {
        string clientPublicKey = context.ServerSessionState?.Get<string>("ClientPublicKey");
        if (string.IsNullOrEmpty(clientPublicKey))
        {
            return null;
        }

        IProfile profile = ProfileManager.FindProfileByPublicKey(clientPublicKey.Sha256());
        if (profile == null)
        {
            return null;
        }

        return new ActorData { Handle = profile.PersonHandle, Name = profile.Name };
    }
}
