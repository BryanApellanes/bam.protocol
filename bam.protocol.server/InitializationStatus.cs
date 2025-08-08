namespace Bam.Protocol.Server;

public enum InitializationStatus
{
    Invalid,
    BeforeInitializationError,
    SessionInitializationFailed,
    SessionRequired,
    ActorResolutionFailed,
    CommandResolutionFailed,
    AuthorizationCalculationFailed,
    AfterInitializationError,
    UnknownError,
    Success,
}