using Bam.Encryption;

namespace Bam.Protocol.Server;

public class ServerSessionInitializationHandler : IBamServerContextInitializationHandler
{
    public ServerSessionInitializationHandler(IServerSessionManager sessionManager)
    {
        this.SessionManager = sessionManager;
    }

    protected IServerSessionManager SessionManager { get; }

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
        Stream outputStream = context.HttpContext?.Response?.OutputStream ?? Stream.Null;

        StartSessionRequest sessionRequest = CreateStartSessionRequest(context.BamRequest);
        StartSessionResponse response = SessionManager.StartSession(sessionRequest, outputStream);

        context.HttpContext.Response.StatusCode = response.StatusCode;
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
