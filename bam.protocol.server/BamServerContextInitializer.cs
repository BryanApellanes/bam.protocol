using Bam.Logging;

namespace Bam.Protocol.Server;

public class BamServerContextInitializer : Loggable, IBamServerContextInitializer
{
    public BamServerContextInitializer(ActorResolverInitializationHandler actorResolverInitializationHandler, AuthorizationCalculatorInitializationHandler authorizationCalculatorInitializationHandler,
        ServerSessionInitializationHandler serverSessionInitializationHandler, CommandInitializationHandler commandInitializationHandler)
    {
        this.AuthorizationCalculatorInitializationHandlerInitializationHandler = authorizationCalculatorInitializationHandler;
        this.ActorResolverInitializationHandler = actorResolverInitializationHandler;
        this.ServerSessionInitializationHandler = serverSessionInitializationHandler;
        this.CommandInitializationHandler = commandInitializationHandler;
    }
    
    protected HashSet<IBamServerContextInitializationHandler> BeforeInitializationHandlers { get; } = new HashSet<IBamServerContextInitializationHandler>();
    protected HashSet<IBamServerContextInitializationHandler> AfterInitializationHandlers { get; } = new HashSet<IBamServerContextInitializationHandler>();
    
    public event EventHandler<BamServerEventArgs> ResolveSessionStateStarted;
    public event EventHandler<BamServerEventArgs> ResolveSessionStateComplete;
    
    public event EventHandler<BamServerEventArgs> ResolveActorStarted;
    public event EventHandler<BamServerEventArgs> ResolveActorComplete;
    
    public event EventHandler<BamServerEventArgs> ResolveCommandStarted;
    public event EventHandler<BamServerEventArgs> ResolveCommandComplete;

    public event EventHandler<BamServerEventArgs> AuthorizeRequestStarted;
    public event EventHandler<BamServerEventArgs> AuthorizeRequestComplete;
    
    public event EventHandler<BamServerEventArgs> BeforeInitializationStarted;
    public event EventHandler<BamServerEventArgs> BeforeInitializationComplete;
    
    public event EventHandler<BamServerEventArgs> AfterInitializationStarted;
    public event EventHandler<BamServerEventArgs> AfterInitializationComplete;
    
    public event EventHandler<InitializationExceptionEventArgs>? InitializationException;
    
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

    public IBamServerContextInitializer AddBeforeInitializationHandler(IBamServerContextInitializationHandler handler)
    {
        BeforeInitializationHandlers.Add(handler);
        return this;
    }

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