using Bam.Logging;

namespace Bam.Protocol.Server;

/// <summary>
/// Orchestrates the server context initialization pipeline, running session, actor, authentication, command, and authorization handlers in sequence.
/// </summary>
public class BamServerContextInitializer : Loggable, IBamServerContextInitializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamServerContextInitializer"/> class with the required initialization handlers.
    /// </summary>
    /// <param name="actorResolverInitializationHandler">The actor resolver initialization handler.</param>
    /// <param name="authorizationCalculatorInitializationHandler">The authorization calculator initialization handler.</param>
    /// <param name="serverSessionInitializationHandler">The server session initialization handler.</param>
    /// <param name="commandInitializationHandler">The command initialization handler.</param>
    /// <param name="authenticationInitializationHandler">The authentication initialization handler.</param>
    public BamServerContextInitializer(ActorResolverInitializationHandler actorResolverInitializationHandler, AuthorizationCalculatorInitializationHandler authorizationCalculatorInitializationHandler,
        ServerSessionInitializationHandler serverSessionInitializationHandler, CommandInitializationHandler commandInitializationHandler,
        AuthenticationInitializationHandler authenticationInitializationHandler)
    {
        this.AuthorizationCalculatorInitializationHandlerInitializationHandler = authorizationCalculatorInitializationHandler;
        this.ActorResolverInitializationHandler = actorResolverInitializationHandler;
        this.ServerSessionInitializationHandler = serverSessionInitializationHandler;
        this.CommandInitializationHandler = commandInitializationHandler;
        this.AuthenticationInitializationHandler = authenticationInitializationHandler;
    }
    
    protected HashSet<IBamServerContextInitializationHandler> BeforeInitializationHandlers { get; } = new HashSet<IBamServerContextInitializationHandler>();
    protected HashSet<IBamServerContextInitializationHandler> AfterInitializationHandlers { get; } = new HashSet<IBamServerContextInitializationHandler>();
    
    /// <summary>
    /// Occurs when session state resolution starts.
    /// </summary>
    public event EventHandler<BamServerEventArgs> ResolveSessionStateStarted;

    /// <summary>
    /// Occurs when session state resolution completes.
    /// </summary>
    public event EventHandler<BamServerEventArgs> ResolveSessionStateComplete;

    /// <summary>
    /// Occurs when actor resolution starts.
    /// </summary>
    public event EventHandler<BamServerEventArgs> ResolveActorStarted;

    /// <summary>
    /// Occurs when actor resolution completes.
    /// </summary>
    public event EventHandler<BamServerEventArgs> ResolveActorComplete;

    /// <summary>
    /// Occurs when request authentication starts.
    /// </summary>
    public event EventHandler<BamServerEventArgs> AuthenticateRequestStarted;

    /// <summary>
    /// Occurs when request authentication completes.
    /// </summary>
    public event EventHandler<BamServerEventArgs> AuthenticateRequestComplete;

    /// <summary>
    /// Occurs when command resolution starts.
    /// </summary>
    public event EventHandler<BamServerEventArgs> ResolveCommandStarted;

    /// <summary>
    /// Occurs when command resolution completes.
    /// </summary>
    public event EventHandler<BamServerEventArgs> ResolveCommandComplete;

    /// <summary>
    /// Occurs when request authorization starts.
    /// </summary>
    public event EventHandler<BamServerEventArgs> AuthorizeRequestStarted;

    /// <summary>
    /// Occurs when request authorization completes.
    /// </summary>
    public event EventHandler<BamServerEventArgs> AuthorizeRequestComplete;

    /// <summary>
    /// Occurs when the before-initialization phase starts.
    /// </summary>
    public event EventHandler<BamServerEventArgs> BeforeInitializationStarted;

    /// <summary>
    /// Occurs when the before-initialization phase completes.
    /// </summary>
    public event EventHandler<BamServerEventArgs> BeforeInitializationComplete;

    /// <summary>
    /// Occurs when the after-initialization phase starts.
    /// </summary>
    public event EventHandler<BamServerEventArgs> AfterInitializationStarted;

    /// <summary>
    /// Occurs when the after-initialization phase completes.
    /// </summary>
    public event EventHandler<BamServerEventArgs> AfterInitializationComplete;

    /// <summary>
    /// Occurs when an exception is thrown during initialization.
    /// </summary>
    public event EventHandler<InitializationExceptionEventArgs>? InitializationException;
    
    /// <summary>
    /// Runs the full initialization pipeline for the specified server context.
    /// </summary>
    /// <param name="initialization">The initialization context to process.</param>
    /// <returns>The fully processed initialization context.</returns>
    public BamServerInitializationContext InitializeServerContext(BamServerInitializationContext initialization)
    {
        IBamServerContext serverContext = initialization.ServerContext;
        try
        {
            BamServerEventArgs args = initialization.EventArgs;

            OnBeforeInitialization(initialization, args);
            
            initialization = InitializeSession(initialization, args);
            if (!initialization.CanContinue)
            {
                return initialization;
            }
            
            initialization = InitializeActor(initialization, args);
            if (!initialization.CanContinue)
            {
                return initialization;
            }

            initialization = InitializeAuthentication(initialization, args);
            if (!initialization.CanContinue)
            {
                return initialization;
            }

            initialization = InitializeCommand(initialization, args);
            if (!initialization.CanContinue)
            {
                return initialization;
            }
            
            initialization = InitializeAuthorization(initialization, args);
            if (!initialization.CanContinue)
            {
                return initialization;
            }

            initialization.Status = InitializationStatus.Success;

            OnAfterInitialization(initialization, args);
        }
        catch (Exception ex)
        {
            FireEvent(InitializationException, new InitializationExceptionEventArgs(ex, initialization));
            serverContext.SetInitializationException(ex);
        }
        return initialization;
    }

    private BamServerInitializationContext InitializeAuthorization(BamServerInitializationContext initialization,
        BamServerEventArgs args)
    {
        FireEvent(AuthorizeRequestStarted, args);
        initialization = AuthorizationCalculatorInitializationHandlerInitializationHandler.HandleInitialization(initialization);
        FireEvent(AuthorizeRequestComplete, args);
        return initialization;
    }

    private BamServerInitializationContext InitializeAuthentication(BamServerInitializationContext initialization,
        BamServerEventArgs args)
    {
        FireEvent(AuthenticateRequestStarted, args);
        initialization = AuthenticationInitializationHandler.HandleInitialization(initialization);
        FireEvent(AuthenticateRequestComplete, args);
        return initialization;
    }

    private BamServerInitializationContext InitializeCommand(BamServerInitializationContext initialization,
        BamServerEventArgs args)
    {
        FireEvent(ResolveCommandStarted, args);
        initialization = CommandInitializationHandler.HandleInitialization(initialization);
        FireEvent(ResolveCommandComplete, args);
        return initialization;
    }

    private BamServerInitializationContext InitializeActor(BamServerInitializationContext initialization,
        BamServerEventArgs args)
    {
        FireEvent(ResolveActorStarted, args);
        initialization = ActorResolverInitializationHandler.HandleInitialization(initialization);
        FireEvent(ResolveActorComplete, args);
        return initialization;
    }

    private BamServerInitializationContext InitializeSession(BamServerInitializationContext initialization,
        BamServerEventArgs args)
    {
        FireEvent(ResolveSessionStateStarted, args);
        initialization = ServerSessionInitializationHandler.HandleInitialization(initialization);
        FireEvent(ResolveSessionStateComplete, args);
        return initialization;
    }

    /// <summary>
    /// Adds a handler to run before the main initialization steps.
    /// </summary>
    /// <param name="handler">The handler to add.</param>
    /// <returns>This initializer for fluent chaining.</returns>
    public IBamServerContextInitializer AddBeforeInitializationHandler(IBamServerContextInitializationHandler handler)
    {
        BeforeInitializationHandlers.Add(handler);
        return this;
    }

    /// <summary>
    /// Adds a handler to run after the main initialization steps.
    /// </summary>
    /// <param name="handler">The handler to add.</param>
    /// <returns>This initializer for fluent chaining.</returns>
    public IBamServerContextInitializer AddAfterInitializationHandler(IBamServerContextInitializationHandler handler)
    {
        AfterInitializationHandlers.Add(handler);
        return this;
    }
    
    protected ServerSessionInitializationHandler ServerSessionInitializationHandler
    {
        get;
        set;
    }
    
    protected ActorResolverInitializationHandler ActorResolverInitializationHandler
    {
        get;
        set;
    }

    protected CommandInitializationHandler CommandInitializationHandler
    {
        get;
        set;
    }
    
    protected AuthenticationInitializationHandler AuthenticationInitializationHandler
    {
        get;
        set;
    }

    protected AuthorizationCalculatorInitializationHandler AuthorizationCalculatorInitializationHandlerInitializationHandler
    {
        get;
        set;
    }

    
    protected void OnBeforeInitialization(BamServerInitializationContext initialization, BamServerEventArgs args)
    {
        try
        {
            FireEvent(BeforeInitializationStarted, args);
            if (!initialization.CanContinue)
            {
                return;
            }
            foreach (IBamServerContextInitializationHandler handler in BeforeInitializationHandlers)
            {
                if (!handler.HandleInitialization(initialization).CanContinue)
                {
                    break;
                }
            }
            FireEvent(BeforeInitializationComplete, args);
        }
        catch (Exception ex)
        {
            FireEvent(InitializationException, new InitializationExceptionEventArgs(ex, initialization));
        }
    }

    protected void OnAfterInitialization(BamServerInitializationContext initialization, BamServerEventArgs args)
    {
        try
        {
            FireEvent(AfterInitializationStarted, args);
            if (!initialization.CanContinue)
            {
                return;
            }
            foreach (IBamServerContextInitializationHandler handler in AfterInitializationHandlers)
            {
                if (!handler.HandleInitialization(initialization).CanContinue)
                {
                    break;
                }
            }
            FireEvent(AfterInitializationComplete, args);
        }
        catch (Exception ex)
        {
            FireEvent(InitializationException, new InitializationExceptionEventArgs(ex, initialization));
        }
    }
}