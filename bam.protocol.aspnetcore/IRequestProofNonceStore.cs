namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Remembers the request-proof nonces a host has accepted, so a captured proof cannot be replayed while its
/// timestamp is still inside <see cref="ActorAuthenticationOptions.RequestProofMaxAge"/>. A host with more than one
/// instance registers a shared implementation; <see cref="MemoryRequestProofNonceStore"/> covers one process.
/// </summary>
public interface IRequestProofNonceStore
{
    /// <summary>
    /// Records a nonce as used, unless it already is or the store cannot record it.
    /// </summary>
    /// <param name="scope">Whose nonce this is: the fingerprint of the signing key.</param>
    /// <param name="nonce">The nonce from the request.</param>
    /// <param name="expiresAt">When the proof stops being acceptable anyway; the nonce need not be kept after it.</param>
    /// <param name="now">The current time.</param>
    /// <returns>Whether the nonce was claimed, was already used, or could not be recorded.</returns>
    NonceClaimResult TryClaim(string scope, string nonce, DateTimeOffset expiresAt, DateTimeOffset now);
}

/// <summary>The outcome of <see cref="IRequestProofNonceStore.TryClaim"/>.</summary>
public enum NonceClaimResult
{
    /// <summary>The nonce was unused and is now recorded: the proof may proceed.</summary>
    Claimed = 0,

    /// <summary>The nonce was already used by this scope: a replay.</summary>
    AlreadyUsed = 1,

    /// <summary>
    /// The store is at a capacity limit and cannot record the nonce. The proof is refused, because accepting an
    /// unrecorded nonce would allow its replay; this is load, not an attack on this request.
    /// </summary>
    StoreFull = 2,
}
