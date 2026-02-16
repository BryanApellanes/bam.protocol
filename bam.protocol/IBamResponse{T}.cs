
namespace Bam.Protocol;

/// <summary>
/// Defines a Bam HTTP response with strongly-typed content.
/// </summary>
/// <typeparam name="T">The type of the response content.</typeparam>
public interface IBamResponse<T>: IBamResponse
{
    /// <summary>
    /// Gets the strongly-typed response content.
    /// </summary>
    T Content { get; }
}