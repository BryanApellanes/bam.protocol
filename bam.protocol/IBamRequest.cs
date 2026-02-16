/*
	Copyright © Bryan Apellanes 2022  
*/

using System.Text;
using System.Net;

namespace Bam.Protocol.Server
{
    /// <summary>
    /// Defines the properties of a server-side HTTP request.
    /// </summary>
    public interface IBamRequest
    {
        /// <summary>
        /// Gets the protocol version string (e.g., "HTTP/1.1").
        /// </summary>
        string ProtocolVersion { get; }

        /// <summary>
        /// Gets or sets the request body content.
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// Gets or sets the MIME types accepted by the client.
        /// </summary>
        string[] AcceptTypes { get; set; }

        /// <summary>
        /// Gets or sets the encoding of the request content.
        /// </summary>
        Encoding ContentEncoding { get; set; }
        
        /// <summary>
        /// Gets the length of the body data included in the request.
        /// </summary>
        long ContentLength64 { get; }

        /// <summary>
        /// Gets the query string parameters.
        /// </summary>
        Dictionary<string, string> QueryString { get; }

        /// <summary>
        /// Gets the MIME type of the body data included in the request.
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Gets the cookies sent with the request.
        /// </summary>
        CookieCollection Cookies { get; }

        /// <summary>
        /// Gets the collection of header name/value pairs sent in the request.
        /// </summary>
        Dictionary<string, string> Headers { get; }
        
        /// <summary>
        /// Gets the HTTP method specified by the client.
        /// </summary>
        HttpMethods HttpMethod { get; }

        /// <summary>
        /// Gets the URI requested by the client.
        /// </summary>
        Uri Url { get; }

        /// <summary>
        /// Gets the URI of the resource that referred the client to the server.
        /// </summary>
        Uri UrlReferrer { get; }

        /// <summary>
        /// Gets the user agent string presented by the client.
        /// </summary>
        string UserAgent { get; }

        /// <summary>
        /// Gets the server IP address and port number to which the request is directed.
        /// </summary>
        string UserHostAddress { get; }

        /// <summary>
        /// Gets the DNS name and, if provided, the port number specified by the client.
        /// </summary>
        string UserHostName { get; }

        /// <summary>
        /// Gets the natural languages preferred for the response.
        /// </summary>
        string[] UserLanguages { get; }

        /// <summary>
        /// Gets the raw URL information (without the host and port) requested by the client.
        /// </summary>
        string RawUrl { get; }
    }
}
