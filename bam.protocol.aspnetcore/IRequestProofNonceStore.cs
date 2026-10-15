namespace Bam.Protocol.AspNetCore;

/// <summary>
/// Remembers the request-proof nonces a host has accepted, so a captured proof cannot be replayed while
/// its timestamp is still inside <see cref="ActorAuthenticationOptions.RequestProofMaxAge"/>. A host with
/// more than one instance registers a shared implementation; <see cref="MemoryRequestProofNonceStore"/>
/// covers one process.
/// </summary>
public interface IRequestProofNonceStore
{
    /// <summary>
    /// Records a nonce as used, unless it already is.
    /// </summary>
    /// <param name="scope">Whose nonce this is: the fingerprint of the signing key.</param>
    /// <param name="nonce">The nonce from the request.</param>
    /// <param name="expiresAt">When the proof stops being acceptable anyway; the nonce need not be kept after it.</param>
    /// <param name="now">The current time.</param>
    /// <returns>True when the nonce was unused and is now recorded; false when it was used, or the store cannot record it.</returns>
    bool TryClaim(string scope, string nonce, DateTimeOffset expiresAt, DateTimeOffset now);
}
