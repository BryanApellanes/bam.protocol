namespace Bam.Protocol.Server;

/// <summary>
/// Defines a component that creates BAM responses for server contexts and initialization contexts.
/// </summary>
public interface IBamResponseProvider
{
    /// <summary>
    /// Creates a response based on the initialization context, which may represent a failure or success state.
    /// </summary>
    /// <param name="initialization">The initialization context.</param>
    /// <returns>The created response.</returns>
    IBamResponse CreateResponse(BamServerInitializationContext initialization);

    /// <summary>
    /// Creates a response for a fully initialized server context.
    /// </summary>
    /// <param name="serverContext">The server context.</param>
    /// <returns>The created response.</returns>
    IBamResponse CreateResponse(IBamServerContext serverContext);
}