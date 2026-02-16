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
    /// Gets the response body content as a string.
    /// </summary>
    string Content { get; }

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