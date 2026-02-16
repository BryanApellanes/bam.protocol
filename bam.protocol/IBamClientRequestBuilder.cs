using Bam.Server;

namespace Bam.Protocol.Client;

/// <summary>
/// Fluent builder for constructing <see cref="IBamClientRequest"/> instances.
/// </summary>
public interface IBamClientRequestBuilder
{
    /// <summary>
    /// Sets the base address from a host string.
    /// </summary>
    /// <param name="host">The host string.</param>
    /// <returns>This builder for chaining.</returns>
    IBamClientRequestBuilder BaseAddress(string host);

    /// <summary>
    /// Sets the base address from a <see cref="HostBinding"/>.
    /// </summary>
    /// <param name="hostBinding">The host binding.</param>
    /// <returns>This builder for chaining.</returns>
    IBamClientRequestBuilder BaseAddress(HostBinding hostBinding);

    /// <summary>
    /// Sets the request path.
    /// </summary>
    /// <param name="path">The request path.</param>
    /// <returns>This builder for chaining.</returns>
    IBamClientRequestBuilder Path(string path);

    /// <summary>
    /// Sets the query string from a collection of key-value pairs.
    /// </summary>
    /// <param name="queryString">The query string parameters.</param>
    /// <returns>This builder for chaining.</returns>
    IBamClientRequestBuilder QueryString(IEnumerable<KeyValuePair<string, object>> queryString);

    /// <summary>
    /// Sets the query string from key-value pairs.
    /// </summary>
    /// <param name="queryString">The query string parameters.</param>
    /// <returns>This builder for chaining.</returns>
    IBamClientRequestBuilder QueryString(params KeyValuePair<string, object>[] queryString);

    /// <summary>
    /// Sets the HTTP method.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <returns>This builder for chaining.</returns>
    IBamClientRequestBuilder HttpMethod(HttpMethods method);

    /// <summary>
    /// Sets the request body content.
    /// </summary>
    /// <param name="content">The request body content.</param>
    /// <returns>This builder for chaining.</returns>
    IBamClientRequestBuilder Content(object content);

    /// <summary>
    /// Builds and returns the configured <see cref="IBamClientRequest"/>.
    /// </summary>
    /// <returns>The constructed client request.</returns>
    IBamClientRequest Build();
}