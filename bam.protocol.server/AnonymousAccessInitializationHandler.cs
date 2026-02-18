namespace Bam.Protocol.Server;

/// <summary>
/// Handles the anonymous access check step during server context initialization.
/// Sets <see cref="BamServerInitializationContext.IsAnonymousAccess"/> if the resolved command allows anonymous access.
/// </summary>
public class AnonymousAccessInitializationHandler : IBamServerContextInitializationHandler
{
    /// <summary>
    /// Handles the initialization step by checking if the resolved command allows anonymous access.
    /// </summary>
    /// <param name="initialization">The initialization context to process.</param>
    /// <returns>The updated initialization context.</returns>
    public BamServerInitializationContext HandleInitialization(BamServerInitializationContext initialization)
    {
        IBamServerContext context = initialization.ServerContext;
        ICommand command = context.Command;
        if (command != null && CommandAttributeResolver.IsAnonymousAccessAllowed(command))
        {
            initialization.IsAnonymousAccess = true;
        }

        return initialization;
    }
}
