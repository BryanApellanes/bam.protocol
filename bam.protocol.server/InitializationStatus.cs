namespace Bam.Protocol.Server;

/// <summary>
/// Represents the status of a server context initialization pipeline.
/// </summary>
public enum InitializationStatus
{
    /// <summary>
    /// The initialization state is invalid or uninitialized.
    /// </summary>
    Invalid,

    /// <summary>
    /// Session initialization failed.
    /// </summary>
    SessionInitializationFailed,

    /// <summary>
    /// A session is required but was not provided.
    /// </summary>
    SessionRequired,

    /// <summary>
    /// The actor could not be resolved from the request.
    /// </summary>
    ActorResolutionFailed,

    /// <summary>
    /// Authentication of the request failed.
    /// </summary>
    AuthenticationFailed,

    /// <summary>
    /// The command could not be resolved from the request.
    /// </summary>
    CommandResolutionFailed,

    /// <summary>
    /// The authorization calculation failed.
    /// </summary>
    AuthorizationCalculationFailed,

    /// <summary>
    /// An unknown error occurred during initialization.
    /// </summary>
    UnknownError,

    /// <summary>
    /// Initialization completed successfully.
    /// </summary>
    Success,
}