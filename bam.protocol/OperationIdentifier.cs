using System.Reflection;

namespace Bam.Protocol;

/// <summary>
/// Identifies a specific operation (method) that can be invoked remotely, using the format "TypeFullName+MethodName, AssemblyFullName".
/// </summary>
public class OperationIdentifier : IOperationIdentifier
{
    /// <inheritdoc />
    public string Value { get; } = null!;

    /// <summary>
    /// Creates an operation identifier string for the specified method on type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type containing the method.</typeparam>
    /// <param name="methodName">The name of the method.</param>
    /// <returns>The operation identifier string.</returns>
    public static string For<T>(string methodName)
    {
        return For(typeof(T), methodName);
    }

    /// <summary>
    /// Creates an operation identifier string for the specified method on the given type.
    /// </summary>
    /// <param name="type">The type containing the method.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <returns>The operation identifier string.</returns>
    public static string For(Type type, string methodName)
    {
        Args.ThrowIfNull(type, "type");
        return For(type.GetMethod(methodName)!);
    }

    /// <summary>
    /// Creates an operation identifier string for the specified method info.
    /// </summary>
    /// <param name="method">The method info.</param>
    /// <returns>The operation identifier string in the format "TypeFullName+MethodName, AssemblyFullName".</returns>
    public static string For(MethodInfo method)
    {
        Args.ThrowIfNull(method, nameof(method));
        return $"{method.DeclaringType!.FullName}+{method.Name}, {method.DeclaringType.Assembly.FullName}";
    }

    /// <summary>
    /// Resolves a <see cref="MethodInfo"/> from an operation identifier string.
    /// </summary>
    /// <param name="operationIdentifier">The operation identifier string.</param>
    /// <returns>The resolved <see cref="MethodInfo"/>.</returns>
    public static MethodInfo ToMethod(string operationIdentifier)
    {
        Args.ThrowIfNull(operationIdentifier, nameof(operationIdentifier));
        string[] parts = operationIdentifier.Split('+', ',');
        string typeName = parts[0];
        string methodName = parts[1];
        string assemblyName = parts[2];
        Assembly assembly = Assembly.Load(assemblyName);
        Type type = assembly.GetType(typeName)!;
        return type.GetMethod(methodName)!;
    }
}