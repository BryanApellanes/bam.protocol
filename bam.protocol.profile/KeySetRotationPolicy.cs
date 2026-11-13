namespace Bam.Protocol.Profile;

/// <summary>
/// How often one handle may rotate its key set (bam.protocol#89). <see cref="PublicKeySetRegistrar"/> refuses a
/// rotation that comes sooner than <see cref="MinimumInterval"/> after the handle's previous rotation, judged from the
/// time the registrar stamps on the row (<see cref="Bam.Protocol.Data.Profile.PublicKeySetData.LastRotatedUtc"/>). A
/// handle's first rotation is never delayed.
/// Because that time lives in the key-set store, the limit holds for every process that shares the store.
/// <para>
/// Why a host sets it: request-proof nonces are counted per signing key, so each rotation starts a handle on a fresh
/// per-key quota while the old key's nonces stay until they expire. A nonce can outlive its key's last use by up to
/// twice the proof's maximum age (the proof may be signed that far ahead). With rotations spaced at least that far
/// apart, any such stretch of time retires at most one key, so at most two of one handle's keys hold live nonces at
/// once: the current one and the one before it.
/// </para>
/// <para>
/// A handle registered again after an admin revocation starts with no rotation time, so its first rotation is free
/// too. For one interval it can then have three keys with live nonces (the revoked one, which can no longer sign, and
/// two new ones). That takes the break-glass revocation, so a key holder cannot cause it.
/// </para>
/// </summary>
public sealed class KeySetRotationPolicy
{
    /// <summary>
    /// The longest interval accepted: two days, twice the longest proof age bam.protocol.aspnetcore accepts
    /// (<c>RequestProofEvaluator.MaximumMaxAge</c>, one day), so the recommended interval is always within range.
    /// This project cannot reference that one, so the two constants are kept in step by hand.
    /// </summary>
    public static readonly TimeSpan MaxInterval = TimeSpan.FromDays(2);

    /// <summary>No limit: a handle may rotate as often as its proofs allow. The default, so existing hosts keep their behaviour.</summary>
    public static KeySetRotationPolicy None { get; } = new KeySetRotationPolicy(TimeSpan.Zero, TimeProvider.System);

    /// <summary>Initializes a new instance of the <see cref="KeySetRotationPolicy"/> class.</summary>
    /// <param name="minimumInterval">The least time between two rotations of one handle; zero for no limit, at most <see cref="MaxInterval"/>.</param>
    /// <param name="clock">The clock the interval is measured with, and the one the registrar stamps rotation times from.</param>
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
    /// How long a handle that last rotated at <paramref name="lastRotated"/> must still wait to rotate, or null when it
    /// may rotate now. For a time in the future (a clock that moved back by <c>d</c>) the answer is capped at the
    /// interval, but the handle stays refused until the clock passes the stamp plus the interval, <c>d</c> plus the
    /// interval in all: each refusal before the clock reaches the stamp reports the full interval, and each one after
    /// it reports what is left.
    /// </summary>
    /// <param name="lastRotated">When the handle last rotated, or null when it never has.</param>
    /// <returns>The wait, or null.</returns>
    public TimeSpan? WaitBeforeRotating(DateTime? lastRotated)
    {
        if (MinimumInterval == TimeSpan.Zero || lastRotated is null)
        {
            return null;
        }
        // The store may hand a stamped time back as local; an unmarked one was written as UTC.
        DateTime since = lastRotated.Value.Kind == DateTimeKind.Local
            ? lastRotated.Value.ToUniversalTime()
            : DateTime.SpecifyKind(lastRotated.Value, DateTimeKind.Utc);
        TimeSpan elapsed = UtcNow() - since;
        if (elapsed >= MinimumInterval)
        {
            return null;
        }
        return elapsed < TimeSpan.Zero ? MinimumInterval : MinimumInterval - elapsed;
    }
}
