namespace Bam.Protocol.Server;

public class CommandInitializationHandler: IBamServerContextInitializationHandler
{
    public CommandInitializationHandler(ICommandResolver commandResolver)
    {
        this.CommandResolver = commandResolver;
    }
    protected ICommandResolver CommandResolver { get; set; }
    public BamServerContextInitialization HandleInitialization(BamServerContextInitialization initialization)
    {
        IBamServerContext context = initialization.ServerContext;
        ICommand command = CommandResolver.ResolveCommand(context.BamRequest);
        if (command == null)
        {
            initialization.CanContinue = false;
            initialization.Status = InitializationStatus.CommandResolutionFailed;
        }

        context.SetCommand(command);
        return initialization;
    }
}