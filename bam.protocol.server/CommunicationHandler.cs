using Bam.Protocol.Server;

namespace Bam.Protocol;

public class CommunicationHandler : ICommunicationHandler
{
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
    
    public ITcpIPAddressProvider? TcpIpAddressProvider { get; internal set; }
    public IUdpIPAddressProvider? UdpIpAddressProvider { get; internal set; }
    public IBamRequestReader? RequestReader { get; internal set; }
    public IBamServerContextProvider? ServerContextProvider { get; internal set; }
    public IBamResponseProvider? ResponseProvider { get; internal set; }
    public IActorResolver? ActorResolver { get; internal set; }
    public IServerSessionManager? ServerSessionManager { get; internal set; }
    public IAuthorizationCalculator? AuthorizationCalculator { get; internal set; }
    public IBamRequestProcessor? RequestProcessor { get; internal set; }
    public IObjectEncoderDecoder? ObjectEncoderDecoder { get; }
}