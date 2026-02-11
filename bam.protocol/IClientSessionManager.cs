namespace Bam.Protocol.Client;

public interface IClientSessionManager
{
    Task<StartSessionResponse> StartSessionAsync(StartSessionRequest request);
    Task<IClientSessionState> StartSessionAsync();
}