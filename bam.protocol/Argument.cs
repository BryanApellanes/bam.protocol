using System.Reflection;

namespace Bam.Protocol;

/// <summary>
/// Provides factory methods for creating argument dictionaries for method invocations.
/// </summary>
public static class Argument
{
    /// <summary>
    /// Creates an argument dictionary by matching values to the parameters of the specified method on type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type containing the method.</typeparam>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="arguments">The argument values to match to parameters.</param>
    /// <returns>A dictionary mapping parameter names to values.</returns>
    public static Dictionary<string, object?> ForValues<T>(string methodName, params object[] arguments)
    {
        return ForValues(typeof(T).GetMethod(methodName)!, arguments);
    }

    /// <summary>
    /// Creates an argument dictionary by matching values to the parameters of the specified method.
    /// </summary>
    /// <param name="methodInfo">The method info to get parameters from.</param>
    /// <param name="arguments">The argument values to match to parameters.</param>
    /// <returns>A dictionary mapping parameter names to values.</returns>
    public static Dictionary<string, object?> ForValues(MethodInfo methodInfo, params object[] arguments)
    {
        Args.ThrowIfNull(methodInfo, nameof(methodInfo));
        Dictionary<string, object?> result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        ParameterInfo[] parameters = methodInfo.GetParameters();
        for (int i = 0; i < parameters.Length; i++)
        {
            ParameterInfo parameterInfo = parameters[i];
            result[parameterInfo.Name!] = i < arguments.Length ? arguments[i] : parameterInfo.DefaultValue;
        }

        return result;
    }
}
