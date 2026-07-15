namespace Bam.Protocol.Server;

/// <summary>
/// Handles actor assignment for anonymous-access requests during server context initialization.
/// </summary>
public class AnonymousActorInitializationHandler : IBamServerContextInitializationHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnonymousActorInitializationHandler"/> class.
    /// </summary>
    /// <param name="anonymousActorProvider">The provider of the well-known anonymous actor.</param>
    public AnonymousActorInitializationHandler(IAnonymousActorProvider anonymousActorProvider)
    {
        this.AnonymousActorProvider = anonymousActorProvider;
    }

    protected IAnonymousActorProvider AnonymousActorProvider { get; set; }

    /// <summary>
    /// Handles the initialization step by assigning the well-known anonymous actor to the current server context.
    /// </summary>
    /// <param name="initialization">The initialization context to process.</param>
    /// <returns>The updated initialization context.</returns>
    public BamServerInitializationContext HandleInitialization(BamServerInitializationContext initialization)
    {
        IBamServerContext context = initialization.ServerContext;
        context.SetActor(AnonymousActorProvider.GetAnonymousActor());
        return initialization;
    }
}
