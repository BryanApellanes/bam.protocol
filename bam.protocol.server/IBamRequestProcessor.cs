namespace Bam.Protocol.Server;

public interface IBamRequestProcessor
{
    object ProcessRequestContext(IBamServerContext serverContext);
}
