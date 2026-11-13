namespace Bam.Protocol.Profile;

/// <summary>
/// How often one handle may rotate its key set (bam.protocol#89). <see cref="PublicKeySetRegistrar"/> refuses a
/// rotation that comes sooner than <see cref="MinimumInterval"/> after the handle's current keys were set, judged
/// from the time the registrar stamps on the row (<see cref="Bam.Protocol.Data.Profile.PublicKeySetData.KeyMaterialSinceUtc"/>).
/// Because that time lives in the key-set store, the limit holds for every process that shares the store.
/// <para>
/// Why a host sets it: request-proof nonces are counted per signing key, so each rotation starts a handle on a fresh
/// per-key quota while the old key's nonces stay until they expire. A nonce can outlive its key's last use by up to
/// twice the proof's maximum age (the proof may be signed that far ahead), so an interval of at least that long keeps
/// at most two of one handle's keys holding live nonces at once.
/// </para>
/// </summary>
public sealed class KeySetRotationPolicy
{
    /// <summary>The longest interval accepted: a day.</summary>
    public static readonly TimeSpan MaxInterval = TimeSpan.FromDays(1);

    /// <summary>No limit: a handle may rotate as often as its proofs allow. The default, so existing hosts keep their behaviour.</summary>
    public static KeySetRotationPolicy None { get; } = new KeySetRotationPolicy(TimeSpan.Zero, TimeProvider.System);

    /// <summary>Initializes a new instance of the <see cref="KeySetRotationPolicy"/> class.</summary>
    /// <param name="minimumInterval">The least time between two rotations of one handle; zero for no limit, at most <see cref="MaxInterval"/>.</param>
    /// <param name="clock">The clock the interval is measured with, and the one the registrar stamps key-material times from.</param>
    /// <exception cref="ArgumentOutOfRangeException">The interval is negative or longer than <see cref="MaxInterval"/>.</exception>
    public KeySetRotationPolicy(TimeSpan minimumInterval, TimeProvider clock)
    {
        ArgumentNullException.ThrowIfNull(clock);
        if (minimumInterval < TimeSpan.Zero || minimumInterval > MaxInterval)
        {
            throw new ArgumentOutOfRangeException(nameof(minimumInterval), minimumInterval, $"The minimum rotation interval must be between zero and {MaxInterval}.");
        }
        MinimumInterval = minimumInterval;
        Clock = clock;
    }

    /// <summary>Gets the least time between two rotations of one handle; zero for no limit.</summary>
    public TimeSpan MinimumInterval { get; }

    /// <summary>Gets the clock the interval is measured with.</summary>
    public TimeProvider Clock { get; }

    /// <summary>The current UTC time from <see cref="Clock"/>.</summary>
    /// <returns>The time, as a UTC <see cref="DateTime"/>.</returns>
    public DateTime UtcNow()
    {
        return Clock.GetUtcNow().UtcDateTime;
    }

    /// <summary>
    /// How long a handle whose keys were set at <paramref name="keyMaterialSince"/> must still wait to rotate, or null
    /// when it may rotate now. A time in the future (a clock that moved back) waits the whole interval, never longer.
    /// </summary>
    /// <param name="keyMaterialSince">When the handle's current keys were set, or null when that is unknown.</param>
    /// <returns>The wait, or null.</returns>
    public TimeSpan? WaitBeforeRotating(DateTime? keyMaterialSince)
    {
        if (MinimumInterval == TimeSpan.Zero || keyMaterialSince is null)
        {
            return null;
        }
        // The store may hand a stamped time back as local; an unmarked one was written as UTC.
        DateTime since = keyMaterialSince.Value.Kind == DateTimeKind.Local
            ? keyMaterialSince.Value.ToUniversalTime()
            : DateTime.SpecifyKind(keyMaterialSince.Value, DateTimeKind.Utc);
        TimeSpan elapsed = UtcNow() - since;
        if (elapsed >= MinimumInterval)
        {
            return null;
        }
        return elapsed < TimeSpan.Zero ? MinimumInterval : MinimumInterval - elapsed;
    }
}
