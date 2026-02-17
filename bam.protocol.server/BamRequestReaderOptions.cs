namespace Bam.Protocol.Server;

/// <summary>
/// Configuration options for a <see cref="BamRequestReader"/>.
/// </summary>
public class BamRequestReaderOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamRequestReaderOptions"/> class from server options.
    /// </summary>
    /// <param name="serverOptions">The server options to derive request reader settings from.</param>
    public BamRequestReaderOptions(BamServerOptions serverOptions)
    {
        this.RequestBufferSize = serverOptions.RequestBufferSize;
    }
    
    /// <summary>
    /// Gets or sets the buffer size for reading request data.
    /// </summary>
    public int RequestBufferSize { get; set; }

    /// <summary>
    /// Gets or sets the event handler that is subscribed to all the events of a `RequestReader`.
    /// </summary>
    public EventHandler<BamRequestReaderEventArgs> EventHandler { get; set; } = null!;
}