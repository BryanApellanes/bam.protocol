using System.Net;
using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Logging;
using Bam.Protocol.Profile;
using Bam.Server;
using Bam.Services;

namespace Bam.Protocol.Server;

public class BamServerOptions
{
    public BamServerOptions()
    {
        this.ComponentRegistry = new ServiceRegistry();
        this.Initialize();
        this.RequestBufferSize = 5000;
        this.Logger = Log.Default;
        this.HttpPort = BamServer.DefaultHttpPort;
        this.TcpPort = BamServer.DefaultTcpPort;
        this.TcpIPAddress = IPAddress.Any;
        this.UdpPort = BamServer.DefaultUdpPort;
        this.UdpIPAddress = IPAddress.Any;
        this.ServerName = 6.RandomLetters();
        this.HttpHostBinding = new HostBinding(HttpPort);
    }
    
    public ServiceRegistry ComponentRegistry { get; set; }
    public BamServerEventHandlers ServerEventHandlers { get; set; }
    public BamRequestEventHandlers RequestEventHandlers { get; set; }
    public HostBinding HttpHostBinding { get; set; }
    
    public int RequestBufferSize { get; set; }
    
    public ILogger? Logger { get; set; }

    private int _tcpPort;

    public int HttpPort
    {
        get;
        set;
    }
    public int TcpPort
    {
        get
        {
            if (_tcpPort <= 0 || UseNameBasedPort)
            {
                _tcpPort = ServerName.ToHashIntBetween(HashAlgorithms.SHA256, 1024, 65535);
            }

            return _tcpPort;
        }
        set => _tcpPort = value;
    }

    private IPAddress? _tcpIpAddress;
    public IPAddress? TcpIPAddress
    {
        get => _tcpIpAddress;
        set
        {
            _tcpIpAddress = value;
            ComponentRegistry.For<ITcpIPAddressProvider>().UseSingleton(new BamTcpIPAddressProvider(_tcpIpAddress));
        }
    }

    private int _udpPort;
    public int UdpPort
    {
        get
        {
            if (_udpPort <= 0 || UseNameBasedPort)
            {
                _udpPort = TcpPort + 1;
            }

            return _udpPort;
        }
        set => _udpPort = value;
    }

    private IPAddress _udpIpAddress;
    public IPAddress UdpIPAddress
    {
        get => _udpIpAddress;
        set
        {
            _udpIpAddress = value;
            ComponentRegistry.For<IUdpIPAddressProvider>().UseSingleton(new BamUdpIPAddressProvider(_udpIpAddress));
        }
    }
    public string ServerName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Tcp and Udp ports should be deterministically derived from the name of the server.
    /// </summary>
    public bool UseNameBasedPort { get; set; }
    
    protected void Initialize()
    {
        ServerEventHandlers = new BamServerEventHandlers();
        RequestEventHandlers = new BamRequestEventHandlers();
        ComponentRegistry
            .For<ITcpIPAddressProvider>().UseSingleton(new BamTcpIPAddressProvider(IPAddress.Any))
            .For<IUdpIPAddressProvider>().UseSingleton(new BamUdpIPAddressProvider(IPAddress.Any))
            .For<IObjectEncoderDecoder>().UseSingleton<JsonObjectDataEncoder>()
            .For<IBamRequestReader>().Use<BamRequestReader>()
            .For<ISignatureProvider>().Use<RsaSignatureProvider>()
            .For<INonceProvider>().Use<NonceProvider>()
            .For<IKeyManager>().Use<KeyManager>()
            .For<IBamServerContextProvider>().Use<BamServerContextProvider>()
            .For<IBamResponseProvider>().Use<DefaultBamResponseProvider>()
            .For<IActorResolver>().Use<ActorResolver>()
            .For<IServerSessionManager>().Use<ServerSessionManager>()
            .For<IAuthorizationCalculator>().Use<AuthorizationCalculator>()
            .For<ICommandResolver>().Use<CommandResolver>()
            .For<IBamRequestProcessor>().Use<BamRequestProcessor>();
        
        ComponentRegistry
            .For<IBamServerContextInitializer>().UseSingleton(ComponentRegistry.Get<BamServerContextInitializer>());
    }

    internal void SubscribeEventHandlers(BamServer server)
    {
        if (ServerEventHandlers.HasHandlers)
        {
            SubscribeServerEventHandlers(server);
        }

        if (RequestEventHandlers.HasHandlers)
        {
            SubscribeRequestEventHandlers(server);
        }
    }

    internal void SubscribeServerEventHandlers(BamServer server)
    {
        ServerEventHandlers.ListenTo(server);
    }

    internal void SubscribeRequestEventHandlers(BamServer server)
    {
        RequestEventHandlers.ListenTo(server);
    }
    
    private ICommunicationHandler? _communicationHandler;
    public virtual ICommunicationHandler? GetCommunicationHandler(bool reinit = false)
    {
        if (_communicationHandler == null || reinit)
        {
            _communicationHandler = ComponentRegistry.Get<CommunicationHandler>();
        }

        return _communicationHandler;
    }

    public IBamServerContextInitializer GetServerContextInitializer()
    {
        return ComponentRegistry.Get<IBamServerContextInitializer>();
    }
}