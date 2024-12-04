using System;
using Bam.Server;
using Bam.Protocol.Server;

namespace Bam.Protocol.Client;

public class HttpClientRequest : BamClientRequest
{
    public HttpClientRequest()
    {
        Protocol = "HTTP";
        ProtocolVersion = "1.1";
        HttpMethod = HttpMethods.GET;
    }

    public HttpClientRequest(string content) : this()
    {
        Content = content;
    }

    public HttpClientRequest(BamClientRequestOptions options)
    {
        this.Host = options.Host;
    }

}