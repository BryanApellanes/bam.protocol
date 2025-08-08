namespace Bam.Protocol.Client;

public interface IClientSessionManager
{
    Task<StartSessionResponse> StartSessionAsync(StartSessionRequest request);
}