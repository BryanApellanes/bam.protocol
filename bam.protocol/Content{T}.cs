namespace Bam.Protocol;

public class Content<T> : Content, IContent<T>
{
    public static implicit operator T(Content<T> content)
    {
        return content.Value;
    }

    public static implicit operator Content<T>(T content)
    {
        return new Content<T>(content);
    }
    
    public Content(T value)
    {
        this.Value = value;
    }
    
    object IContent.Value => Value;

    public T Value { get; }
}