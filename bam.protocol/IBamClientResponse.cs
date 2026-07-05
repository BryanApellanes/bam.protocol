namespace Bam.Protocol.Client;

/// <summary>
/// Represents a response received from a Bam server.
/// </summary>
public interface IBamClientResponse
{
    /// <summary>
    /// Gets the HTTP status code of the response.
    /// </summary>
    int StatusCode { get; }

    /// <summary>
    /// Gets the raw response content as a string. For HTTP this is the entity body; for the
    /// BAM wire protocol (TCP/UDP) this is the full raw response including the <c>BAM/2.0 {status}</c>
    /// status line and headers. Use <see cref="Body"/> to get the entity body regardless of transport.
    /// </summary>
    string Content { get; }

    /// <summary>
    /// Gets the entity body of the response, independent of transport framing. For HTTP this equals
    /// <see cref="Content"/>; for a raw BAM response it is the portion after the status line/headers
    /// (i.e. after the first blank line).
    /// </summary>
    string Body { get; }

    /// <summary>
    /// Applies authorization information from the specified response to this response.
    /// </summary>
    /// <param name="clientResponse">The response containing authorization information.</param>
    /// <returns>This response with authorization applied.</returns>
    IBamClientResponse Authorize(IBamClientResponse clientResponse);

    /// <summary>
    /// Gets the response headers.
    /// </summary>
    Dictionary<string, string> Headers { get; }
}