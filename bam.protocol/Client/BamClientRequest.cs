using Bam.Protocol.Server;
using Bam.Server;

namespace Bam.Protocol.Client;

public abstract class BamClientRequest : IBamClientRequest
{
    public Dictionary<string, string> Headers { get; set; }
    public HostBinding Host { get; set; }
    public string Path { get; set; }
    public string QueryString { get; set; }
    public HttpMethods HttpMethod { get; set; }
    public string ProtocolVersion { get; set; }
    public string Protocol { get; set; }
    public object? Content { get; set; }
    public Uri GetUrl(IBamClient client)
    {
        TcpClientRequest copy = new TcpClientRequest();
        copy.CopyProperties(this);
        copy.Host = client.BaseAddress;
        return copy.GetUrl();
    }
    
    public Uri GetUrl()
    {
        return new Uri($"{Host}{Path}?{QueryString}");
    }

    public BamRequestLine GetRequestLine()
    {
        return new BamRequestLine($"{HttpMethod} {GetUrl()} {Protocol}/{ProtocolVersion}");
    }
}