namespace Bam.Protocol.Server;

public abstract class BamResponseProvider : IBamResponseProvider
{
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
    
    public IBamResponse CreateResponse(BamServerInitializationContext initialization)
    {
        if (InitializationStatusResponseHandlers.ContainsKey(initialization.Status))
        {
            return InitializationStatusResponseHandlers[initialization.Status]();
        }

        return CreateFailureResponse(initialization);
    }


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

    public abstract IBamResponse CreateDeniedResponse(IBamServerContext serverContext);
    public abstract IBamResponse CreateReadResponse(IBamServerContext serverContext);
    public abstract IBamResponse CreateWriteResponse(IBamServerContext serverContext);

    public abstract void LogAccessDenied(IBamServerContext serverContext);
}