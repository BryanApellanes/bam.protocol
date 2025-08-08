namespace Bam.Protocol.Server;

public class DefaultBamResponseProvider : BamResponseProvider
{
    public DefaultBamResponseProvider(IAuthorizationCalculator authorizationCalculator) : base(authorizationCalculator)
    {
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
}