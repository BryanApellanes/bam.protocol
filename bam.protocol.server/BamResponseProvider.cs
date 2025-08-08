namespace Bam.Protocol.Server;

public abstract class BamResponseProvider : IBamResponseProvider
{
    public BamResponseProvider(IAuthorizationCalculator authorizationCalculator)
    {
        this.AuthorizationCalculator = authorizationCalculator;
    }
    
    private  IAuthorizationCalculator AuthorizationCalculator { get; set; } 
    
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