namespace Bam.Protocol.Server;

/// <summary>
/// Manages collections of event listeners for server lifecycle events.
/// </summary>
public class BamServerEventHandlers
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamServerEventHandlers"/> class.
    /// </summary>
    public BamServerEventHandlers()
    {
        this.StartingHandlers = new List<BamEventListener>();
        this.StartedHandlers = new List<BamEventListener>();
        this.StoppingHandlers = new List<BamEventListener>();
        this.StoppedHandlers = new List<BamEventListener>();
        this.TcpClientConnectedHandlers = new List<BamEventListener>();
        this.UdpDataReceivedHandlers = new List<BamEventListener>();
    }
    /// <summary>
    /// Gets the event listeners for the Starting event.
    /// </summary>
    public List<BamEventListener> StartingHandlers { get; }

    /// <summary>
    /// Gets the event listeners for the Started event.
    /// </summary>
    public List<BamEventListener> StartedHandlers { get; }

    /// <summary>
    /// Gets the event listeners for the Stopping event.
    /// </summary>
    public List<BamEventListener> StoppingHandlers { get; }

    /// <summary>
    /// Gets the event listeners for the Stopped event.
    /// </summary>
    public List<BamEventListener> StoppedHandlers { get; }

    /// <summary>
    /// Gets the event listeners for the TcpClientConnected event.
    /// </summary>
    public List<BamEventListener> TcpClientConnectedHandlers { get; }

    /// <summary>
    /// Gets the event listeners for the UdpDataReceived event.
    /// </summary>
    public List<BamEventListener> UdpDataReceivedHandlers { get; }

    /// <summary>
    /// Gets a value indicating whether any event handlers are registered.
    /// </summary>
    public bool HasHandlers =>
        StartingHandlers.Count > 0 || StartedHandlers.Count > 0 || StoppingHandlers.Count > 0 ||
        StoppedHandlers.Count > 0 || TcpClientConnectedHandlers.Count > 0 || UdpDataReceivedHandlers.Count > 0;

    /// <summary>
    /// Subscribes all registered event listeners to the specified server instance.
    /// </summary>
    /// <param name="server">The server to subscribe event listeners to.</param>
    public void ListenTo(object server)
    {
        List<BamEventListener> allEventHandlers = new List<BamEventListener>();
        allEventHandlers.AddRange(StartingHandlers);
        allEventHandlers.AddRange(StartedHandlers);
        allEventHandlers.AddRange(StoppingHandlers);
        allEventHandlers.AddRange(StoppedHandlers);
        allEventHandlers.AddRange(TcpClientConnectedHandlers);
        allEventHandlers.AddRange(UdpDataReceivedHandlers);

        foreach (BamEventListener bamEventListener in allEventHandlers)
        {
            bamEventListener.Listen(server);
        }
    }
}