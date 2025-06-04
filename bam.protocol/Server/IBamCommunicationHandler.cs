namespace Bam.Protocol.Server;

public interface IBamCommunicationHandler
{
    ITcpIPAddressProvider? TcpIpAddressProvider { get; }
    IUdpIPAddressProvider? UdpIpAddressProvider { get; }
    IBamContextProvider? ContextProvider { get; }
    IBamResponseProvider? ResponseProvider { get; }
    IBamActorResolver? ActorResolver { get; }
    IBamSessionStateProvider? SessionStateProvider { get; }
    IBamAuthorizationCalculator? AuthorizationCalculator { get; }
    IBamRequestProcessor? RequestProcessor { get; }
}