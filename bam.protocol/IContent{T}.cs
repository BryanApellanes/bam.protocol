namespace Bam.Protocol;

/// <summary>
/// Defines strongly-typed content.
/// </summary>
/// <typeparam name="T">The type of the content value.</typeparam>
public interface IContent<T> : IContent
{
    /// <summary>
    /// Gets the strongly-typed content value.
    /// </summary>
    new  T Value { get; }
}