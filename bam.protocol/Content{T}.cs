namespace Bam.Protocol;

/// <summary>
/// Represents strongly-typed content with implicit conversion to and from the content type.
/// </summary>
/// <typeparam name="T">The type of the content value.</typeparam>
public class Content<T> : Content, IContent<T>
{
    /// <summary>
    /// Implicitly converts a <see cref="Content{T}"/> to its underlying value.
    /// </summary>
    /// <param name="content">The content to convert.</param>
    public static implicit operator T(Content<T> content)
    {
        return content.Value;
    }

    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="T"/> to a <see cref="Content{T}"/>.
    /// </summary>
    /// <param name="content">The value to wrap in content.</param>
    public static implicit operator Content<T>(T content)
    {
        return new Content<T>(content);
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Content{T}"/> class with the specified value.
    /// </summary>
    /// <param name="value">The content value.</param>
    public Content(T value)
    {
        this.Value = value;
    }
    
    object IContent.Value => Value;

    /// <summary>
    /// Gets the strongly-typed content value.
    /// </summary>
    public T Value { get; }
}