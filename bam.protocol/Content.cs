namespace Bam.Protocol;

/// <summary>
/// Represents untyped content with an object value.
/// </summary>
public class Content : IContent
{
    /// <summary>
    /// Gets or sets the content value.
    /// </summary>
    public object Value { get; set; } = null!;
}