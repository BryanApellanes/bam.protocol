namespace Bam.Protocol.Server;

/// <summary>
/// Defines the aggregate of all communication components required by a BAM server.
/// </summary>
public interface ICommunicationHandler
{
    /// <summary>
    /// Gets the TCP IP address provider.
    /// </summary>
    ITcpIPAddressProvider? TcpIpAddressProvider { get; }

    /// <summary>
    /// Gets the UDP IP address provider.
    /// </summary>
    IUdpIPAddressProvider? UdpIpAddressProvider { get; }

    /// <summary>
    /// Gets the server context provider.
    /// </summary>
    IBamServerContextProvider? ServerContextProvider { get; }

    /// <summary>
    /// Gets the response provider.
    /// </summary>
    IBamResponseProvider? ResponseProvider { get; }

    /// <summary>
    /// Gets the actor resolver.
    /// </summary>
    IActorResolver? ActorResolver { get; }

    /// <summary>
    /// Gets the server session manager.
    /// </summary>
    IServerSessionManager? ServerSessionManager { get; }

    /// <summary>
    /// Gets the authorization calculator.
    /// </summary>
    IAuthorizationCalculator? AuthorizationCalculator { get; }

    /// <summary>
    /// Gets the request processor.
    /// </summary>
    IBamRequestProcessor? RequestProcessor { get; }

    /// <summary>
    /// Gets the object encoder/decoder for serialization.
    /// </summary>
    IObjectEncoderDecoder? ObjectEncoderDecoder { get; }
}