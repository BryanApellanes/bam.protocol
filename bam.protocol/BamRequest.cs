using System.Net;
using System.Text;

namespace Bam.Protocol.Server;

/// <summary>
/// Represents a server-side HTTP request, implementing <see cref="IBamRequest"/>.
/// </summary>
public class BamRequest : IBamRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamRequest"/> class.
    /// </summary>
    public BamRequest()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BamRequest"/> class from the specified request line.
    /// </summary>
    /// <param name="line">The parsed request line containing the method, URI, and protocol version.</param>
    public BamRequest(BamRequestLine line)
    {
        this.Line = line;
        this.HttpMethod = line.Method;
        this.Url = new Uri(line.RequestUri);
        this.ProtocolVersion = line.ProtocolVersion;
    }

    /// <summary>
    /// Gets or sets the parsed request line.
    /// </summary>
    public BamRequestLine Line { get; set; } = null!;

    /// <inheritdoc />
    public string ProtocolVersion { get; internal set; } = null!;

    /// <inheritdoc />
    public string Content { get; set; } = null!;

    /// <inheritdoc />
    public string[] AcceptTypes { get; set; } = null!;

    /// <inheritdoc />
    public Encoding ContentEncoding { get; set; } = null!;

    /// <inheritdoc />
    public long ContentLength64 { get; internal set;}

    /// <inheritdoc />
    public Dictionary<string, string> QueryString { get; internal set;} = null!;

    /// <inheritdoc />
    public string ContentType { get; internal set;} = null!;

    /// <inheritdoc />
    public CookieCollection Cookies { get; internal set;} = null!;

    /// <inheritdoc />
    public Dictionary<string, string> Headers { get; set; } = null!;

    /// <inheritdoc />
    public HttpMethods HttpMethod { get; internal set; }

    /// <inheritdoc />
    public Uri Url { get; internal set; } = null!;

    /// <inheritdoc />
    public Uri UrlReferrer => Headers.ContainsKey("referer") ? new Uri(Headers["referer"]) : null!;

    /// <inheritdoc />
    public string UserAgent => Headers.ContainsKey("user-agent") ? Headers["user-agent"] : string.Empty;

    /// <inheritdoc />
    public string UserHostAddress { get; internal set;} = null!;

    /// <inheritdoc />
    public string UserHostName { get; internal set;} = null!;

    /// <inheritdoc />
    public string[] UserLanguages { get; internal set;} = null!;

    /// <inheritdoc />
    public string RawUrl { get; internal set;} = null!;
}