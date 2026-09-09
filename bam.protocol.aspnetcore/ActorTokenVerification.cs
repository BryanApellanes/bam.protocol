namespace Bam.Protocol.AspNetCore;

/// <summary>
/// The outcome of verifying a bearer token: on success the resolved actor and the PEM of the ECC
/// public key its registered active key set carries (the key every per-request body signature must
/// verify against); on failure the reasons, in the order they were found.
/// </summary>
public sealed record ActorTokenVerification
{
    /// <summary>Gets whether the token proved an enrolled actor.</summary>
    public required bool Success { get; init; }

    /// <summary>Gets the resolved actor, or null when verification failed.</summary>
    public IActor? Actor { get; init; }

    /// <summary>Gets the handle of the key set the token was verified against, or null on failure.</summary>
    public string? KeySetHandle { get; init; }

    /// <summary>Gets the PEM of the registered ECC public key, or null on failure.</summary>
    public string? EccPublicKeyPem { get; init; }

    /// <summary>Gets diagnostic messages: the failure reasons, or the checks that passed.</summary>
    public IReadOnlyList<string> Messages { get; init; } = [];

    /// <summary>Builds a failed verification carrying the given reasons.</summary>
    /// <param name="reasons">Why verification failed.</param>
    /// <returns>A failed verification.</returns>
    public static ActorTokenVerification Failed(params string[] reasons)
    {
        return new ActorTokenVerification { Success = false, Messages = reasons };
    }
}
