using Bam.Protocol.Data;

namespace Bam.Protocol.Server;

public interface IActorResolver
{
    IActor ResolveActor(IBamServerContext context);
}