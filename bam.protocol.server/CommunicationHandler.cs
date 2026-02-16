using Bam.Protocol.Server;

namespace Bam.Protocol;

/// <summary>
/// Default implementation of <see cref="ICommunicationHandler"/> that aggregates all server communication components.
/// </summary>
public class CommunicationHandler : ICommunicationHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommunicationHandler"/> class with the specified communication components.
    /// </summary>
    /// <param name="tcpIpAddressProvider">The TCP IP address provider.</param>
    /// <param name="udpIpAddressProvider">The UDP IP address provider.</param>
    /// <param name="requestReader">The request reader.</param>
    /// <param name="serverContextProvider">The server context provider.</param>
    /// <param name="responseProvider">The response provider.</param>
    /// <param name="actorResolver">The actor resolver.</param>
    /// <param name="serverSessionManager">The session manager.</param>
    /// <param name="authorizationCalculator">The authorization calculator.</param>
    /// <param name="requestProcessor">The request processor.</param>
    /// <param name="objectEncoderDecoder">The object encoder/decoder.</param>
    public CommunicationHandler(
        ITcpIPAddressProvider tcpIpAddressProvider,
        IUdpIPAddressProvider udpIpAddressProvider,
        IBamRequestReader requestReader,
        IBamServerContextProvider serverContextProvider,
        IBamResponseProvider responseProvider,
        IActorResolver actorResolver,
        IServerSessionManager serverSessionManager,
        IAuthorizationCalculator authorizationCalculator,
        IBamRequestProcessor requestProcessor, 
        IObjectEncoderDecoder objectEncoderDecoder)
    {
        this.TcpIpAddressProvider = tcpIpAddressProvider;
        this.UdpIpAddressProvider = udpIpAddressProvider;
        this.RequestReader = requestReader;
        this.ServerContextProvider = serverContextProvider;
        this.ResponseProvider = responseProvider;
        this.ActorResolver = actorResolver;
        this.ServerSessionManager = serverSessionManager;
        this.AuthorizationCalculator = authorizationCalculator;
        this.RequestProcessor = requestProcessor;
        this.ObjectEncoderDecoder = objectEncoderDecoder;
    }
    
    /// <summary>
    /// Gets or sets the TCP IP address provider.
    /// </summary>
    public ITcpIPAddressProvider? TcpIpAddressProvider { get; internal set; }

    /// <summary>
    /// Gets or sets the UDP IP address provider.
    /// </summary>
    public IUdpIPAddressProvider? UdpIpAddressProvider { get; internal set; }

    /// <summary>
    /// Gets or sets the request reader used to parse incoming requests.
    /// </summary>
    public IBamRequestReader? RequestReader { get; internal set; }

    /// <summary>
    /// Gets or sets the server context provider.
    /// </summary>
    public IBamServerContextProvider? ServerContextProvider { get; internal set; }

    /// <summary>
    /// Gets or sets the response provider.
    /// </summary>
    public IBamResponseProvider? ResponseProvider { get; internal set; }

    /// <summary>
    /// Gets or sets the actor resolver for identifying request actors.
    /// </summary>
    public IActorResolver? ActorResolver { get; internal set; }

    /// <summary>
    /// Gets or sets the server session manager.
    /// </summary>
    public IServerSessionManager? ServerSessionManager { get; internal set; }

    /// <summary>
    /// Gets or sets the authorization calculator.
    /// </summary>
    public IAuthorizationCalculator? AuthorizationCalculator { get; internal set; }

    /// <summary>
    /// Gets or sets the request processor for executing commands.
    /// </summary>
    public IBamRequestProcessor? RequestProcessor { get; internal set; }

    /// <summary>
    /// Gets the object encoder/decoder for serialization.
    /// </summary>
    public IObjectEncoderDecoder? ObjectEncoderDecoder { get; }
}