namespace Bam.Protocol.Server;

/// <summary>
/// Defines a component that resolves an <see cref="ICommand"/> from a BAM request.
/// </summary>
public interface ICommandResolver
{
    /// <summary>
    /// Resolves the command to execute from the specified request.
    /// </summary>
    /// <param name="request">The BAM request to resolve a command from.</param>
    /// <returns>The resolved command, or null if no command could be resolved.</returns>
    ICommand ResolveCommand(IBamRequest request);
}