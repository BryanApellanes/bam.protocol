using Bam.Protocol.Data;
using Bam.Protocol.Server;

namespace Bam.Protocol;

/// <summary>
/// Represents the result of an authentication attempt, including success status and associated messages.
/// </summary>
public class BamAuthentication
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamAuthentication"/> class.
    /// </summary>
    /// <param name="success">Whether the authentication was successful.</param>
    /// <param name="actor">The actor that was authenticated.</param>
    /// <param name="request">The request that was authenticated.</param>
    /// <param name="messages">Optional messages describing the authentication result.</param>
    public BamAuthentication(bool success, IActor actor, IBamRequest request, string[] messages = null!)
    {
        Success = success;
        Actor = actor;
        Request = request;
        Messages = messages ?? Array.Empty<string>();
    }

    /// <summary>
    /// Gets a value indicating whether the authentication was successful.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets the messages describing the authentication result.
    /// </summary>
    public string[] Messages { get; }

    /// <summary>
    /// Gets the actor that was authenticated.
    /// </summary>
    public IActor Actor { get; }

    /// <summary>
    /// Gets the request that was authenticated.
    /// </summary>
    public IBamRequest Request { get; }
}