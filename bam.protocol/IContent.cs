namespace Bam.Protocol;

/// <summary>
/// Defines untyped content with an object value.
/// </summary>
public interface IContent
{
    /// <summary>
    /// Gets the content value.
    /// </summary>
    object Value { get; }
}