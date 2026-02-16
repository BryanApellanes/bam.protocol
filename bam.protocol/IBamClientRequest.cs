using Bam.Server;
using Bam.Protocol.Server;

namespace Bam.Protocol.Client;

/// <summary>
/// Defines a client-side request to be sent to a Bam server.
/// </summary>
public interface IBamClientRequest
{
    /// <summary>
    /// Gets or sets the host binding for the request.
    /// </summary>
    HostBinding Host { get; set; }

    /// <summary>
    /// Gets or sets the request path.
    /// </summary>
    string Path { get; set; }

    /// <summary>
    /// Gets or sets the query string portion of the URL.
    /// </summary>
    string QueryString { get; set; }

    /// <summary>
    /// Gets or sets the HTTP method for the request.
    /// </summary>
    HttpMethods HttpMethod { get; set; }

    /// <summary>
    /// Gets or sets the protocol version (e.g., "HTTP/1.1").
    /// </summary>
    string ProtocolVersion { get; set; }

    /// <summary>
    /// Gets or sets the protocol identifier (e.g., "http", "tcp").
    /// </summary>
    string Protocol { get; set; }

    /// <summary>
    /// Gets or sets the request body content.
    /// </summary>
    object Content { get; set; }

    /// <summary>
    /// Builds the full URL from the host, path, and query string.
    /// </summary>
    /// <returns>The fully constructed URI.</returns>
    Uri GetUrl();

    /// <summary>
    /// Builds the full URL using the specified client's base address.
    /// </summary>
    /// <param name="client">The client whose base address to use.</param>
    /// <returns>The fully constructed URI.</returns>
    Uri GetUrl(IBamClient client);

    /// <summary>
    /// Constructs a <see cref="BamRequestLine"/> from this request's method, path, and protocol version.
    /// </summary>
    /// <returns>A request line representing this request.</returns>
    BamRequestLine GetRequestLine();
}