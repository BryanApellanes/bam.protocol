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
       // this.PrivateKeyStorage = privateKeyStorage;
       // this.PublicKeyStorage = publicKeyStorage;
        this.NonceProvider = nonceProvider;
    }
    protected ISignatureProvider SignatureProvider { get; }
    protected IKeyManager KeyManager { get; set; }
   // protected IPrivateKeyStorage PrivateKeyStorage { get; set; }
   // protected IPublicKeyStorage PublicKeyStorage { get; set; }
    protected INonceProvider NonceProvider { get; set; }
    public ServerSessionSchemaRepository Repository { get; private set; }

    protected StartSessionResponse CreateStartSessionResponse(EccPublicKey serverPublicKey, Stream outputStream, int statusCode = 404)
    {
        return new StartSessionResponse(NonceProvider.GetNonce(), serverPublicKey, outputStream, statusCode);
    }

    public StartSessionResponse StartSession(StartSessionRequest request, Stream outputStream, int statusCode = 200)
    {
        throw new NotImplementedException();
        //KeyManager.StoreClientPublicKey(request.ClientPublicKey);

        return CreateStartSessionResponse(request.ClientPublicKey, outputStream, statusCode);
    }

    public bool EndSession(string sessionId)
    {
        throw new NotImplementedException();
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