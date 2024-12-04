using System;
using Bam.Server;
using Bam.Protocol.Server;

namespace Bam.Protocol.Client;

public class UdpClientRequest : BamClientRequest
{
    public UdpClientRequest(string content)
    {
        Protocol = "BAM";
        ProtocolVersion = "2.0";
        HttpMethod = HttpMethods.PUT;
        Content = content;
    }

    public UdpClientRequest(object content)
    {
        Protocol = "BAM";
        ProtocolVersion = "2.0";
        HttpMethod = HttpMethods.PUT;
        Content = content;
    }
}