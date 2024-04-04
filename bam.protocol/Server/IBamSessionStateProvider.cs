namespace Bam.Protocol.Server;

public interface IBamSessionStateProvider
{
    IBamSessionState GetSession(IBamServerContext serverContext);
}