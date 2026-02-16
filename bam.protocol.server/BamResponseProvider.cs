namespace Bam.Protocol.Server;

/// <summary>
/// Abstract base class for creating BAM responses based on authorization and initialization status.
/// </summary>
public abstract class BamResponseProvider : IBamResponseProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamResponseProvider"/> class.
    /// </summary>
    /// <param name="authorizationCalculator">The authorization calculator used for access decisions.</param>
    public BamResponseProvider(IAuthorizationCalculator authorizationCalculator)
    {
        this.AuthorizationCalculator = authorizationCalculator;
        this.InitializationStatusResponseHandlers =
            new Dictionary<InitializationStatus, Func<IBamResponse>>();
    }
    
    private  IAuthorizationCalculator AuthorizationCalculator { get; set; }

    protected Dictionary<InitializationStatus, Func<IBamResponse>> InitializationStatusResponseHandlers
    {
        get;
        set;
    }

    protected abstract IBamResponse CreateFailureResponse(BamServerInitializationContext initialization);
    
    /// <summary>
    /// Creates a response for the specified initialization context, delegating to status-specific handlers.
    /// </summary>
    /// <param name="initialization">The initialization context containing the current status.</param>
    /// <returns>The created response.</returns>
    public IBamResponse CreateResponse(BamServerInitializationContext initialization)
    {
        if (InitializationStatusResponseHandlers.ContainsKey(initialization.Status))
        {
            return InitializationStatusResponseHandlers[initialization.Status]();
        }

        return CreateFailureResponse(initialization);
    }


    /// <summary>
    /// Creates a response for the specified server context based on the authorization access level.
    /// </summary>
    /// <param name="serverContext">The server context to create a response for.</param>
    /// <returns>The created response.</returns>
    public IBamResponse CreateResponse(IBamServerContext serverContext)
    {
        switch (serverContext.AuthorizationCalculation.Access)
        {
            case BamAccess.Read:
                return CreateReadResponse(serverContext);
                break;
            case BamAccess.Write:
                return CreateWriteResponse(serverContext);
            default:
            case BamAccess.Denied:
                LogAccessDenied(serverContext);
                return CreateDeniedResponse(serverContext);
                break;
        }
    }

    /// <summary>
    /// Creates a response for a denied access request.
    /// </summary>
    /// <param name="serverContext">The server context for the denied request.</param>
    /// <returns>The denied response.</returns>
    public abstract IBamResponse CreateDeniedResponse(IBamServerContext serverContext);

    /// <summary>
    /// Creates a response for a read access request.
    /// </summary>
    /// <param name="serverContext">The server context for the read request.</param>
    /// <returns>The read response.</returns>
    public abstract IBamResponse CreateReadResponse(IBamServerContext serverContext);

    /// <summary>
    /// Creates a response for a write access request.
    /// </summary>
    /// <param name="serverContext">The server context for the write request.</param>
    /// <returns>The write response.</returns>
    public abstract IBamResponse CreateWriteResponse(IBamServerContext serverContext);

    /// <summary>
    /// Logs information when access is denied for a request.
    /// </summary>
    /// <param name="serverContext">The server context for the denied request.</param>
    public abstract void LogAccessDenied(IBamServerContext serverContext);
}