namespace Bam.Protocol.Server;

public class AuthorizationCalculatorInitializationHandler : IBamServerContextInitializationHandler
{
    public AuthorizationCalculatorInitializationHandler(IAuthorizationCalculator calculator)
    {
        this.AuthorizationCalculator = calculator;
    }
    protected IAuthorizationCalculator AuthorizationCalculator { get; set; }
    public BamServerInitializationContext HandleInitialization(BamServerInitializationContext initialization)
    {
        IBamServerContext context = initialization.ServerContext;
        IAuthorizationCalculation authorization = AuthorizationCalculator.CalculateAuthorization(context);
        if (authorization == null)
        {
            initialization.CanContinue = false;
            initialization.Status = InitializationStatus.AuthorizationCalculationFailed;
        }

        context.SetAuthorizationCalculation(authorization);
        return initialization;
    }
}