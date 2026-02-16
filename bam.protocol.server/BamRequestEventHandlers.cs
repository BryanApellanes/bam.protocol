namespace Bam.Protocol.Server;

/// <summary>
/// Manages collections of event listeners for request processing lifecycle events.
/// </summary>
public class BamRequestEventHandlers
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamRequestEventHandlers"/> class.
    /// </summary>
    public BamRequestEventHandlers()
    {
        this.CreateContextStartedHandlers = new List<BamEventListener>();
        this.CreateContextCompleteHandlers = new List<BamEventListener>();
        this.ResolveUserStartedHandlers = new List<BamEventListener>();
        this.ResolveUserCompleteHandlers = new List<BamEventListener>();
        this.AuthorizeRequestStartedHandlers = new List<BamEventListener>();
        this.AuthorizeRequestCompleteHandlers = new List<BamEventListener>();
        this.ResolveSessionStateStartedHandlers = new List<BamEventListener>();
        this.ResolveSessionStateCompleteHandlers = new List<BamEventListener>();
        this.CreateResponseStartedHandlers = new List<BamEventListener>();
        this.CreateResponseCompleteHandlers = new List<BamEventListener>();
    }
    /// <summary>
    /// Gets the event listeners for when context creation starts.
    /// </summary>
    public List<BamEventListener> CreateContextStartedHandlers { get; }

    /// <summary>
    /// Gets the event listeners for when context creation completes.
    /// </summary>
    public List<BamEventListener> CreateContextCompleteHandlers { get; }

    /// <summary>
    /// Gets the event listeners for when user resolution starts.
    /// </summary>
    public List<BamEventListener> ResolveUserStartedHandlers { get; }

    /// <summary>
    /// Gets the event listeners for when user resolution completes.
    /// </summary>
    public List<BamEventListener> ResolveUserCompleteHandlers { get; }

    /// <summary>
    /// Gets the event listeners for when request authorization starts.
    /// </summary>
    public List<BamEventListener> AuthorizeRequestStartedHandlers { get; }

    /// <summary>
    /// Gets the event listeners for when request authorization completes.
    /// </summary>
    public List<BamEventListener> AuthorizeRequestCompleteHandlers { get; }

    /// <summary>
    /// Gets the event listeners for when session state resolution starts.
    /// </summary>
    public List<BamEventListener> ResolveSessionStateStartedHandlers { get; }

    /// <summary>
    /// Gets the event listeners for when session state resolution completes.
    /// </summary>
    public List<BamEventListener> ResolveSessionStateCompleteHandlers { get; }

    /// <summary>
    /// Gets the event listeners for when response creation starts.
    /// </summary>
    public List<BamEventListener> CreateResponseStartedHandlers { get; }

    /// <summary>
    /// Gets the event listeners for when response creation completes.
    /// </summary>
    public List<BamEventListener> CreateResponseCompleteHandlers { get; }

    /// <summary>
    /// Gets a value indicating whether any event handlers are registered.
    /// </summary>
    public bool HasHandlers =>
        CreateContextStartedHandlers.Count > 0 ||
        CreateContextCompleteHandlers.Count > 0 ||
        ResolveUserStartedHandlers.Count > 0 ||
        ResolveUserCompleteHandlers.Count > 0 ||
        AuthorizeRequestStartedHandlers.Count > 0 ||
        AuthorizeRequestCompleteHandlers.Count > 0 ||
        ResolveSessionStateStartedHandlers.Count > 0 ||
        ResolveSessionStateCompleteHandlers.Count > 0 ||
        CreateResponseStartedHandlers.Count > 0 ||
        CreateResponseCompleteHandlers.Count > 0;

    /// <summary>
    /// Subscribes all registered event listeners to the specified server instance.
    /// </summary>
    /// <param name="server">The server to subscribe event listeners to.</param>
    public void ListenTo(object server)
    {
        List<BamEventListener> allEventListeners = new List<BamEventListener>();
        allEventListeners.AddRange(CreateContextStartedHandlers);
        allEventListeners.AddRange(CreateContextCompleteHandlers);
        allEventListeners.AddRange(ResolveUserStartedHandlers);
        allEventListeners.AddRange(ResolveUserCompleteHandlers);
        allEventListeners.AddRange(AuthorizeRequestStartedHandlers);
        allEventListeners.AddRange(AuthorizeRequestCompleteHandlers);
        allEventListeners.AddRange(ResolveSessionStateStartedHandlers);
        allEventListeners.AddRange(ResolveSessionStateCompleteHandlers);
        allEventListeners.AddRange(CreateResponseStartedHandlers);
        allEventListeners.AddRange(CreateResponseCompleteHandlers);

        foreach (BamEventListener bamEventListener in allEventListeners)
        {
            bamEventListener.Listen(server);
        }
    }
}