using System.Reflection;

namespace Bam.Protocol;

/// <summary>
/// Represents a named argument for a method invocation, containing a parameter name and its value.
/// </summary>
public class Argument : IArgument
{
    /// <summary>
    /// Gets or sets the name of the parameter.
    /// </summary>
    public string ParameterName { get; set; }

    /// <summary>
    /// Gets or sets the value of the argument.
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// Creates an <see cref="Argument"/> from a <see cref="ParameterInfo"/>, using the specified value or the parameter's default value.
    /// </summary>
    /// <param name="parameter">The parameter info to create the argument from.</param>
    /// <param name="value">The value for the argument, or null to use the parameter's default value.</param>
    /// <returns>A new <see cref="Argument"/> instance.</returns>
    public static Argument For(ParameterInfo parameter, object value = null)
    {
        return new Argument()
        {
            ParameterName = parameter.Name,
            Value = value ?? parameter.DefaultValue
        };
    }

    /// <summary>
    /// Creates a list of arguments by matching values to the parameters of the specified method on type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type containing the method.</typeparam>
    /// <param name="methodName">The name of the method.</param>
    /// <param name="arguments">The argument values to match to parameters.</param>
    /// <returns>A list of <see cref="Argument"/> instances.</returns>
    public static List<Argument> ListForValues<T>(string methodName, params object[] arguments)
    {
        return ListForValues(typeof(T).GetMethod(methodName), arguments);
    }
    
    /// <summary>
    /// Creates a list of arguments by matching values to the parameters of the specified method.
    /// </summary>
    /// <param name="methodInfo">The method info to get parameters from.</param>
    /// <param name="arguments">The argument values to match to parameters.</param>
    /// <returns>A list of <see cref="Argument"/> instances.</returns>
    public static List<Argument> ListForValues(MethodInfo methodInfo, params object[] arguments)
    {
        Args.ThrowIfNull(methodInfo, nameof(methodInfo));
        List<Argument> argumentList = new List<Argument>();
        ParameterInfo[] parameters = methodInfo.GetParameters();
        for(int i = 0; i < parameters.Length; i++)
        {
            ParameterInfo parameterInfo = parameters[i];
            argumentList.Add(Argument.For(parameterInfo, arguments[i]));
        }

        return argumentList;
    }
}
