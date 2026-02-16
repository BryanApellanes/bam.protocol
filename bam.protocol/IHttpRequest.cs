using Bam.Web;
using System.Text;

namespace Bam.Protocol
{
    /// <summary>
    /// Defines a client-side HTTP request with URI, headers, content, verb, and encoding.
    /// </summary>
    public interface IHttpRequest
    {
        /// <summary>
        /// Gets or sets the request URI.
        /// </summary>
        Uri Uri { get; set; }

        /// <summary>
        /// Gets the collection of request headers.
        /// </summary>
        IDictionary<string, string> Headers { get; }

        /// <summary>
        /// Gets or sets the MIME content type.
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the HTTP verb (GET, POST, etc.).
        /// </summary>
        HttpVerbs Verb { get; set; }

        /// <summary>
        /// Gets or sets the request body content as a string.
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// Gets or sets the character encoding for the request content.
        /// </summary>
        Encoding Encoding { get; set; }

        /// <summary>
        /// Converts this request to an <see cref="HttpRequestMessage"/> with the specified URL.
        /// </summary>
        /// <param name="url">The URL to use.</param>
        /// <returns>An <see cref="HttpRequestMessage"/>.</returns>
        HttpRequestMessage ToHttpRequestMessage(string url);

        /// <summary>
        /// Converts this request to an <see cref="HttpRequestMessage"/> using the current URI.
        /// </summary>
        /// <returns>An <see cref="HttpRequestMessage"/>.</returns>
        HttpRequestMessage ToHttpRequestMessage();
    }
}
