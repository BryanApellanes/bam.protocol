namespace Bam.Protocol;

/// <summary>
/// Serializes and deserializes invocation context objects using JSON format.
/// </summary>
public class JsonInvocationContextSerializer : IInvocationContextSerializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonInvocationContextSerializer"/> class.
    /// </summary>
    public JsonInvocationContextSerializer()
    {
        this.Format = "json";

    }

    /// <inheritdoc />
    public string Format { get; }

    /// <inheritdoc />
    public string Serialize(object context)
    {
        return context.ToJson();
    }

    /// <inheritdoc />
    public object Deserialize(Type type, string serialization)
    {
        return serialization.FromJson(type);
    }
}