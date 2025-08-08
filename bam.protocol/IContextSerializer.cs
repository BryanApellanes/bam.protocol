namespace Bam.Protocol;

public interface IContextSerializer
{
    string Format { get; }
    string Serialize(object data);
}