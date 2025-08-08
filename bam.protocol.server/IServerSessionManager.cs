namespace Bam.Protocol.Server;

public interface IServerSessionManager
{
    StartSessionResponse StartSession(StartSessionRequest request, Stream outputStream, int statusCode = 200);
    bool EndSession(string sessionId);
    IServerSessionState GetSession(IBamRequest request);
    string GetSessionId(IBamRequest request);
    bool HasSessionId(IBamRequest request);
}