namespace Bam.Protocol;

public class YamlInvocationContextSerializer : IInvocationContextSerializer
{
    public YamlInvocationContextSerializer()
    {
        this.Format = "yaml";
        
    }
    public string Format { get; }
    public string Serialize(object context)
    {
        return context.ToYaml();
    }

    public object Deserialize(Type type, string serialization)
    {
        return serialization.FromYaml(type);
    }
}