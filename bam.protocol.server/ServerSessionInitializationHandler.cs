namespace Bam.Protocol.Server;

public class ServerSessionInitializationHandler : IBamServerContextInitializationHandler
{
    public ServerSessionInitializationHandler(IServerSessionManager sessionManager)
    {
        this.SessionManager = sessionManager;
    }
    
    protected IServerSessionManager SessionManager { get; }
    
    public BamServerContextInitialization HandleInitialization(BamServerContextInitialization initialization)
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
            initialization.CanContinue = false;
            initialization.Status = InitializationStatus.SessionRequired;
            initialization.Message = "Session Required";
        }

        return initialization;
    }
}