using Bam.Data.Repositories;
using Bam.Encryption;
using Bam.Protocol.Data.Server;
using Bam.Protocol.Data.Server.Dao.Repository;
using Bam.Protocol.Profile;
using Bam.Web;
using QueryFilter = Bam.Data.QueryFilter;

namespace Bam.Protocol.Server;

/// <summary>
/// Manages server-side sessions, including creation, retrieval, and termination of sessions with cryptographic key exchange.
/// </summary>
public class ServerSessionManager : IServerSessionManager
{

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerSessionManager"/> class.
    /// </summary>
    /// <param name="repository">The repository for persisting session data.</param>
    /// <param name="signatureProvider">The signature provider for cryptographic operations.</param>
    /// <param name="keyManager">The key manager for generating key pairs.</param>
    /// <param name="nonceProvider">The nonce provider for generating session nonces.</param>
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
    /// <summary>
    /// Gets the session schema repository used for persisting session data.
    /// </summary>
    public ServerSessionSchemaRepository Repository { get; private set; }

    protected StartSessionResponse CreateStartSessionResponse(EccPublicKey serverPublicKey, Stream outputStream, int statusCode = 404)
    {
        return new StartSessionResponse(NonceProvider.GetNonce(), serverPublicKey, outputStream, statusCode);
    }

    /// <summary>
    /// Starts a new session, generating a session ID and server key pair, and persisting the session state.
    /// </summary>
    /// <param name="request">The session start request containing the client's public key.</param>
    /// <param name="outputStream">The output stream for writing the response.</param>
    /// <param name="statusCode">The HTTP status code for the response.</param>
    /// <returns>The session start response containing the session ID and server public key.</returns>
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

    /// <summary>
    /// Ends the session with the specified identifier by deleting it from the repository.
    /// </summary>
    /// <param name="sessionId">The session identifier to end.</param>
    /// <returns>True if the session was found and deleted; otherwise false.</returns>
    public bool EndSession(string sessionId)
    {
        ServerSession session = Repository.OneServerSessionWhere(s => s.SessionId == sessionId);
        if (session == null)
        {
            return false;
        }
        return Repository.Delete(session);
    }

    /// <summary>
    /// Gets the session state for the specified request by looking up the session ID.
    /// </summary>
    /// <param name="request">The BAM request containing session information.</param>
    /// <returns>The session state, or null if no session exists.</returns>
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

    /// <summary>
    /// Extracts the session identifier from the request headers.
    /// </summary>
    /// <param name="request">The BAM request to extract the session ID from.</param>
    /// <returns>The session identifier string, or null if not present.</returns>
    public string GetSessionId(IBamRequest request)
    {
        return request.Headers.GetValueOrDefault(Headers.SessionId);
    }

    /// <summary>
    /// Determines whether the specified request contains a non-empty session identifier.
    /// </summary>
    /// <param name="request">The BAM request to check.</param>
    /// <returns>True if the request has a session ID; otherwise false.</returns>
    public bool HasSessionId(IBamRequest request)
    {
        return !string.IsNullOrEmpty(GetSessionId(request));
    }
}