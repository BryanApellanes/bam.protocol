namespace Bam.Protocol.AspNetCore;

/// <summary>
/// An in-process <see cref="IRequestProofNonceStore"/> with two capacity limits: at most
/// <see cref="ActorAuthenticationOptions.RequestProofNoncesPerKey"/> unexpired nonces per signing key, and at most
/// <see cref="ActorAuthenticationOptions.RequestProofNonceCapacity"/> in all. Expired nonces are dropped before each
/// claim. A full store refuses new claims rather than evict an unexpired nonce, which could then be replayed; the
/// per-key limit means one busy or hostile key can only lock itself out, not every other actor.
/// </summary>
public sealed class MemoryRequestProofNonceStore : IRequestProofNonceStore
{
    private readonly object _sync = new object();
    private readonly Dictionary<string, Claim> _claimed = new Dictionary<string, Claim>(StringComparer.Ordinal);
    private readonly Dictionary<string, int> _perScope = new Dictionary<string, int>(StringComparer.Ordinal);
    private readonly PriorityQueue<string, DateTimeOffset> _expiries = new PriorityQueue<string, DateTimeOffset>();
    private readonly int _capacity;
    private readonly int _perKey;

    /// <summary>Creates a store sized by the options.</summary>
    /// <param name="options">Carries <see cref="ActorAuthenticationOptions.RequestProofNonceCapacity"/> and <see cref="ActorAuthenticationOptions.RequestProofNoncesPerKey"/>.</param>
    /// <exception cref="InvalidOperationException">A limit is not positive, or the per-key limit exceeds the total.</exception>
    public MemoryRequestProofNonceStore(ActorAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.RequestProofNonceCapacity <= 0)
        {
            throw new InvalidOperationException("ActorAuth:RequestProofNonceCapacity must be positive.");
        }

        if (options.RequestProofNoncesPerKey <= 0 || options.RequestProofNoncesPerKey > options.RequestProofNonceCapacity)
        {
            throw new InvalidOperationException("ActorAuth:RequestProofNoncesPerKey must be positive and no more than ActorAuth:RequestProofNonceCapacity.");
        }

        _capacity = options.RequestProofNonceCapacity;
        _perKey = options.RequestProofNoncesPerKey;
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
    public NonceClaimResult TryClaim(string scope, string nonce, DateTimeOffset expiresAt, DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(scope);
        ArgumentNullException.ThrowIfNull(nonce);
        string key = scope + "\n" + nonce;
        lock (_sync)
        {
            DropExpired(now);
            if (_claimed.ContainsKey(key))
            {
                return NonceClaimResult.AlreadyUsed;
            }

            int held = _perScope.TryGetValue(scope, out int count) ? count : 0;
            if (held >= _perKey || _claimed.Count >= _capacity)
            {
                return NonceClaimResult.StoreFull;
            }

            _claimed[key] = new Claim(scope, expiresAt);
            _perScope[scope] = held + 1;
            _expiries.Enqueue(key, expiresAt);
            return NonceClaimResult.Claimed;
        }
    }

    private void DropExpired(DateTimeOffset now)
    {
        // A proof is acceptable up to and including signedAt + RequestProofMaxAge, so its nonce is kept through that
        // instant and dropped only after it.
        while (_expiries.TryPeek(out string? key, out DateTimeOffset expiresAt) && expiresAt < now)
        {
            _expiries.Dequeue();
            if (_claimed.Remove(key, out Claim claim))
            {
                int left = _perScope[claim.Scope] - 1;
                if (left == 0)
                {
                    _perScope.Remove(claim.Scope);
                }
                else
                {
                    _perScope[claim.Scope] = left;
                }
            }
        }
    }

    private readonly record struct Claim(string Scope, DateTimeOffset ExpiresAt);
}
