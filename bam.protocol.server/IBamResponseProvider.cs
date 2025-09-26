namespace Bam.Protocol.Server;

public interface IBamResponseProvider
{
    IBamResponse CreateResponse(BamServerInitializationContext initialization);
    IBamResponse CreateResponse(IBamServerContext serverContext);
}