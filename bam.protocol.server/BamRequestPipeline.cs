namespace Bam.Protocol.Server;

/// <summary>
/// Runs the server context initialization pipeline for incoming requests, creating protocol-specific initialization contexts.
/// </summary>
public class BamRequestPipeline
{
    private static readonly Dictionary<RequestType, Func<BamServerInitializationContext>> Instantiators = new()
    {
        { RequestType.Http, () => new HttpBamServerInitializationContext() },
        { RequestType.Tcp, () => new TcpBamServerInitializationContext() },
        { RequestType.Udp, () => new UdpBamServerInitializationContext() }
    };

    private readonly BamServerOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="BamRequestPipeline"/> class with the specified server options.
    /// </summary>
    /// <param name="options">The server options providing the context initializer.</param>
    public BamRequestPipeline(BamServerOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Runs the initialization pipeline for the specified server context.
    /// </summary>
    /// <param name="serverContext">The server context to initialize.</param>
    /// <param name="args">The event arguments for this request.</param>
    /// <param name="server">The optional server instance.</param>
    /// <returns>The completed initialization context.</returns>
    public BamServerInitializationContext RunPipeline(
        IBamServerContext serverContext,
        BamServerEventArgs args,
        BamServer? server = null)
    {
        BamServerInitializationContext initialization = Instantiators[serverContext.RequestType]();
        initialization.Server = server;
        initialization.ServerContext = serverContext;
        initialization.EventArgs = args;

        IBamServerContextInitializer initializer = _options.GetServerContextInitializer();
        return initializer.InitializeServerContext(initialization);
    }
}
