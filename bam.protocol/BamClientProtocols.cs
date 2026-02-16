namespace Bam.Protocol.Client;

/// <summary>
/// Specifies the transport protocol used by a Bam client.
/// </summary>
public enum BamClientProtocols
{
    /// <summary>
    /// HTTP protocol.
    /// </summary>
    Http,
    /// <summary>
    /// TCP protocol.
    /// </summary>
    Tcp,
    /// <summary>
    /// UDP protocol.
    /// </summary>
    Udp
}