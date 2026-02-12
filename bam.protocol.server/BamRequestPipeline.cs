namespace Bam.Protocol.Server;

public class BamRequestPipeline
{
    private static readonly Dictionary<RequestType, Func<BamServerInitializationContext>> Instantiators = new()
    {
        { RequestType.Http, () => new HttpBamServerInitializationContext() },
        { RequestType.Tcp, () => new TcpBamServerInitializationContext() },
        { RequestType.Udp, () => new UdpBamServerInitializationContext() }
    };

    private readonly BamServerOptions _options;

    public BamRequestPipeline(BamServerOptions options)
    {
        _options = options;
    }

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
