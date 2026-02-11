using System.Net;
using System.Net.Sockets;
using Bam.Logging;

namespace Bam.Protocol.Server;

public class BamServerContextProvider : Loggable, IBamServerContextProvider
{
    public BamServerContextProvider(IBamRequestReader requestReader, IBamResponseProvider responseProvider)
    {
        this.RequestReader = requestReader;
        this.ResponseProvider = responseProvider;
    }
    protected IBamRequestReader RequestReader { get; private set; }
    protected  IBamResponseProvider ResponseProvider { get; private set; }

    public IBamServerContext CreateServerContext(HttpListenerContext httpContext, string requestId)
    {
        IBamRequest request = RequestReader.ReadRequest(httpContext.Request);
        return new BamServerContext
        {
            HttpContext = httpContext,
            RequestId = requestId,
            BamRequest = request,
            OutputStream = httpContext.Response.OutputStream,
        };
    }

    public IBamServerContext CreateServerContext(TcpClient client, string requestId)
    {
        IBamRequest request = RequestReader.ReadRequest(client);
        return new BamServerContext
        {
            RequestId = requestId,
            BamRequest = request,
            OutputStream = client.GetStream(),
        };
    }
    
    public IBamServerContext CreateServerContext(Stream stream, string requestId)
    {
        IBamRequest request = RequestReader.ReadRequest(stream);
        return new BamServerContext
        {
            RequestId = requestId,
            BamRequest = request,
            OutputStream = stream,
        };
    }
}