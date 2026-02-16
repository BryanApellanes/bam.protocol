using Newtonsoft.Json;

namespace Bam.Protocol;

/// <summary>
/// Defines a named argument for method invocation.
/// </summary>
public interface IArgument
{
    /// <summary>
    /// Gets or sets the name of the parameter.
    /// </summary>
    string ParameterName { get; set; }

    /// <summary>
    /// Gets or sets the value of the argument.
    /// </summary>
    object Value { get; set; }
}