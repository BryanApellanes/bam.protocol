using Bam.Encryption;

namespace Bam.Protocol.Server;

/// <summary>
/// Handles session initialization during server context setup, including session creation and validation.
/// </summary>
public class ServerSessionInitializationHandler : IBamServerContextInitializationHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerSessionInitializationHandler"/> class.
    /// </summary>
    /// <param name="sessionManager">The session manager for session operations.</param>
    public ServerSessionInitializationHandler(IServerSessionManager sessionManager)
    {
        this.SessionManager = sessionManager;
    }

    protected IServerSessionManager SessionManager { get; }

    /// <summary>
    /// Handles the initialization step by resolving or creating a session for the current request.
    /// </summary>
    /// <param name="initialization">The initialization context to process.</param>
    /// <returns>The updated initialization context.</returns>
    public BamServerInitializationContext HandleInitialization(BamServerInitializationContext initialization)
    {
        IBamServerContext context = initialization.ServerContext;
        IBamRequest request = context.BamRequest;
        if (SessionManager.HasSessionId(request))
        {
            IServerSessionState state = SessionManager.GetSession(context.BamRequest);
            if (state == null)
            {
                initialization.CanContinue = false;
                initialization.Status = InitializationStatus.SessionInitializationFailed;
                initialization.Message = "Session Initialization Failed";
            }

            context.SetSessionState(state);
        }
        else
        {
            if (IsSessionCreationRequest(request))
            {
                HandleSessionCreation(initialization);
            }
            else
            {
                initialization.CanContinue = false;
                initialization.Status = InitializationStatus.SessionRequired;
                initialization.Message = "Session Required";
            }
        }

        return initialization;
    }

    private bool IsSessionCreationRequest(IBamRequest request)
    {
        return request.Url?.AbsolutePath?.Equals(BamSessionPaths.Create, StringComparison.OrdinalIgnoreCase) == true;
    }

    private void HandleSessionCreation(BamServerInitializationContext initialization)
    {
        IBamServerContext context = initialization.ServerContext;
        MemoryStream memoryStream = new MemoryStream();
        Stream outputStream = context.HttpContext?.Response?.OutputStream ?? memoryStream;

        StartSessionRequest sessionRequest = CreateStartSessionRequest(context.BamRequest);
        StartSessionResponse response = SessionManager.StartSession(sessionRequest, outputStream);

        if (context.HttpContext != null)
        {
            context.HttpContext.Response.StatusCode = response.StatusCode;
        }
        context.BamResponse = response;
        initialization.CanContinue = false;
    }

    private StartSessionRequest CreateStartSessionRequest(IBamRequest request)
    {
        StartSessionRequest sessionRequest = new StartSessionRequest();
        string content = request.Content;
        if (!string.IsNullOrEmpty(content))
        {
            var json = System.Text.Json.JsonDocument.Parse(content);
            if (json.RootElement.TryGetProperty("ClientPublicKey", out var keyElement))
            {
                string pem = keyElement.GetString();
                if (!string.IsNullOrEmpty(pem))
                {
                    sessionRequest.ClientPublicKey = new EccPublicKey(pem);
                }
            }
        }
        return sessionRequest;
    }
}
