namespace Bam.Protocol;

/// <summary>
/// Defines a serializer for converting context objects to a specific string format.
/// </summary>
public interface IContextSerializer
{
    /// <summary>
    /// Gets the serialization format identifier (e.g., "json", "yaml").
    /// </summary>
    string Format { get; }

    /// <summary>
    /// Serializes the specified data to a string.
    /// </summary>
    /// <param name="data">The object to serialize.</param>
    /// <returns>The serialized string representation.</returns>
    string Serialize(object data);
}