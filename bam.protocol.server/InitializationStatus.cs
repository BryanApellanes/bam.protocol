namespace Bam.Protocol.Server;

public enum InitializationStatus
{
    Invalid,
    //BeforeInitializationError,
    SessionInitializationFailed,
    SessionRequired,
    ActorResolutionFailed,
    AuthenticationFailed,
    CommandResolutionFailed,
    AuthorizationCalculationFailed,
    //AfterInitializationError,
    UnknownError,
    Success,
}