namespace Bam.Protocol.Server;

public interface IBamResponseProvider
{
    IBamResponse CreateResponse(IBamServerContext serverContext);
}