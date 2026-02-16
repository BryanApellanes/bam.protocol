namespace Bam.Protocol;

/// <summary>
/// Specifies the transport protocol type for a request.
/// </summary>
public enum RequestType
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