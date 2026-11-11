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
    /// <remarks>
    /// A shared store whose backing storage fails returns <see cref="NonceClaimResult.Unavailable"/> (or throws), and never
    /// returns <see cref="NonceClaimResult.Claimed"/> for a nonce it did not record. <see cref="RequestProofEvaluator"/>
    /// refuses the proof in both cases and logs the outage. A store's own timeout counts as an outage whatever exception
    /// carries it, <see cref="OperationCanceledException"/> included; only a cancellation caused by the client aborting the
    /// request propagates.
    /// </remarks>
    NonceClaimResult TryClaim(string scope, string nonce, DateTimeOffset expiresAt, DateTimeOffset now);

    /// <summary>
    /// The form <see cref="RequestProofEvaluator"/> calls: the same claim as <see cref="TryClaim"/>, without holding a
    /// thread while a shared store waits on its backing storage, and able to stop when the caller goes away
    /// (bam.protocol#59). The default implementation runs <see cref="TryClaim"/> and completes synchronously, so a store
    /// written before this member existed, or one that never blocks (<see cref="MemoryRequestProofNonceStore"/>),
    /// needs no change. A store that does I/O overrides it and passes <paramref name="cancellationToken"/> on.
    /// </summary>
    /// <param name="scope">Whose nonce this is: the fingerprint of the signing key.</param>
    /// <param name="nonce">The nonce from the request.</param>
    /// <param name="expiresAt">When the proof stops being acceptable anyway; the nonce need not be kept after it.</param>
    /// <param name="now">The current time.</param>
    /// <param name="cancellationToken">
    /// The request's <c>RequestAborted</c> token. Cancellation by this token means the client is gone and propagates;
    /// the evaluator treats every other failure, a store's own timeout included, as an outage and fails closed.
    /// </param>
    /// <returns>Whether the nonce was claimed, was already used, or could not be recorded.</returns>
    /// <remarks>
    /// The atomicity and retention rules of <see cref="TryClaim"/> apply unchanged. A store that has recorded the nonce
    /// must not then report cancellation in place of <see cref="NonceClaimResult.Claimed"/> in a way that lets the same
    /// nonce be claimed again: check the token before the write, not after it.
    /// </remarks>
    ValueTask<NonceClaimResult> TryClaimAsync(string scope, string nonce, DateTimeOffset expiresAt, DateTimeOffset now, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return new ValueTask<NonceClaimResult>(TryClaim(scope, nonce, expiresAt, now));
    }
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

    /// <summary>
    /// The store could not reach its backing storage, so it cannot tell whether the nonce is new. The proof is refused
    /// (fail closed): accepting it would allow its replay on another instance.
    /// </summary>
    Unavailable = 4,
}
