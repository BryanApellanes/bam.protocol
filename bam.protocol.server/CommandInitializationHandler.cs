namespace Bam.Protocol.Server;

/// <summary>
/// Handles the command resolution step during server context initialization.
/// </summary>
public class CommandInitializationHandler: IBamServerContextInitializationHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandInitializationHandler"/> class.
    /// </summary>
    /// <param name="commandResolver">The command resolver to use.</param>
    public CommandInitializationHandler(ICommandResolver commandResolver)
    {
        this.CommandResolver = commandResolver;
    }
    protected ICommandResolver CommandResolver { get; set; }
    /// <summary>
    /// Handles the initialization step by resolving the command from the current request.
    /// </summary>
    /// <param name="initialization">The initialization context to process.</param>
    /// <returns>The updated initialization context.</returns>
    public BamServerInitializationContext HandleInitialization(BamServerInitializationContext initialization)
    {
        IBamServerContext context = initialization.ServerContext;
        ICommand command = CommandResolver.ResolveCommand(context.BamRequest);
        if (command == null)
        {
            initialization.CanContinue = false;
            initialization.Status = InitializationStatus.CommandResolutionFailed;
        }

        context.SetCommand(command!);
        return initialization;
    }
}