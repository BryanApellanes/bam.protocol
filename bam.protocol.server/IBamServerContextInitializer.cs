namespace Bam.Protocol.Server;

/// <summary>
/// Defines the orchestrator that runs the server context initialization pipeline, raising events at each step.
/// </summary>
public interface IBamServerContextInitializer
{
    /// <summary>
    /// Occurs when the before-initialization phase starts.
    /// </summary>
    event EventHandler<BamServerEventArgs> BeforeInitializationStarted;

    /// <summary>
    /// Occurs when the before-initialization phase completes.
    /// </summary>
    event EventHandler<BamServerEventArgs> BeforeInitializationComplete;

    /// <summary>
    /// Occurs when the after-initialization phase starts.
    /// </summary>
    event EventHandler<BamServerEventArgs> AfterInitializationStarted;

    /// <summary>
    /// Occurs when the after-initialization phase completes.
    /// </summary>
    event EventHandler<BamServerEventArgs> AfterInitializationComplete;

    /// <summary>
    /// Occurs when session state resolution starts.
    /// </summary>
    event EventHandler<BamServerEventArgs> ResolveSessionStateStarted;

    /// <summary>
    /// Occurs when session state resolution completes.
    /// </summary>
    event EventHandler<BamServerEventArgs> ResolveSessionStateComplete;

    /// <summary>
    /// Occurs when actor resolution starts.
    /// </summary>
    event EventHandler<BamServerEventArgs> ResolveActorStarted;

    /// <summary>
    /// Occurs when actor resolution completes.
    /// </summary>
    event EventHandler<BamServerEventArgs> ResolveActorComplete;

    /// <summary>
    /// Occurs when command resolution starts.
    /// </summary>
    event EventHandler<BamServerEventArgs> ResolveCommandStarted;

    /// <summary>
    /// Occurs when command resolution completes.
    /// </summary>
    event EventHandler<BamServerEventArgs> ResolveCommandComplete;

    /// <summary>
    /// Occurs when request authorization starts.
    /// </summary>
    event EventHandler<BamServerEventArgs> AuthorizeRequestStarted;

    /// <summary>
    /// Occurs when request authorization completes.
    /// </summary>
    event EventHandler<BamServerEventArgs> AuthorizeRequestComplete;

    /// <summary>
    /// Occurs when an exception is thrown during initialization.
    /// </summary>
    event EventHandler<InitializationExceptionEventArgs> InitializationException;

    /// <summary>
    /// Runs the full initialization pipeline for the specified server context.
    /// </summary>
    /// <param name="initializationContext">The initialization context to process.</param>
    /// <returns>The fully processed initialization context.</returns>
    BamServerInitializationContext InitializeServerContext(BamServerInitializationContext initializationContext);

    /// <summary>
    /// Adds a handler to run before the main initialization steps.
    /// </summary>
    /// <param name="handler">The handler to add.</param>
    /// <returns>This initializer for fluent chaining.</returns>
    IBamServerContextInitializer AddBeforeInitializationHandler(IBamServerContextInitializationHandler handler);

    /// <summary>
    /// Adds a handler to run after the main initialization steps.
    /// </summary>
    /// <param name="handler">The handler to add.</param>
    /// <returns>This initializer for fluent chaining.</returns>
    IBamServerContextInitializer AddAfterInitializationHandler(IBamServerContextInitializationHandler handler);
}