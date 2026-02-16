namespace Bam.Protocol.Server;

/// <summary>
/// Defines a handler that performs a specific step in the server context initialization pipeline.
/// </summary>
public interface IBamServerContextInitializationHandler
{
    /// <summary>
    /// Handles a step of the server context initialization process.
    /// </summary>
    /// <param name="initialization">The initialization context to process.</param>
    /// <returns>The updated initialization context.</returns>
    BamServerInitializationContext HandleInitialization(BamServerInitializationContext initialization);
}