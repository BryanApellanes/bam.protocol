namespace Bam.Protocol.Server;

/// <summary>
/// Handles the authorization calculation step during server context initialization.
/// </summary>
public class AuthorizationCalculatorInitializationHandler : IBamServerContextInitializationHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationCalculatorInitializationHandler"/> class.
    /// </summary>
    /// <param name="calculator">The authorization calculator to use.</param>
    public AuthorizationCalculatorInitializationHandler(IAuthorizationCalculator calculator)
    {
        this.AuthorizationCalculator = calculator;
    }
    protected IAuthorizationCalculator AuthorizationCalculator { get; set; }
    /// <summary>
    /// Handles the initialization step by calculating authorization for the current server context.
    /// </summary>
    /// <param name="initialization">The initialization context to process.</param>
    /// <returns>The updated initialization context.</returns>
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