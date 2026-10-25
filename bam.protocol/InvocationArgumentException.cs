namespace Bam.Protocol;

/// <summary>
/// Raised when an invocation argument cannot be converted to the type its target parameter declares.
/// </summary>
/// <remarks>
/// Thrown by <see cref="InvocationArgumentCoercer"/> and surfaced unchanged by
/// <see cref="MethodInvocationRequest.Invoke()"/>, so a host can answer a malformed argument as a client error
/// instead of a server fault. The message names the parameter and both types; the inner exception carries the
/// converter's own reason.
/// </remarks>
public class InvocationArgumentException : ArgumentException
{
    /// <summary>
    /// Initializes a new instance for a parameter that has no default and was not supplied at all.
    /// <see cref="ActualType"/> is <see cref="System.Reflection.Missing"/>.
    /// </summary>
    /// <param name="parameterName">The missing parameter's name.</param>
    /// <param name="targetType">The parameter type the method declares.</param>
    public InvocationArgumentException(string parameterName, Type targetType)
        : base($"Argument '{parameterName}' of type {targetType.FullName} is required but was not supplied.", parameterName)
    {
        ActualType = typeof(System.Reflection.Missing);
        TargetType = targetType;
    }

    /// <summary>
    /// Initializes a new instance for a supplied argument that cannot be converted to its parameter's type.
    /// </summary>
    /// <param name="parameterName">The target parameter's name.</param>
    /// <param name="actualType">The runtime type of the deserialized argument.</param>
    /// <param name="targetType">The parameter type the method declares.</param>
    /// <param name="innerException">The converter's failure, when there is one.</param>
    public InvocationArgumentException(string parameterName, Type actualType, Type targetType, Exception? innerException = null)
        : base($"Argument '{parameterName}' of type {actualType.FullName} cannot be converted to {targetType.FullName}.", parameterName, innerException)
    {
        this.ActualType = actualType;
        this.TargetType = targetType;
    }

    /// <summary>
    /// Gets the runtime type of the deserialized argument that failed to convert.
    /// </summary>
    public Type ActualType { get; }

    /// <summary>
    /// Gets the parameter type the method declares.
    /// </summary>
    public Type TargetType { get; }
}
