namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Remembers the request-proof nonces a host has accepted, so a captured proof cannot be replayed while its
/// timestamp is still inside <see cref="ActorAuthenticationOptions.RequestProofMaxAge"/>. A host with more than one
/// instance registers a shared implementation; <see cref="MemoryRequestProofNonceStore"/> covers one process.
/// </summary>
public interface IRequestProofNonceStore
{
    /// <summary>
    /// Records a nonce as used, unless it already is or the store cannot record it. A shared implementation must claim
    /// atomically (two instances racing on one nonce: exactly one gets <see cref="NonceClaimResult.Claimed"/>) and must
    /// keep the nonce through <paramref name="expiresAt"/> inclusive: a store whose expiry rounds down, such as a
    /// whole-second TTL, would reopen the replay window at the edge of <see cref="ActorAuthenticationOptions.RequestProofMaxAge"/>.
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
    /// The store holds as many nonces as it may in total and cannot record this one. The proof is refused, because
    /// accepting an unrecorded nonce would allow its replay; this is load, not an attack on this request.
    /// </summary>
    StoreFull = 2,

    /// <summary>The signing key holds as many unexpired nonces as one key may; only this key's proofs are refused.</summary>
    KeyFull = 3,
}
