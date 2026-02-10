namespace Bam.Protocol.Server;

public class DefaultBamResponseProvider : BamResponseProvider
{
    public DefaultBamResponseProvider(IAuthorizationCalculator authorizationCalculator, IBamRequestProcessor requestProcessor)
        : base(authorizationCalculator)
    {
        this.RequestProcessor = requestProcessor;
        this.FailureResponseProviders =
            new Dictionary<RequestType, Func<BamServerInitializationContext, IBamResponse>>()
            {
                { RequestType.Http, (initialization) => new HttpRequestInitializationFailedResponse(initialization) }
            };
    }

    protected IBamRequestProcessor RequestProcessor { get; }

    protected Dictionary<RequestType, Func<BamServerInitializationContext, IBamResponse>> FailureResponseProviders
    {
        get;
        set;
    }

    private BamServerInitializationContext _currentInitialization;

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

        return new BamResponse<BamProtocolException>(initialization, GetStatusCode(initialization))
        {
            Content = new BamProtocolException("Request context initialization failed")
        };
    }

    public override IBamResponse CreateWriteResponse(IBamServerContext serverContext)
    {
        object result = RequestProcessor.ProcessRequestContext(serverContext);
        return new BamResponse<object>(_currentInitialization, 200) { Content = result };
    }

    public override IBamResponse CreateReadResponse(IBamServerContext serverContext)
    {
        object result = RequestProcessor.ProcessRequestContext(serverContext);
        return new BamResponse<object>(_currentInitialization, 200) { Content = result };
    }

    public override IBamResponse CreateDeniedResponse(IBamServerContext serverContext)
    {
        return new BamResponse<object>(_currentInitialization, 403)
        {
            Content = new { Message = "Access Denied" }
        };
    }

    public override void LogAccessDenied(IBamServerContext serverContext)
    {
        // Default no-op; can be overridden for logging
    }

    private int GetStatusCode(BamServerInitializationContext initialization)
    {
        switch (initialization.Status)
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
