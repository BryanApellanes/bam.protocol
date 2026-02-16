
namespace Bam.Protocol;

/// <summary>
/// Represents an error that occurs when an unsupported request type is encountered.
/// </summary>
public class UnsupportedRequestTypeException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedRequestTypeException"/> class for the specified type.
    /// </summary>
    /// <param name="type">The unsupported request type.</param>
    public UnsupportedRequestTypeException(Type type) : base(
        $"Unsupported request type: {type.Name}")
    {
    }
}