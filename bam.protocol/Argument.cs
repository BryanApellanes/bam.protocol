using System.Reflection;

namespace Bam.Protocol;

public class Argument : IArgument
{
    public string ParameterName { get; set; }
    public object Value { get; set; }

    public static Argument For(ParameterInfo parameter, object value = null)
    {
        return new Argument()
        {
            ParameterName = parameter.Name,
            Value = value ?? parameter.DefaultValue
        };
    }

    public static List<Argument> ListForValues<T>(string methodName, params object[] arguments)
    {
        return ListForValues(typeof(T).GetMethod(methodName), arguments);
    }
    
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
