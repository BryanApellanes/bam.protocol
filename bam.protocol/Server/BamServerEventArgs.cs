using System;
using System.Net.Sockets;
using Bam;

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
    
    public BamServerEventArgs(TcpClient client, IBamServerContext serverContext = null)
    {
        this.LocalEndpoint = client?.Client?.LocalEndPoint?.ToString();
        this.RemoteEndpoint = client?.Client?.RemoteEndPoint?.ToString();
        this.ServerContext = serverContext;
    }

    public BamServer Server { get; set; }
    public IBamServerContext ServerContext { get; internal set; }
    public string LocalEndpoint { get; private set; }
    public string RemoteEndpoint { get; private set; }
}