namespace Bam.Protocol.Server;

public interface IBamActorResolver
{
    IBamActor ResolveActor(IBamRequest request);
    
}