namespace Bam.Protocol;

/// <summary>
/// Represents a command to invoke a method on a named type with arguments.
/// </summary>
public class Command : ICommand
{
    /// <inheritdoc />
    public string TypeName { get; set; }

    /// <inheritdoc />
    public string MethodName { get; set; }

    /// <inheritdoc />
    public string[] Arguments { get; set; }
}
