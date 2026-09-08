namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Selects how bearer tokens are verified by <see cref="ActorAuthenticationMiddleware"/>.
/// </summary>
public enum ActorAuthenticationMode
{
    /// <summary>
    /// Both factors (design threeheadz-tracker#30, Refinement 1): a server-issued token whose
    /// <c>kfp</c> claim binds the caller's registered ECC key, plus per-request body signatures on
    /// protected endpoints. Requires a server signing key under
    /// <see cref="ActorAuthenticationOptions.ServerKeyName"/>.
    /// </summary>
    Hybrid = 0,

    /// <summary>
    /// Degraded mode for hosts without a server signing key: the caller signs its own short-lived
    /// token, verified against its registered active key set. No issuance endpoint is needed.
    /// </summary>
    ClientSignedOnly = 1,
}
