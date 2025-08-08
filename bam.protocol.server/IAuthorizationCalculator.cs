namespace Bam.Protocol.Server;

public interface IAuthorizationCalculator
{
    IAuthorizationCalculation CalculateAuthorization(IBamServerContext serverContext);
}