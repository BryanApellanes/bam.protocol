namespace Bam.Protocol.Server;

/// <summary>
/// Handles the authentication step during server context initialization.
/// </summary>
public class AuthenticationInitializationHandler : IBamServerContextInitializationHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationInitializationHandler"/> class.
    /// </summary>
    /// <param name="authenticator">The authenticator to use.</param>
    public AuthenticationInitializationHandler(IAuthenticator authenticator)
    {
        Authenticator = authenticator;
    }

    protected IAuthenticator Authenticator { get; set; }

    /// <summary>
    /// Handles the initialization step by authenticating the current server context.
    /// </summary>
    /// <param name="initialization">The initialization context to process.</param>
    /// <returns>The updated initialization context.</returns>
    public BamServerInitializationContext HandleInitialization(BamServerInitializationContext initialization)
    {
        IBamServerContext context = initialization.ServerContext;
        BamAuthentication authentication = Authenticator.Authenticate(context);
        if (authentication == null || !authentication.Success)
        {
            initialization.CanContinue = false;
            initialization.Status = InitializationStatus.AuthenticationFailed;
        }

        context.SetAuthentication(authentication!);
        return initialization;
    }
}
