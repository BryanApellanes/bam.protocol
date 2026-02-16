using Bam.Protocol.Data;

namespace Bam.Protocol.Server;

/// <summary>
/// Handles the actor resolution step during server context initialization.
/// </summary>
public class ActorResolverInitializationHandler: IBamServerContextInitializationHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActorResolverInitializationHandler"/> class.
    /// </summary>
    /// <param name="actorResolver">The actor resolver to use.</param>
    public ActorResolverInitializationHandler(IActorResolver actorResolver)
    {
        this.ActorResolver = actorResolver;
    }
    
    protected IActorResolver ActorResolver { get; set; }
    /// <summary>
    /// Handles the initialization step by resolving the actor from the current server context.
    /// </summary>
    /// <param name="initialization">The initialization context to process.</param>
    /// <returns>The updated initialization context.</returns>
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