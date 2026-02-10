namespace Bam.Protocol.Server;

public class AuthorizationCalculator : IAuthorizationCalculator
{
    public IAuthorizationCalculation CalculateAuthorization(IBamServerContext serverContext)
    {
        return new AuthorizationCalculation(serverContext, BamAccess.Write);
    }
}
