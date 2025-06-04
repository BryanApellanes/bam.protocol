using Bam.Protocol.Server;
using Bam.Server;

namespace Bam.Protocol.Client;

public class HttpBamClientRequestBuilder : BamClientRequestBuilder
{
    public HttpBamClientRequestBuilder()
    {
        this._options.Host = new HostBinding() { Port = BamServer.DefaultHttpPort };
    }
    public override IBamClientRequest Build()
    {
        return new HttpClientRequest()
        {
            Host = _options.Host,
            Path = _options.Path,
            QueryString = _options.GetQueryString(),
            HttpMethod = _options.Method,
            Content = _options.Content
        };
    }
}