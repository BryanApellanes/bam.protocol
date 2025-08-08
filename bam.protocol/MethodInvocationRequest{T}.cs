namespace Bam.Protocol;

public class MethodInvocationRequest<T> : MethodInvocationRequest where T : class, new()
{
    public MethodInvocationRequest(string methodName) : base(new T(),
        Protocol.OperationIdentifier.For(typeof(T), methodName))
    {
    }

    public MethodInvocationRequest(T instance, string methodName) : base(instance, methodName)
    {
    }

    public MethodInvocationRequest(T instance, string methodName, params object[] args) : base(instance, methodName,
        args)
    {
    }
}