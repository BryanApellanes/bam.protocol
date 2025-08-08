using System.Reflection;

namespace Bam.Protocol;

public class OperationIdentifier : IOperationIdentifier
{
    public string Value { get; }

    public static string For<T>(string methodName)
    {
        return For(typeof(T), methodName);
    }
    
    public static string For(Type type, string methodName)
    {
        return For(type.GetMethod(methodName));
    }
    
    public static string For(MethodInfo method)
    {
        Args.ThrowIfNull(method, nameof(method));
        return $"{method.DeclaringType.FullName}+{method.Name}, {method.DeclaringType.Assembly.FullName}";
    }

    public static MethodInfo ToMethod(string operationIdentifier)
    {
        Args.ThrowIfNull(operationIdentifier, nameof(operationIdentifier));
        string[] parts = operationIdentifier.Split('+', ',');
        string typeName = parts[0];
        string methodName = parts[1];
        string assemblyName = parts[2];
        Assembly assembly = Assembly.Load(assemblyName);
        Type type = assembly.GetType(typeName);
        return type.GetMethod(methodName);
    }
}