using System;
using Bam.Server;
using Bam.Protocol.Server;

namespace Bam.Protocol.Client;

public class TcpClientRequest : BamClientRequest
{
    public TcpClientRequest()
    {
        Protocol = "BAM";
        ProtocolVersion = "2.0";
        HttpMethod = HttpMethods.GET;
    }

    public TcpClientRequest(string content) : this()
    {
        Content = content;
    }
}