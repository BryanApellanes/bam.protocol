namespace Bam.Protocol.Server;

public interface IActorResolver
{
    IActor ResolveActor(IBamRequest request);
    
}