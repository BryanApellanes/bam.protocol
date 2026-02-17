namespace Bam.Protocol;

/// <summary>
/// Represents an HTTP header name/value pair, with implicit conversion to string.
/// </summary>
public class BamHeaderValue
{
    /// <summary>
    /// Implicitly converts a <see cref="BamHeaderValue"/> to its string value.
    /// </summary>
    /// <param name="header">The header to convert.</param>
    public static implicit operator string(BamHeaderValue header)
    {
        return header.Value;
    }

    /// <summary>
    /// Gets or sets the name of the header.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the value of the header.
    /// </summary>
    public string Value { get; set; } = null!;
}