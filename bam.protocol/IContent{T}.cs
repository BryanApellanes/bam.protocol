namespace Bam.Protocol;

public interface IContent<T> : IContent
{
    new  T Value { get; }
}