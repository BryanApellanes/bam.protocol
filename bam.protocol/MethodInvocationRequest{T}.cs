namespace Bam.Protocol;

/// <summary>
/// Represents a strongly-typed method invocation request that creates the target instance automatically.
/// </summary>
/// <typeparam name="T">The type containing the method to invoke. Must have a parameterless constructor.</typeparam>
public class MethodInvocationRequest<T> : MethodInvocationRequest where T : class, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MethodInvocationRequest{T}"/> class, creating a new instance of <typeparamref name="T"/>.
    /// </summary>
    /// <param name="methodName">The name of the method to invoke.</param>
    public MethodInvocationRequest(string methodName) : base(new T(),
        Protocol.OperationIdentifier.For(typeof(T), methodName))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodInvocationRequest{T}"/> class with the specified instance.
    /// </summary>
    /// <param name="instance">The instance to invoke the method on.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    public MethodInvocationRequest(T instance, string methodName) : base(instance, methodName)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodInvocationRequest{T}"/> class with the specified instance and arguments.
    /// </summary>
    /// <param name="instance">The instance to invoke the method on.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="args">The arguments to pass to the method.</param>
    public MethodInvocationRequest(T instance, string methodName, params object[] args) : base(instance, methodName,
        args)
    {
    }
}