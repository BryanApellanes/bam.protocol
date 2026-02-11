namespace Bam.Protocol.Server;

public class AuthenticationInitializationHandler : IBamServerContextInitializationHandler
{
    public AuthenticationInitializationHandler(IAuthenticator authenticator)
    {
        Authenticator = authenticator;
    }

    protected IAuthenticator Authenticator { get; set; }

    public BamServerInitializationContext HandleInitialization(BamServerInitializationContext initialization)
    {
        IBamServerContext context = initialization.ServerContext;
        BamAuthentication authentication = Authenticator.Authenticate(context);
        if (authentication == null || !authentication.Success)
        {
            initialization.CanContinue = false;
            initialization.Status = InitializationStatus.AuthenticationFailed;
        }

        context.SetAuthentication(authentication);
        return initialization;
    }
}
