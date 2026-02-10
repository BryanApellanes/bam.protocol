using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Bam.Web;

namespace Bam.Protocol.Server;

public class ActorResolver : IActorResolver
{
    public IActor ResolveActor(IBamRequest request)
    {
        string sessionId = request.Headers.GetValueOrDefault(Headers.SessionId);
        if (string.IsNullOrEmpty(sessionId))
        {
            return null;
        }

        return new ActorData { Handle = sessionId, Name = sessionId };
    }
}
