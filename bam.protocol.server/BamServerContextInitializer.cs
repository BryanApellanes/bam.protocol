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
    /// <param name="anonymousAccessInitializationHandler">The anonymous access initialization handler.</param>
    /// <param name="requestDecryptionInitializationHandler">The request decryption initialization handler.</param>
    /// <param name="anonymousActorInitializationHandler">The anonymous actor initialization handler.</param>
    /// <param name="sessionManager">The server session manager, used to cheaply detect whether the request carries a session id before command resolution runs.</param>
    public BamServerContextInitializer(ActorResolverInitializationHandler actorResolverInitializationHandler, AuthorizationCalculatorInitializationHandler authorizationCalculatorInitializationHandler,
        ServerSessionInitializationHandler serverSessionInitializationHandler, CommandInitializationHandler commandInitializationHandler,
        AuthenticationInitializationHandler authenticationInitializationHandler, AnonymousAccessInitializationHandler anonymousAccessInitializationHandler,
        RequestDecryptionInitializationHandler requestDecryptionInitializationHandler, AnonymousActorInitializationHandler anonymousActorInitializationHandler,
        IServerSessionManager sessionManager)
    {
        this.AuthorizationCalculatorInitializationHandlerInitializationHandler = authorizationCalculatorInitializationHandler;
        this.ActorResolverInitializationHandler = actorResolverInitializationHandler;
        this.ServerSessionInitializationHandler = serverSessionInitializationHandler;
        this.CommandInitializationHandler = commandInitializationHandler;
        this.AuthenticationInitializationHandler = authenticationInitializationHandler;
        this.AnonymousAccessInitializationHandler = anonymousAccessInitializationHandler;
        this.RequestDecryptionInitializationHandler = requestDecryptionInitializationHandler;
        this.AnonymousActorInitializationHandler = anonymousActorInitializationHandler;
        this.SessionManager = sessionManager;
    }
    
    protected HashSet<IBamServerContextInitializationHandler> BeforeInitializationHandlers { get; } = new HashSet<IBamServerContextInitializationHandler>();
    protected HashSet<IBamServerContextInitializationHandler> AfterInitializationHandlers { get; } = new HashSet<IBamServerContextInitializationHandler>();
    
    /// <summary>
    /// Occurs when session state resolution starts.
    /// </summary>
    public event EventHandler<BamServerEventArgs> ResolveSessionStateStarted = null!;

    /// <summary>
    /// Occurs when session state resolution completes.
    /// </summary>
    public event EventHandler<BamServerEventArgs> ResolveSessionStateComplete = null!;

    /// <summary>
    /// Occurs when actor resolution starts.
    /// </summary>
    public event EventHandler<BamServerEventArgs> ResolveActorStarted = null!;

    /// <summary>
    /// Occurs when actor resolution completes.
    /// </summary>
    public event EventHandler<BamServerEventArgs> ResolveActorComplete = null!;

    /// <summary>
    /// Occurs when request authentication starts.
    /// </summary>
    public event EventHandler<BamServerEventArgs> AuthenticateRequestStarted = null!;

    /// <summary>
    /// Occurs when request authentication completes.
    /// </summary>
    public event EventHandler<BamServerEventArgs> AuthenticateRequestComplete = null!;

    /// <summary>
    /// Occurs when command resolution starts.
    /// </summary>
    public event EventHandler<BamServerEventArgs> ResolveCommandStarted = null!;

    /// <summary>
    /// Occurs when command resolution completes.
    /// </summary>
    public event EventHandler<BamServerEventArgs> ResolveCommandComplete = null!;

    /// <summary>
    /// Occurs when request authorization starts.
    /// </summary>
    public event EventHandler<BamServerEventArgs> AuthorizeRequestStarted = null!;

    /// <summary>
    /// Occurs when request authorization completes.
    /// </summary>
    public event EventHandler<BamServerEventArgs> AuthorizeRequestComplete = null!;

    /// <summary>
    /// Occurs when the before-initialization phase starts.
    /// </summary>
    public event EventHandler<BamServerEventArgs> BeforeInitializationStarted = null!;

    /// <summary>
    /// Occurs when the before-initialization phase completes.
    /// </summary>
    public event EventHandler<BamServerEventArgs> BeforeInitializationComplete = null!;

    /// <summary>
    /// Occurs when the after-initialization phase starts.
    /// </summary>
    public event EventHandler<BamServerEventArgs> AfterInitializationStarted = null!;

    /// <summary>
    /// Occurs when the after-initialization phase completes.
    /// </summary>
    public event EventHandler<BamServerEventArgs> AfterInitializationComplete = null!;

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

            if (SessionManager.HasSessionId(serverContext.BamRequest))
            {
                // A session id was presented: resolve it (existing handler, unchanged, still
                // fires its events) and decrypt the body before any command resolution is
                // attempted, so IsAnonymousAccess is reliably determined for every transport.
                initialization = InitializeSession(initialization, args);
                if (!initialization.CanContinue)
                {
                    return initialization;
                }

                initialization = InitializeRequestDecryption(initialization, args);
            }

            initialization = InitializeCommand(initialization, args);
            if (!initialization.CanContinue)
            {
                return initialization;
            }

            initialization = InitializeAnonymousAccess(initialization, args);

            if (!initialization.IsAnonymousAccess)
            {
                // Full authenticated pipeline. InitializeSession is idempotent, so this is a
                // no-op re-entry when a session was already resolved above.
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
            }
            else
            {
                // Anonymous access (encrypted or not) — the body, if any, was already
                // decrypted above. Resolve to the well-known anonymous actor instead of
                // leaving context.Actor unset.
                initialization = InitializeAnonymousActor(initialization, args);
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
            FireEvent(InitializationException!, new InitializationExceptionEventArgs(ex, initialization));
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

    private BamServerInitializationContext InitializeAnonymousAccess(BamServerInitializationContext initialization,
        BamServerEventArgs args)
    {
        initialization = AnonymousAccessInitializationHandler.HandleInitialization(initialization);
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

    private BamServerInitializationContext InitializeRequestDecryption(BamServerInitializationContext initialization,
        BamServerEventArgs args)
    {
        initialization = RequestDecryptionInitializationHandler.HandleInitialization(initialization);
        return initialization;
    }

    private BamServerInitializationContext InitializeAnonymousActor(BamServerInitializationContext initialization,
        BamServerEventArgs args)
    {
        initialization = AnonymousActorInitializationHandler.HandleInitialization(initialization);
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

    protected AnonymousAccessInitializationHandler AnonymousAccessInitializationHandler
    {
        get;
        set;
    }

    protected AuthorizationCalculatorInitializationHandler AuthorizationCalculatorInitializationHandlerInitializationHandler
    {
        get;
        set;
    }

    protected RequestDecryptionInitializationHandler RequestDecryptionInitializationHandler
    {
        get;
        set;
    }

    protected AnonymousActorInitializationHandler AnonymousActorInitializationHandler
    {
        get;
        set;
    }

    protected IServerSessionManager SessionManager
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
            FireEvent(InitializationException!, new InitializationExceptionEventArgs(ex, initialization));
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
            FireEvent(InitializationException!, new InitializationExceptionEventArgs(ex, initialization));
        }
    }
}