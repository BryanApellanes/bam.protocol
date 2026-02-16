namespace Bam.Protocol.Client;

/// <summary>
/// Defines a manager for creating and establishing client sessions with a Bam server.
/// </summary>
public interface IClientSessionManager
{
    /// <summary>
    /// Starts a new session with the specified request details.
    /// </summary>
    /// <param name="request">The start session request containing client information.</param>
    /// <returns>The server's start session response.</returns>
    Task<StartSessionResponse> StartSessionAsync(StartSessionRequest request);

    /// <summary>
    /// Starts a new session using default settings.
    /// </summary>
    /// <returns>The established client session state.</returns>
    Task<IClientSessionState> StartSessionAsync();
}