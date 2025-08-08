namespace Bam.Protocol;

public interface IInvocationContextSerializer
{
    string Format { get; }
    string Serialize(object context);
    object Deserialize(Type type, string serialization);
}