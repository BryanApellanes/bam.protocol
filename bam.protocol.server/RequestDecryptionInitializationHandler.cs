namespace Bam.Protocol.Server;

/// <summary>
/// Handles request body decryption during server context initialization, before command resolution runs.
/// </summary>
public class RequestDecryptionInitializationHandler : IBamServerContextInitializationHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequestDecryptionInitializationHandler"/> class.
    /// </summary>
    /// <param name="requestSecurityValidator">The request security validator used to decrypt the request body.</param>
    public RequestDecryptionInitializationHandler(RequestSecurityValidator requestSecurityValidator)
    {
        this.RequestSecurityValidator = requestSecurityValidator;
    }

    protected RequestSecurityValidator RequestSecurityValidator { get; set; }

    /// <summary>
    /// Handles the initialization step by decrypting the request body when session state has already been resolved.
    /// </summary>
    /// <param name="initialization">The initialization context to process.</param>
    /// <returns>The updated initialization context.</returns>
    public BamServerInitializationContext HandleInitialization(BamServerInitializationContext initialization)
    {
        IBamServerContext context = initialization.ServerContext;
        if (context.ServerSessionState == null)
        {
            return initialization;
        }

        string decrypted = RequestSecurityValidator.DecryptBody(context);
        if (decrypted != null)
        {
            context.BamRequest.Content = decrypted;
        }

        return initialization;
    }
}
