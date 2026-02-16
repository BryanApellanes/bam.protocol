namespace Bam.Protocol;

/// <summary>
/// Serializes and deserializes invocation context objects, supporting JSON and YAML formats with a configurable default format.
/// </summary>
public class InvocationContextSerializer : IInvocationContextSerializer
{
    Dictionary<string, IInvocationContextSerializer> serializers = new Dictionary<string, IInvocationContextSerializer>();
    public InvocationContextSerializer()
    {
        this.SetFormat("json");
        this.serializers.Add("json", new JsonInvocationContextSerializer());
        this.serializers.Add("yaml", new YamlInvocationContextSerializer());
    }

    /// <summary>
    /// Sets the serialization format to use.
    /// </summary>
    /// <param name="format">The format identifier (e.g., "json", "yaml").</param>
    public void SetFormat(string format)
    {
        this.Format = format;
    }
    
    /// <inheritdoc />
    public string Format { get; private set; }

    /// <inheritdoc />
    public string Serialize(object context)
    {
        return this.serializers[Format].Serialize(context);
    }

    /// <inheritdoc />
    public object Deserialize(Type type, string serialization)
    {
        return this.serializers[Format].Deserialize(type, serialization);
    }
}