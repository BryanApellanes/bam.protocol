namespace Bam.Protocol.Server;

/// <summary>
/// Represents the first line of a BAM protocol response, containing the protocol version, status code, and description.
/// </summary>
public class BamResponseLine
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamResponseLine"/> class from a BAM request.
    /// </summary>
    /// <param name="request">The request to derive the protocol version from.</param>
    public BamResponseLine(BamRequest request): this(request.Line)
    {
    }

    protected BamResponseLine(BamRequestLine line)
    {
        this.ProtocolVersion = line.ProtocolVersion;
        this.StatusCode = 404;
    }
    
    /// <summary>
    /// Gets or sets the protocol version string.
    /// </summary>
    public string ProtocolVersion { get; set; }

    /// <summary>
    /// Gets or sets the numeric status code of the response.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Gets the human-readable description for the current status code.
    /// </summary>
    public string StatusDescription => BamStatusCodes.GetDescription(StatusCode);

    /// <summary>
    /// Returns the string representation of this response line.
    /// </summary>
    /// <returns>A string in the format "ProtocolVersion StatusCode StatusDescription".</returns>
    public override string ToString()
    {
        return $"{ProtocolVersion} {StatusCode} {StatusDescription}";
    }
}