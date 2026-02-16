namespace Bam.Protocol;

/// <summary>
/// Identifies a specific operation (method) that can be invoked remotely.
/// </summary>
public interface IOperationIdentifier
{
    /// <summary>
    /// Gets the string representation of the operation identifier.
    /// </summary>
    string Value { get; }
}