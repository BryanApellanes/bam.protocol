using System.Net;
using System.Net.Sockets;

namespace Bam.Protocol.Server;

/// <summary>
/// Provides event data for BAM server events, including context and endpoint information.
/// </summary>
public class BamServerEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamServerEventArgs"/> class.
    /// </summary>
    public BamServerEventArgs()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BamServerEventArgs"/> class with the specified server context.
    /// </summary>
    /// <param name="serverContext">The server context for this event.</param>
    public BamServerEventArgs(IBamServerContext serverContext)
    {
        this.ServerContext = serverContext;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BamServerEventArgs"/> class with an HTTP listener context.
    /// </summary>
    /// <param name="context">The HTTP listener context.</param>
    /// <param name="serverContext">An optional server context.</param>
    public BamServerEventArgs(HttpListenerContext context, IBamServerContext serverContext = null!)
    {
        this.ServerContext = serverContext;
        this.HttpContext = context;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="BamServerEventArgs"/> class with a TCP client.
    /// </summary>
    /// <param name="client">The TCP client.</param>
    /// <param name="serverContext">An optional server context.</param>
    public BamServerEventArgs(TcpClient client, IBamServerContext serverContext = null!)
    {
        this.LocalEndpoint = client?.Client?.LocalEndPoint?.ToString();
        this.RemoteEndpoint = client?.Client?.RemoteEndPoint?.ToString();
        this.ServerContext = serverContext;
    }

    /// <summary>
    /// Gets the server name from the associated server.
    /// </summary>
    public string ServerName => Server?.ServerName!;

    /// <summary>
    /// Gets or sets the BAM server that raised this event.
    /// </summary>
    public BamServer Server { get; set; } = null!;

    /// <summary>
    /// Gets or sets the raw UDP data received, if applicable.
    /// </summary>
    public byte[] UdpData { get; set; } = null!;

    /// <summary>
    /// Gets or sets the HTTP listener context, if applicable.
    /// </summary>
    public HttpListenerContext HttpContext { get; set; } = null!;

    /// <summary>
    /// Gets or sets the server context for this event.
    /// </summary>
    public IBamServerContext ServerContext { get; set; } = null!;

    /// <summary>
    /// Gets the local endpoint string of the TCP connection, if applicable.
    /// </summary>
    public string? LocalEndpoint { get; private set; }

    /// <summary>
    /// Gets the remote endpoint string of the TCP connection, if applicable.
    /// </summary>
    public string? RemoteEndpoint { get; private set; }
}