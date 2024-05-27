using Bam.Logging;

namespace Bam.Protocol.Server;

public abstract class BamResponseProvider : IBamResponseProvider
{
    public BamResponseProvider(IBamAuthorizationCalculator authorizationCalculator)
    {
        this.AuthorizationCalculator = authorizationCalculator;
    }
    
    private  IBamAuthorizationCalculator AuthorizationCalculator { get; set; } 
    
    public IBamResponse CreateResponse(IBamServerContext serverContext)
    {
        BamAuthorizationCalculation authorizationCalculation = AuthorizationCalculator.CalculateAuthorization(serverContext);
        switch (authorizationCalculation.Access)
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