namespace Bam.Protocol;

/// <summary>
/// Serializes and deserializes invocation context objects using YAML format.
/// </summary>
public class YamlInvocationContextSerializer : IInvocationContextSerializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="YamlInvocationContextSerializer"/> class.
    /// </summary>
    public YamlInvocationContextSerializer()
    {
        this.Format = "yaml";

    }

    /// <inheritdoc />
    public string Format { get; }

    /// <inheritdoc />
    public string Serialize(object context)
    {
        return context.ToYaml();
    }

    /// <inheritdoc />
    public object Deserialize(Type type, string serialization)
    {
        return serialization.FromYaml(type);
    }
}