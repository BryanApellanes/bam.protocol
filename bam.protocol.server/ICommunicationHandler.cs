namespace Bam.Protocol.Server;

public interface ICommunicationHandler
{
    ITcpIPAddressProvider? TcpIpAddressProvider { get; }
    IUdpIPAddressProvider? UdpIpAddressProvider { get; }
    IBamServerContextProvider? ServerContextProvider { get; }
    IBamResponseProvider? ResponseProvider { get; }
    IActorResolver? ActorResolver { get; }
    IServerSessionManager? ServerSessionManager { get; }
    IAuthorizationCalculator? AuthorizationCalculator { get; }
    IBamRequestProcessor? RequestProcessor { get; }
    IObjectEncoderDecoder? ObjectEncoderDecoder { get; }
}