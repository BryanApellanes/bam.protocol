namespace Bam.Protocol.Server;

/// <summary>
/// Defines a component that authenticates a BAM server request context.
/// </summary>
public interface IAuthenticator
{
    /// <summary>
    /// Authenticates the specified server context.
    /// </summary>
    /// <param name="serverContext">The server context to authenticate.</param>
    /// <returns>The authentication result.</returns>
    BamAuthentication Authenticate(IBamServerContext serverContext);
}
