namespace Bam.Protocol.Server;

public interface IBamServerContextInitializer
{
    event EventHandler<BamServerEventArgs> BeforeInitializationStarted;
    event EventHandler<BamServerEventArgs> BeforeInitializationComplete;
    event EventHandler<BamServerEventArgs> AfterInitializationStarted;
    event EventHandler<BamServerEventArgs> AfterInitializationComplete;
    
    event EventHandler<BamServerEventArgs> ResolveSessionStateStarted;
    event EventHandler<BamServerEventArgs> ResolveSessionStateComplete;
    event EventHandler<BamServerEventArgs> ResolveActorStarted;
    event EventHandler<BamServerEventArgs> ResolveActorComplete;
    event EventHandler<BamServerEventArgs> ResolveCommandStarted;
    event EventHandler<BamServerEventArgs> ResolveCommandComplete;
    event EventHandler<BamServerEventArgs> AuthorizeRequestStarted;
    event EventHandler<BamServerEventArgs> AuthorizeRequestComplete;
    
    event EventHandler<InitializationExceptionEventArgs> InitializationException;
    
    BamServerContextInitialization InitializeServerContext(BamServerContextInitialization context);
    
    IBamServerContextInitializer AddBeforeInitializationHandler(IBamServerContextInitializationHandler handler);
    IBamServerContextInitializer AddAfterInitializationHandler(IBamServerContextInitializationHandler handler);
}