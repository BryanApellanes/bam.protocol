using System.Net;
using System.Net.Sockets;

namespace Bam.Protocol.Server;

public class BamServerEventArgs : EventArgs
{
    public BamServerEventArgs()
    {
    }

    public BamServerEventArgs(IBamServerContext serverContext)
    {
        this.ServerContext = serverContext;
    }

    public BamServerEventArgs(HttpListenerContext context, IBamServerContext serverContext = null)
    {
        this.ServerContext = serverContext;
        this.HttpContext = context;
    }
    
    public BamServerEventArgs(TcpClient client, IBamServerContext serverContext = null)
    {
        this.LocalEndpoint = client?.Client?.LocalEndPoint?.ToString();
        this.RemoteEndpoint = client?.Client?.RemoteEndPoint?.ToString();
        this.ServerContext = serverContext;
    }

    public string ServerName => Server?.ServerName;
    public BamServer Server { get; set; }
    public byte[] UdpData { get; set; }
    public HttpListenerContext HttpContext { get; set; }
    public IBamServerContext ServerContext { get; set; }
    public string? LocalEndpoint { get; private set; }
    public string? RemoteEndpoint { get; private set; }
}