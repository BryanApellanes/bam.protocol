namespace Bam.Protocol.Client;

public interface IBamServiceClient : IBamClient
{
    
    StartSessionResponse StartSession(string host, int port);
}