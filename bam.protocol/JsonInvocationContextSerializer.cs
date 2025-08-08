namespace Bam.Protocol;

public class JsonInvocationContextSerializer : IInvocationContextSerializer
{
    public JsonInvocationContextSerializer()
    {
        this.Format = "json";
        
    }
    public string Format { get; }
    public string Serialize(object context)
    {
        return context.ToJson();
    }

    public object Deserialize(Type type, string serialization)
    {
        return serialization.FromJson(type);
    }
}