using System.Text;

namespace Bam.Protocol.Server;

/// <summary>
/// Parses and represents the first line of an HTTP request (method, URI, and protocol version).
/// </summary>
public class BamRequestLine
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamRequestLine"/> class from raw byte data.
    /// </summary>
    /// <param name="requestLineData">The request line data as a byte array.</param>
    public BamRequestLine(byte[] requestLineData) :this(Encoding.ASCII.GetString(requestLineData))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BamRequestLine"/> class from a request line string.
    /// </summary>
    /// <param name="requestLine">The request line string (e.g., "GET /path HTTP/1.1").</param>
    public BamRequestLine(string requestLine)
    {
        this.Value = requestLine;
    }

    private readonly string _value = null!;
    /// <summary>
    /// Gets the raw request line string value.
    /// </summary>
    public string Value
    {
        get => _value;
        private init
        {
            _value = value;
            Parse();
        }
    }
    
    /// <summary>
    /// Gets the HTTP method parsed from the request line.
    /// </summary>
    public HttpMethods Method { get; private set; }

    /// <summary>
    /// Gets the request URI parsed from the request line.
    /// </summary>
    public string RequestUri { get; private set; } = null!;

    /// <summary>
    /// Gets the protocol version parsed from the request line (e.g., "HTTP/1.1").
    /// </summary>
    public string ProtocolVersion { get; private set; } = null!;

    public override string ToString()
    {
        return Value;
    }

    private void Parse()
    {
        string[] split = Value.DelimitSplit(" ");
        if (split.Length != 3)
        {
            throw new InvalidOperationException($"Unrecognized request line: {_value}");
        }

        Method = Enum.Parse<HttpMethods>(split[0]);
        RequestUri = split[1];
        ProtocolVersion = split[2];
    }
}