using System.Net;
using System.Net.Sockets;
using Bam.Logging;

namespace Bam.Protocol.Server;

/// <summary>
/// Creates server contexts by reading requests from various sources (HTTP, TCP, Stream).
/// </summary>
public class BamServerContextProvider : Loggable, IBamServerContextProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamServerContextProvider"/> class.
    /// </summary>
    /// <param name="requestReader">The request reader for parsing incoming requests.</param>
    /// <param name="responseProvider">The response provider for creating responses.</param>
    public BamServerContextProvider(IBamRequestReader requestReader, IBamResponseProvider responseProvider)
    {
        this.RequestReader = requestReader;
        this.ResponseProvider = responseProvider;
    }
    protected IBamRequestReader RequestReader { get; private set; }
    protected  IBamResponseProvider ResponseProvider { get; private set; }

    /// <summary>
    /// Creates a server context from an HTTP listener context.
    /// </summary>
    /// <param name="httpContext">The HTTP listener context.</param>
    /// <param name="requestId">The unique request identifier.</param>
    /// <returns>The created server context.</returns>
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

    /// <summary>
    /// Creates a server context from a TCP client connection.
    /// </summary>
    /// <param name="client">The TCP client.</param>
    /// <param name="requestId">The unique request identifier.</param>
    /// <returns>The created server context.</returns>
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
    
    /// <summary>
    /// Creates a server context from a stream.
    /// </summary>
    /// <param name="stream">The stream to read the request from.</param>
    /// <param name="requestId">The unique request identifier.</param>
    /// <returns>The created server context.</returns>
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