namespace Bam.Protocol.Server;

/// <summary>
/// Defines a component that processes a BAM server request context and returns a result.
/// </summary>
public interface IBamRequestProcessor
{
    /// <summary>
    /// Processes the specified server request context and returns the result.
    /// </summary>
    /// <param name="serverContext">The server context to process.</param>
    /// <returns>The result of processing the request.</returns>
    object ProcessRequestContext(IBamServerContext serverContext);
}
