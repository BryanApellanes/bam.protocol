using System.Net;
using Bam.Data;
using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Logging;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Server.Dao.Repository;
using Bam.Protocol.Profile;
using Bam.Server;
using Bam.Services;

namespace Bam.Protocol.Server;

/// <summary>
/// Configuration options for a <see cref="BamServer"/>, including ports, IP addresses, service registrations, and event handlers.
/// </summary>
public class BamServerOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamServerOptions"/> class with default settings.
    /// </summary>
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
    
    /// <summary>
    /// Gets or sets the service registry for dependency injection.
    /// </summary>
    public ServiceRegistry ComponentRegistry { get; set; }

    /// <summary>
    /// Gets or sets the server event handlers.
    /// </summary>
    public BamServerEventHandlers ServerEventHandlers { get; set; } = null!;

    /// <summary>
    /// Gets or sets the request event handlers.
    /// </summary>
    public BamRequestEventHandlers RequestEventHandlers { get; set; } = null!;

    /// <summary>
    /// Gets or sets the HTTP host binding.
    /// </summary>
    public HostBinding HttpHostBinding { get; set; }

    /// <summary>
    /// Gets or sets the buffer size for reading requests.
    /// </summary>
    public int RequestBufferSize { get; set; }

    /// <summary>
    /// Gets or sets the logger for server operations.
    /// </summary>
    public ILogger? Logger { get; set; }

    private int _tcpPort;

    /// <summary>
    /// Gets or sets the HTTP port number.
    /// </summary>
    public int HttpPort
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the TCP port number. When UseNameBasedPort is true, the port is derived from the server name.
    /// </summary>
    public int TcpPort
    {
        get
        {
            if (_tcpPort <= 0 || UseNameBasedPort)
            {
                _tcpPort = ServerName.ToHashIntBetween(HashAlgorithms.SHA256, 1024, 65534);
            }

            return _tcpPort;
        }
        set => _tcpPort = value;
    }

    private IPAddress? _tcpIpAddress;
    /// <summary>
    /// Gets or sets the TCP IP address. Setting this also updates the TCP IP address provider in the component registry.
    /// </summary>
    public IPAddress? TcpIPAddress
    {
        get => _tcpIpAddress;
        set
        {
            _tcpIpAddress = value;
            ComponentRegistry.For<ITcpIPAddressProvider>().UseSingleton(new BamTcpIPAddressProvider(_tcpIpAddress!));
        }
    }

    private int _udpPort;
    /// <summary>
    /// Gets or sets the UDP port number. Defaults to TcpPort + 1 when not explicitly set or when UseNameBasedPort is true.
    /// </summary>
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

    private IPAddress _udpIpAddress = null!;
    /// <summary>
    /// Gets or sets the UDP IP address. Setting this also updates the UDP IP address provider in the component registry.
    /// </summary>
    public IPAddress UdpIPAddress
    {
        get => _udpIpAddress;
        set
        {
            _udpIpAddress = value;
            ComponentRegistry.For<IUdpIPAddressProvider>().UseSingleton(new BamUdpIPAddressProvider(_udpIpAddress));
        }
    }
    private string _serverName = null!;
    /// <summary>
    /// Gets or sets the server name for identification.
    /// </summary>
    public string ServerName
    {
        get => _serverName;
        set
        {
            _serverName = value;
            ComponentRegistry.For<IServerIdentity>().UseSingleton(new ServerIdentity(value));
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the Tcp and Udp ports should be deterministically derived from the name of the server.
    /// </summary>
    public bool UseNameBasedPort { get; set; }

    private IDatabase? _sessionDatabase;
    /// <summary>
    /// Gets or sets the session database. Setting this creates and registers a <see cref="ServerSessionSchemaRepository"/> singleton in the component registry.
    /// </summary>
    public IDatabase? SessionDatabase
    {
        get => _sessionDatabase;
        set
        {
            _sessionDatabase = value;
            if (value != null)
            {
                var repo = new ServerSessionSchemaRepository { Database = value };
                repo.Initialize();
                ComponentRegistry.For<ServerSessionSchemaRepository>().UseSingleton(repo);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the HTTP listener should be started by <see cref="BamServer"/>.
    /// Default is true. Set to false when sharing options with a <see cref="WebApplicationBamServer"/> to avoid port conflicts.
    /// </summary>
    public bool EnableHttpListener { get; set; } = true;

    protected void Initialize()
    {
        ServerEventHandlers = new BamServerEventHandlers();
        RequestEventHandlers = new BamRequestEventHandlers();
        ComponentRegistry.AddEncryptedProfileRepository("./.bam/profile");
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
            .For<IGroupAccessConfiguration>().UseSingleton(new GroupAccessConfiguration())
            .For<IAccessLevelProvider>().Use<GroupAccessLevelProvider>()
            .For<IAuthorizationCalculator>().Use<AuthorizationCalculator>()
            .For<ICommandResolver>().Use<CommandResolver>()
            .For<IBamRequestProcessor>().Use<BamRequestProcessor>()
            .For<IProfileManager>().Use<ProfileManager>()
            .For<IAuthenticator>().Use<BamAuthenticator>()
            .For<RequestSecurityValidator>().Use<RequestSecurityValidator>()
            .For<IAccountRepository>().Use<SessionSchemaAccountRepository>()
            .For<IAccountManager>().Use<AccountManager>();

        ComponentRegistry
            .For<ServiceRegistry>().UseSingleton(ComponentRegistry);

        ComponentRegistry
            .For<IBamServerContextInitializer>().Use<BamServerContextInitializer>();
    }

    /// <summary>
    /// Subscribes both server and request event handlers to the specified server instance.
    /// </summary>
    /// <param name="server">The server to subscribe event handlers to.</param>
    public void SubscribeEventHandlers(object server)
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

    /// <summary>
    /// Subscribes server lifecycle event handlers to the specified server instance.
    /// </summary>
    /// <param name="server">The server to subscribe server event handlers to.</param>
    public void SubscribeServerEventHandlers(object server)
    {
        ServerEventHandlers.ListenTo(server);
    }

    /// <summary>
    /// Subscribes request processing event handlers to the specified server instance.
    /// </summary>
    /// <param name="server">The server to subscribe request event handlers to.</param>
    public void SubscribeRequestEventHandlers(object server)
    {
        RequestEventHandlers.ListenTo(server);
    }
    
    private ICommunicationHandler? _communicationHandler;
    /// <summary>
    /// Gets the communication handler, resolving it from the component registry if not already initialized.
    /// </summary>
    /// <param name="reinit">If true, forces re-resolution of the communication handler.</param>
    /// <returns>The communication handler instance, or null if not resolved.</returns>
    public virtual ICommunicationHandler? GetCommunicationHandler(bool reinit = false)
    {
        if (_communicationHandler == null || reinit)
        {
            _communicationHandler = ComponentRegistry.Get<CommunicationHandler>();
        }

        return _communicationHandler;
    }

    /// <summary>
    /// Configures group access settings using the provided configuration action.
    /// </summary>
    /// <param name="configure">The action to configure group access.</param>
    /// <returns>This instance for fluent chaining.</returns>
    public BamServerOptions ConfigureGroupAccess(Action<IGroupAccessConfiguration> configure)
    {
        IGroupAccessConfiguration config = ComponentRegistry.Get<IGroupAccessConfiguration>();
        configure(config);
        return this;
    }

    /// <summary>
    /// Gets the server context initializer from the component registry.
    /// </summary>
    /// <returns>The server context initializer.</returns>
    public IBamServerContextInitializer GetServerContextInitializer()
    {
        return ComponentRegistry.Get<IBamServerContextInitializer>();
    }
}