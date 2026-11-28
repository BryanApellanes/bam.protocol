namespace Bam.Protocol.Profile;

/// <summary>
/// Bounds for a <see cref="BoundedMemoryKeySetStore"/>: how many key sets it holds at once, and how long each
/// one lives after it is registered.
/// </summary>
public sealed class BoundedMemoryKeySetStoreOptions
{
    /// <summary>The default <see cref="Capacity"/>.</summary>
    public const int DefaultCapacity = 1000;

    /// <summary>The default <see cref="KeySetLifetime"/>.</summary>
    public static readonly TimeSpan DefaultKeySetLifetime = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Gets the most live key sets the store holds. A registration past it throws
    /// <see cref="Bam.Protocol.Data.KeySetCapacityExceededException"/>. Must be at least 1.
    /// </summary>
    public int Capacity { get; init; } = DefaultCapacity;

    /// <summary>
    /// Gets how long a key set lives, measured from its registration. Once it has passed the key set never
    /// resolves again and its handle and key material are free. Must be positive.
    /// </summary>
    public TimeSpan KeySetLifetime { get; init; } = DefaultKeySetLifetime;
}
