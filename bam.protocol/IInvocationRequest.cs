namespace Bam.Protocol;

/// <summary>
/// Defines an invocation request containing operation identifier, serialized context, and arguments.
/// </summary>
public interface IInvocationRequest
{
    /// <summary>
    /// The format that the SerializedContext is serialized in.
    /// </summary>
    string ContextSerializationFormat { get; }
    
    /// <summary>
    /// The name of the operation.
    /// </summary>
    string OperationIdentifier { get; }
    
    /// <summary>
    /// Gets the serialized context.
    /// </summary>
    string SerializedContext { get; }
    
    /// <summary>
    /// Gets the arguments.
    /// </summary>
    List<Argument> Arguments { get; }
}