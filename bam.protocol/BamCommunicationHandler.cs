using Bam.Protocol.Server;

namespace Bam.Protocol;

public class BamCommunicationHandler : IBamCommunicationHandler
{
    public BamCommunicationHandler(
        ITcpIPAddressProvider tcpIpAddressProvider,
        IUdpIPAddressProvider udpIpAddressProvider,
        IBamRequestReader requestReader,
        IBamContextProvider contextProvider,
        IBamResponseProvider responseProvider,
        IBamActorResolver actorResolver,
        IBamSessionStateProvider sessionStateProvider,
        IBamAuthorizationCalculator authorizationCalculator,
        IBamRequestProcessor requestProcessor)
    {
        this.TcpIPAddressProvider = tcpIpAddressProvider;
        this.UdpIPAddressProvider = udpIpAddressProvider;
        this.RequestReader = requestReader;
        this.ContextProvider = contextProvider;
        this.ResponseProvider = responseProvider;
        this.ActorResolver = actorResolver;
        this.SessionStateProvider = sessionStateProvider;
        this.AuthorizationCalculator = authorizationCalculator;
        this.RequestProcessor = requestProcessor;
    }
    public ITcpIPAddressProvider? TcpIPAddressProvider { get; internal set; }
    public IUdpIPAddressProvider? UdpIPAddressProvider { get; internal set; }
    public IBamRequestReader? RequestReader { get; internal set; }
    public IBamContextProvider? ContextProvider { get; internal set; }
    public IBamResponseProvider? ResponseProvider { get; internal set; }
    public IBamActorResolver? ActorResolver { get; internal set; }
    public IBamSessionStateProvider? SessionStateProvider { get; internal set; }
    public IBamAuthorizationCalculator? AuthorizationCalculator { get; internal set; }
    public IBamRequestProcessor? RequestProcessor { get; internal set; }
}