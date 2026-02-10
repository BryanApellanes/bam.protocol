using Bam.Data.Repositories;
using Bam.Encryption;
using Bam.Protocol.Data.Server;
using Bam.Protocol.Data.Server.Dao.Repository;
using Bam.Protocol.Profile;
using Bam.Web;
using QueryFilter = Bam.Data.QueryFilter;

namespace Bam.Protocol.Server;

public class ServerSessionManager : IServerSessionManager
{

    public ServerSessionManager(ServerSessionSchemaRepository repository, ISignatureProvider signatureProvider, IKeyManager keyManager, INonceProvider nonceProvider)
    {
        this.Repository = repository;
        this.SignatureProvider = signatureProvider;
        this.KeyManager = keyManager;
        this.NonceProvider = nonceProvider;
    }
    protected ISignatureProvider SignatureProvider { get; }
    protected IKeyManager KeyManager { get; set; }
    protected INonceProvider NonceProvider { get; set; }
    public ServerSessionSchemaRepository Repository { get; private set; }

    protected StartSessionResponse CreateStartSessionResponse(EccPublicKey serverPublicKey, Stream outputStream, int statusCode = 404)
    {
        return new StartSessionResponse(NonceProvider.GetNonce(), serverPublicKey, outputStream, statusCode);
    }

    public StartSessionResponse StartSession(StartSessionRequest request, Stream outputStream, int statusCode = 200)
    {
        string sessionId = Cuid.Generate();

        ServerSession session = new ServerSession { SessionId = sessionId };
        session = Repository.Save(session);

        ServerSessionState state = new ServerSessionState(session, Repository);
        state.Set("ClientPublicKey", request.ClientPublicKey?.Pem ?? string.Empty);

        EccPublicPrivateKeyPair serverKeyPair = KeyManager.GenerateEccKeyPair();
        state.Set("ServerPrivateKey", System.Text.Encoding.UTF8.GetString(serverKeyPair.Pem));

        EccPublicKey serverPublicKey = new EccPublicKey(serverKeyPair);

        StartSessionResponse response = CreateStartSessionResponse(serverPublicKey, outputStream, statusCode);
        response.SessionId = sessionId;

        return response;
    }

    public bool EndSession(string sessionId)
    {
        ServerSession session = Repository.OneServerSessionWhere(s => s.SessionId == sessionId);
        if (session == null)
        {
            return false;
        }
        return Repository.Delete(session);
    }

    public IServerSessionState GetSession(IBamRequest request)
    {
        string sessionId = GetSessionId(request);
        if (sessionId == null)
        {
            return null;
        }

        ServerSession session = Repository.OneServerSessionWhere(session=> session.SessionId == sessionId);
        if (session == null)
        {
            return null;
        }

        return new ServerSessionState(session, Repository);
    }

    public string GetSessionId(IBamRequest request)
    {
        return request.Headers.GetValueOrDefault(Headers.SessionId);
    }

    public bool HasSessionId(IBamRequest request)
    {
        return !string.IsNullOrEmpty(GetSessionId(request));
    }
}