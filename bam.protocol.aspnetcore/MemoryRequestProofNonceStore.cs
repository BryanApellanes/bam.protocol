namespace Bam.Protocol.AspNetCore;

/// <summary>
/// An in-process <see cref="IRequestProofNonceStore"/> with a fixed capacity. Expired nonces are dropped
/// before each claim. When the store is full of unexpired nonces it refuses new claims rather than evict
/// one early, because an evicted nonce could be replayed: the host answers 401 until entries expire.
/// </summary>
public sealed class MemoryRequestProofNonceStore : IRequestProofNonceStore
{
    private readonly object _sync = new object();
    private readonly Dictionary<string, DateTimeOffset> _claimed = new Dictionary<string, DateTimeOffset>(StringComparer.Ordinal);
    private readonly PriorityQueue<string, DateTimeOffset> _expiries = new PriorityQueue<string, DateTimeOffset>();
    private readonly int _capacity;

    /// <summary>Creates a store sized by <see cref="ActorAuthenticationOptions.RequestProofNonceCapacity"/>.</summary>
    /// <param name="options">Carries the capacity.</param>
    /// <exception cref="InvalidOperationException">The capacity is not positive.</exception>
    public MemoryRequestProofNonceStore(ActorAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.RequestProofNonceCapacity <= 0)
        {
            throw new InvalidOperationException("ActorAuth:RequestProofNonceCapacity must be positive.");
        }

        _capacity = options.RequestProofNonceCapacity;
    }

    /// <summary>Gets the number of nonces currently held, expired or not.</summary>
    public int Count
    {
        get
        {
            lock (_sync)
            {
                return _claimed.Count;
            }
        }
    }

    /// <inheritdoc />
    public bool TryClaim(string scope, string nonce, DateTimeOffset expiresAt, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(scope);
        ArgumentNullException.ThrowIfNull(nonce);
        string key = scope + "\n" + nonce;
        lock (_sync)
        {
            DropExpired(now);
            if (_claimed.ContainsKey(key))
            {
                return false;
            }

            if (_claimed.Count >= _capacity)
            {
                return false;
            }

            _claimed[key] = expiresAt;
            _expiries.Enqueue(key, expiresAt);
            return true;
        }
    }

    private void DropExpired(DateTimeOffset now)
    {
        while (_expiries.TryPeek(out string? key, out DateTimeOffset expiresAt) && expiresAt <= now)
        {
            _expiries.Dequeue();
            _claimed.Remove(key);
        }
    }
}
