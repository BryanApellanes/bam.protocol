namespace Bam.Protocol;

/// <summary>
/// Defines a command to invoke a method on a named type with arguments.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Gets or sets the fully qualified type name of the class containing the method.
    /// </summary>
    string TypeName { get; set; }

    /// <summary>
    /// Gets or sets the name of the method to invoke.
    /// </summary>
    string MethodName { get; set; }

    /// <summary>
    /// Gets or sets the arguments to pass to the method.
    /// </summary>
    string[] Arguments { get; set; }
}