namespace Bam.Protocol.Server;

public interface IBamAuthorizationCalculator
{
    BamAuthorizationCalculation CalculateAuthorization(IBamServerContext serverContext);
}