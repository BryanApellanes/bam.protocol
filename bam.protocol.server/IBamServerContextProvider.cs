using System.Net;
using System.Net.Sockets;

namespace Bam.Protocol.Server;

/// <summary>
/// Defines a factory that creates server contexts from various request sources.
/// </summary>
public interface IBamServerContextProvider
{
    /// <summary>
    /// Creates a server context from an HTTP listener context.
    /// </summary>
    /// <param name="httpContext">The HTTP listener context.</param>
    /// <param name="requestId">The unique request identifier.</param>
    /// <returns>The created server context.</returns>
    IBamServerContext CreateServerContext(HttpListenerContext httpContext, string requestId);

    /// <summary>
    /// Creates a server context from a TCP client connection.
    /// </summary>
    /// <param name="client">The TCP client.</param>
    /// <param name="requestId">The unique request identifier.</param>
    /// <returns>The created server context.</returns>
    IBamServerContext CreateServerContext(TcpClient client, string requestId);

    /// <summary>
    /// Creates a server context from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="requestId">The unique request identifier.</param>
    /// <returns>The created server context.</returns>
    IBamServerContext CreateServerContext(Stream stream, string requestId);
}