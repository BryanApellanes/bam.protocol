namespace Bam.Protocol.Server;

/// <summary>
/// Default response provider that creates responses based on initialization status and access level, executing commands for successful requests.
/// </summary>
public class DefaultBamResponseProvider : BamResponseProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultBamResponseProvider"/> class.
    /// </summary>
    /// <param name="authorizationCalculator">The authorization calculator.</param>
    /// <param name="requestProcessor">The request processor for executing commands.</param>
    public DefaultBamResponseProvider(IAuthorizationCalculator authorizationCalculator, IBamRequestProcessor requestProcessor)
        : base(authorizationCalculator)
    {
        this.RequestProcessor = requestProcessor;
        this.FailureResponseProviders =
            new Dictionary<RequestType, Func<BamServerInitializationContext, IBamResponse>>()
            {
                { RequestType.Http, (initialization) => new HttpRequestInitializationFailedResponse(initialization) },
                { RequestType.Tcp, (initialization) => new TcpRequestInitializationFailedResponse(initialization) }
            };
    }

    protected IBamRequestProcessor RequestProcessor { get; }

    protected Dictionary<RequestType, Func<BamServerInitializationContext, IBamResponse>> FailureResponseProviders
    {
        get;
        set;
    }

    private BamServerInitializationContext _currentInitialization = null!;

    protected override IBamResponse CreateFailureResponse(BamServerInitializationContext initialization)
    {
        if (initialization.Status == InitializationStatus.Success)
        {
            _currentInitialization = initialization;
            return CreateResponse(initialization.ServerContext);
        }

        if (this.FailureResponseProviders.ContainsKey(initialization.ServerContext.RequestType))
        {
            return this.FailureResponseProviders[initialization.ServerContext.RequestType](initialization);
        }

        return new BamResponse<BamProtocolException>(initialization, GetStatusCode(initialization.Status))
        {
            Content = new BamProtocolException("Request context initialization failed")
        };
    }

    /// <summary>
    /// Creates a response for a write access request by processing the request context.
    /// </summary>
    /// <param name="serverContext">The server context for the write request.</param>
    /// <returns>The write response.</returns>
    public override IBamResponse CreateWriteResponse(IBamServerContext serverContext)
    {
        object result = RequestProcessor.ProcessRequestContext(serverContext);
        return new BamResponse<object>(_currentInitialization, 200) { Content = result };
    }

    /// <summary>
    /// Creates a response for an execute access request by processing the request context.
    /// </summary>
    /// <param name="serverContext">The server context for the execute request.</param>
    /// <returns>The execute response.</returns>
    public override IBamResponse CreateExecuteResponse(IBamServerContext serverContext)
    {
        object result = RequestProcessor.ProcessRequestContext(serverContext);
        return new BamResponse<object>(_currentInitialization, 200) { Content = result };
    }

    /// <summary>
    /// Creates a response for a read access request by processing the request context.
    /// </summary>
    /// <param name="serverContext">The server context for the read request.</param>
    /// <returns>The read response.</returns>
    public override IBamResponse CreateReadResponse(IBamServerContext serverContext)
    {
        object result = RequestProcessor.ProcessRequestContext(serverContext);
        return new BamResponse<object>(_currentInitialization, 200) { Content = result };
    }

    /// <summary>
    /// Creates a 403 response for a denied access request.
    /// </summary>
    /// <param name="serverContext">The server context for the denied request.</param>
    /// <returns>The denied response.</returns>
    public override IBamResponse CreateDeniedResponse(IBamServerContext serverContext)
    {
        return new BamResponse<object>(_currentInitialization, 403)
        {
            Content = new { Message = "Access Denied" }
        };
    }

    /// <summary>
    /// Logs when access is denied. Default implementation is a no-op.
    /// </summary>
    /// <param name="serverContext">The server context for the denied request.</param>
    public override void LogAccessDenied(IBamServerContext serverContext)
    {
        // Default no-op; can be overridden for logging
    }

    /// <summary>
    /// Maps an initialization status to an HTTP status code.
    /// </summary>
    /// <param name="status">The initialization status.</param>
    /// <returns>The corresponding HTTP status code.</returns>
    public static int GetStatusCode(InitializationStatus status)
    {
        switch (status)
        {
            case InitializationStatus.SessionInitializationFailed:
                return 419;
            case InitializationStatus.SessionRequired:
                return 420;
            case InitializationStatus.ActorResolutionFailed:
                return 460;
            case InitializationStatus.CommandResolutionFailed:
                return 461;
            case InitializationStatus.AuthorizationCalculationFailed:
                return 462;
            case InitializationStatus.Success:
                return 200;
            case InitializationStatus.Invalid:
            case InitializationStatus.UnknownError:
            default:
                return 500;
        }
    }
}
