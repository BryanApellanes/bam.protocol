namespace Bam.Protocol.Server;

public interface IBamContextProvider
{
    IBamServerContext CreateContext(Stream stream, string requestId);
}