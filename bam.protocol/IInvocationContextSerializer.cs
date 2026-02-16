namespace Bam.Protocol;

/// <summary>
/// Defines a serializer for converting invocation context objects to and from a specific string format.
/// </summary>
public interface IInvocationContextSerializer
{
    /// <summary>
    /// Gets the serialization format identifier (e.g., "json", "yaml").
    /// </summary>
    string Format { get; }

    /// <summary>
    /// Serializes the specified context object to a string.
    /// </summary>
    /// <param name="context">The context object to serialize.</param>
    /// <returns>The serialized string representation.</returns>
    string Serialize(object context);

    /// <summary>
    /// Deserializes the specified string to an object of the given type.
    /// </summary>
    /// <param name="type">The target type to deserialize into.</param>
    /// <param name="serialization">The serialized string to deserialize.</param>
    /// <returns>The deserialized object.</returns>
    object Deserialize(Type type, string serialization);
}