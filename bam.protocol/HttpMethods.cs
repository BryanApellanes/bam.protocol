namespace Bam.Protocol;

/// <summary>
/// Enumerates the standard HTTP request methods.
/// </summary>
public enum HttpMethods
{
    /// <summary>
    /// HTTP GET method for retrieving resources.
    /// </summary>
    GET,
    /// <summary>
    /// HTTP POST method for submitting data.
    /// </summary>
    POST,
    /// <summary>
    /// HTTP PUT method for replacing a resource.
    /// </summary>
    PUT,
    /// <summary>
    /// HTTP PATCH method for partial resource updates.
    /// </summary>
    PATCH,
    /// <summary>
    /// HTTP DELETE method for removing a resource.
    /// </summary>
    DELETE,
    /// <summary>
    /// HTTP HEAD method for retrieving headers only.
    /// </summary>
    HEAD,
    /// <summary>
    /// HTTP OPTIONS method for querying supported methods.
    /// </summary>
    OPTIONS,
    /// <summary>
    /// HTTP TRACE method for diagnostic loop-back testing.
    /// </summary>
    TRACE,
}