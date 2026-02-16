using System.Net;
using System.Text;

namespace Bam.Protocol.Server;

/// <summary>
/// Abstract base class for Bam HTTP responses, providing header, cookie, and status code management.
/// </summary>
public abstract class BamResponse : IBamResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamResponse"/> class with the specified output stream and status code.
    /// </summary>
    /// <param name="outputStream">The stream to write the response to.</param>
    /// <param name="statusCode">The HTTP status code, defaults to 404.</param>
    public BamResponse(Stream outputStream, int statusCode = 404)
    {
        OutputStream = outputStream;
        ContentEncoding = Encoding.UTF8;
        Cookies = new CookieCollection();
        Headers = new Dictionary<string, List<BamHeaderValue>>();
        StatusCode = statusCode;
    }
    /// <inheritdoc />
    public Encoding ContentEncoding { get; set; }
    /// <inheritdoc />
    public long ContentLength64 { get; set; }
    /// <inheritdoc />
    public string ContentType { get; set; }
    /// <inheritdoc />
    public CookieCollection Cookies { get; set; }
    /// <inheritdoc />
    public Dictionary<string, List<BamHeaderValue>> Headers { get; set; }
    /// <inheritdoc />
    public Stream OutputStream { get; }
    /// <inheritdoc />
    public string RedirectLocation { get; set; }
    /// <inheritdoc />
    public int StatusCode { get; set; }

    /// <summary>
    /// Sets a header value, replacing any existing values for the specified header name.
    /// </summary>
    /// <param name="name">The name of the header.</param>
    /// <param name="value">The value of the header.</param>
    public void SetHeader(string name, string value)
    {
        if (this.Headers.ContainsKey(name))
        {
            this.Headers[name] = new System.Collections.Generic.List<BamHeaderValue>();
        }
        
        this.AddHeader(name, value);
    }
    
    /// <inheritdoc />
    public void AddHeader(string name, string value)
    {
        if (!this.Headers.ContainsKey(name))
        {
            this.Headers.Add(name, new System.Collections.Generic.List<BamHeaderValue>());
        }

        this.Headers[name].Add(new BamHeaderValue { Name = name, Value = value });
    }

    /// <inheritdoc />
    public void AppendCookie(Cookie cookie)
    {
        this.Cookies.Add(cookie);
    }

    /// <inheritdoc />
    public void AppendHeader(string name, string value)
    {
        AddHeader(name, value);
    }

    /// <inheritdoc />
    public abstract void Send();

    /// <inheritdoc />
    public abstract void Send(byte[] responseEntity);

    /// <inheritdoc />
    public void Redirect(string url)
    {
        this.RedirectLocation = url;
        this.StatusCode = 302;
    }

    /// <inheritdoc />
    public void SetCookie(Cookie cookie)
    {
        Cookie existing = Cookies.FirstOrDefault(c => c.Name.Equals(cookie.Name));
        if (existing != null)
        {
            Cookies.Remove(existing);
        }

        Cookies.Add(cookie);
    }
}