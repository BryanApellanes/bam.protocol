using System.Net;
using System.Net.Sockets;
using Bam.Logging;

namespace Bam.Protocol.Server;

/// <summary>
/// Defines a component that reads and parses incoming BAM requests from various sources.
/// </summary>
public interface IBamRequestReader
{
    /// <summary>
    /// Occurs when a request read operation starts.
    /// </summary>
    public event EventHandler ReadRequestStarted;

    /// <summary>
    /// Reads a BAM request from an HTTP listener request.
    /// </summary>
    /// <param name="request">The HTTP listener request to read from.</param>
    /// <returns>The parsed BAM request.</returns>
    IBamRequest ReadRequest(HttpListenerRequest request);

    /// <summary>
    /// Reads a BAM request from a TCP client connection.
    /// </summary>
    /// <param name="client">The TCP client to read from.</param>
    /// <returns>The parsed BAM request.</returns>
    IBamRequest ReadRequest(TcpClient client);

    /// <summary>
    /// Reads a BAM request from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <returns>The parsed BAM request.</returns>
    IBamRequest ReadRequest(Stream stream);
}