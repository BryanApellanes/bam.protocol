using Bam.Protocol.Server;
using Bam.Server;

namespace Bam.Protocol.Client;

public abstract class BamClientRequest : IBamClientRequest
{
    public Dictionary<string, string> Headers { get; set; } = null!;
    public HostBinding Host { get; set; } = null!;
    public string Path { get; set; } = null!;
    public string QueryString { get; set; } = null!;
    public HttpMethods HttpMethod { get; set; }
    public string ProtocolVersion { get; set; } = null!;
    public string Protocol { get; set; } = null!;
    public object Content { get; set; } = null!;
    public virtual Uri GetUrl(IBamClient client)
    {
        TcpClientRequest copy = new TcpClientRequest();
        copy.CopyProperties(this);
        copy.Host = client.BaseAddress;
        return copy.GetUrl();
    }
    
    public Uri GetUrl()
    {
        string path = Path.StartsWith("/") ? Path : "/" + Path;
        string uri = $"{Host}{path}";
        if (!string.IsNullOrWhiteSpace(QueryString))
        {
            uri += "?" + QueryString;
        }
        return new Uri(uri);
    }

    public BamRequestLine GetRequestLine()
    {
        return new BamRequestLine($"{HttpMethod} {GetUrl()} {Protocol}/{ProtocolVersion}");
    }
}