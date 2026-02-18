namespace Bam.Protocol.Server;

/// <summary>
/// Calculates authorization by comparing the actor's access level against the required access for the resolved command.
/// </summary>
public class AuthorizationCalculator : IAuthorizationCalculator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationCalculator"/> class.
    /// </summary>
    /// <param name="accessLevelProvider">The access level provider for determining actor access.</param>
    public AuthorizationCalculator(IAccessLevelProvider accessLevelProvider)
    {
        AccessLevelProvider = accessLevelProvider;
    }

    private IAccessLevelProvider AccessLevelProvider { get; }

    /// <summary>
    /// Calculates the authorization for the specified server context by comparing the actor's access level against the command's required access.
    /// </summary>
    /// <param name="serverContext">The server context to authorize.</param>
    /// <returns>The authorization calculation result.</returns>
    public IAuthorizationCalculation CalculateAuthorization(IBamServerContext serverContext)
    {
        ICommand command = serverContext.Command;
        if (command == null)
        {
            return Denied(serverContext, "No command resolved");
        }

        BamAccess requiredAccess = CommandAttributeResolver.GetRequiredAccess(command);

        if (CommandAttributeResolver.IsAnonymousAccessAllowed(command))
        {
            if (requiredAccess == BamAccess.Denied)
            {
                requiredAccess = BamAccess.Execute;
            }
            return new AuthorizationCalculation(serverContext, requiredAccess);
        }

        BamAccess actorAccess = AccessLevelProvider.GetAccessLevel(serverContext);

        if (actorAccess >= requiredAccess)
        {
            return new AuthorizationCalculation(serverContext, requiredAccess);
        }

        return Denied(serverContext, $"Actor has {actorAccess} access but {command.TypeName}.{command.MethodName} requires {requiredAccess}");
    }

    private static IAuthorizationCalculation Denied(IBamServerContext serverContext, string message)
    {
        return new AuthorizationCalculation(serverContext, BamAccess.Denied)
        {
            Messages = new[] { message }
        };
    }
}
