using Bam.Protocol.Data;

namespace Bam.Protocol.Server;

/// <summary>
/// Represents the result of an authorization calculation, including access level and associated messages.
/// </summary>
public interface IAuthorizationCalculation
{
    /// <summary>
    /// Gets the messages associated with the authorization calculation.
    /// </summary>
    string[] Messages { get; }

    /// <summary>
    /// Gets the calculated access level.
    /// </summary>
    BamAccess Access { get; }

    /// <summary>
    /// Gets the request that was authorized.
    /// </summary>
    IBamRequest Request { get; }

    /// <summary>
    /// Gets the response associated with this authorization.
    /// </summary>
    IBamResponse Response { get; }

    /// <summary>
    /// Gets the actor whose access was calculated.
    /// </summary>
    IActor Actor { get; }
}