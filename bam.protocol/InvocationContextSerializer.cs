namespace Bam.Protocol;

public class InvocationContextSerializer : IInvocationContextSerializer
{
    Dictionary<string, IInvocationContextSerializer> serializers = new Dictionary<string, IInvocationContextSerializer>();
    public InvocationContextSerializer()
    {
        this.SetFormat("json");
        this.serializers.Add("json", new JsonInvocationContextSerializer());
        this.serializers.Add("yaml", new YamlInvocationContextSerializer());
    }

    public void SetFormat(string format)
    {
        this.Format = format;
    }
    
    public string Format { get; private set; }
    public string Serialize(object context)
    {
        return this.serializers[Format].Serialize(context);
    }

    public object Deserialize(Type type, string serialization)
    {
        return this.serializers[Format].Deserialize(type, serialization);
    }
}