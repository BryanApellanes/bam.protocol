namespace Bam.Protocol.Server;

public class DefaultBamResponseProvider : BamResponseProvider
{
    public DefaultBamResponseProvider(IAuthorizationCalculator authorizationCalculator) : base(authorizationCalculator)
    {
        this.FailureResponseProviders =
            new Dictionary<RequestType, Func<BamServerInitializationContext, IBamResponse>>()
            {
                { RequestType.Http, (initialization) => new HttpRequestInitializationFailedResponse(initialization) }
            };
    }

    protected Dictionary<RequestType, Func<BamServerInitializationContext, IBamResponse>> FailureResponseProviders
    {
        get;
        set;
    }

    protected override IBamResponse CreateFailureResponse(BamServerInitializationContext initialization)
    {
        if (this.FailureResponseProviders.ContainsKey(initialization.ServerContext.RequestType))
        {
            return this.FailureResponseProviders[initialization.ServerContext.RequestType](initialization);
        }

        return new BamResponse<BamProtocolException>(initialization, GetStatusCode(initialization))
        {
            Content = new BamProtocolException("Request context initialization failed")
        };
    }

    public override IBamResponse CreateDeniedResponse(IBamServerContext serverContext)
    {
        throw new NotImplementedException();
    }

    public override IBamResponse CreateReadResponse(IBamServerContext serverContext)
    {
        throw new NotImplementedException();
    }

    public override IBamResponse CreateWriteResponse(IBamServerContext serverContext)
    {
        throw new NotImplementedException();
    }

    public override void LogAccessDenied(IBamServerContext serverContext)
    {
        throw new NotImplementedException();
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