namespace Bam.Protocol.Server;

/// <summary>
/// Defines a manager for server-side session lifecycle operations.
/// </summary>
public interface IServerSessionManager
{
    /// <summary>
    /// Starts a new session and returns the session response.
    /// </summary>
    /// <param name="request">The session start request.</param>
    /// <param name="outputStream">The output stream for writing the response.</param>
    /// <param name="statusCode">The HTTP status code for the response.</param>
    /// <returns>The session start response containing session details.</returns>
    StartSessionResponse StartSession(StartSessionRequest request, Stream outputStream, int statusCode = 200);

    /// <summary>
    /// Ends the session with the specified identifier.
    /// </summary>
    /// <param name="sessionId">The session identifier to end.</param>
    /// <returns>True if the session was successfully ended; otherwise false.</returns>
    bool EndSession(string sessionId);

    /// <summary>
    /// Gets the session state associated with the specified request.
    /// </summary>
    /// <param name="request">The BAM request containing session information.</param>
    /// <returns>The session state, or null if no session exists.</returns>
    IServerSessionState GetSession(IBamRequest request);

    /// <summary>
    /// Gets the session identifier from the specified request.
    /// </summary>
    /// <param name="request">The BAM request to extract the session ID from.</param>
    /// <returns>The session identifier string.</returns>
    string GetSessionId(IBamRequest request);

    /// <summary>
    /// Determines whether the specified request contains a session identifier.
    /// </summary>
    /// <param name="request">The BAM request to check.</param>
    /// <returns>True if the request has a session ID; otherwise false.</returns>
    bool HasSessionId(IBamRequest request);
}