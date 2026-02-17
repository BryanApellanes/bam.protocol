using Bam.Server;

namespace Bam.Protocol.Server;

/// <summary>
/// Contains informational properties describing a BAM server instance.
/// </summary>
public class BamServerInfo
{
    /// <summary>
    /// Gets or sets the server name.
    /// </summary>
    public string ServerName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the TCP port number.
    /// </summary>
    public int TcpPort { get; internal set; }

    /// <summary>
    /// Gets or sets the UDP port number.
    /// </summary>
    public int UdpPort { get; internal set; }

    /// <summary>
    /// Gets or sets the TCP IP address as a string.
    /// </summary>
    public string TcpIPAddress { get; internal set; } = null!;

    /// <summary>
    /// Gets or sets the UDP IP address as a string.
    /// </summary>
    public string UdpIPAddress { get; internal set; } = null!;

    /// <summary>
    /// Gets or sets the HTTP host binding for this server.
    /// </summary>
    public HostBinding HttpHostBinding
    {
        get;
        set;
    } = null!;
}