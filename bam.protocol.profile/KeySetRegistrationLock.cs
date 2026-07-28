namespace Bam.Protocol.Profile;

/// <summary>
/// The process-wide mutual-exclusion object shared by <see cref="PublicKeySetRegistrar"/> and
/// <see cref="KeySetRevocation"/>.  Registration, rotation, and revocation must serialize against
/// each other so that, e.g., a revoke-then-free-handle is atomic with respect to a concurrent
/// registration of the same handle.  Cross-process mutual exclusion is a separate, store-level
/// concern tracked in bam.protocol#12.
/// </summary>
internal static class KeySetRegistrationLock
{
    /// <summary>
    /// The shared lock object. Hold it around any read-then-write of the key-set store.
    /// </summary>
    internal static readonly object Sync = new object();
}
