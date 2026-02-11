using Bam.Protocol.Data;

namespace Bam.Protocol.Server;

public class ActorResolverInitializationHandler: IBamServerContextInitializationHandler
{
    public ActorResolverInitializationHandler(IActorResolver actorResolver)
    {
        this.ActorResolver = actorResolver;
    }
    
    protected IActorResolver ActorResolver { get; set; }
    public BamServerInitializationContext HandleInitialization(BamServerInitializationContext initialization)
    {
        IBamServerContext context = initialization.ServerContext;
        IActor actor = ActorResolver.ResolveActor(context);
        if (actor == null)
        {
            initialization.CanContinue = false;
            initialization.Status = InitializationStatus.ActorResolutionFailed;
        }

        context.SetActor(actor);
        return initialization;
    }
}