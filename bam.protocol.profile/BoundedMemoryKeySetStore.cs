using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Profile;

/// <summary>
/// An in-memory key-set store for hosts whose actors live for one visit, such as a public demo
/// (bam.protocol#119). It implements both <see cref="IPublicKeySetResolver"/> and <see cref="IPublicKeySetRegistrar"/>,
/// so one instance backs registration and resolution.
/// <para>
/// It is single-process and keeps nothing across a restart. Every key set expires a fixed
/// <see cref="BoundedMemoryKeySetStoreOptions.KeySetLifetime"/> after it is registered, and the store holds at most
/// <see cref="BoundedMemoryKeySetStoreOptions.Capacity"/> live key sets, so memory stays bounded whatever callers do.
/// </para>
/// <para>
/// Registration validates key material exactly as <see cref="PublicKeySetRegistrar"/> does (at least one key,
/// each parseable and of its slot's kind; an ECC-only key set is accepted). First registration wins over live key
/// sets, and key material maps to one live handle by canonical identity. An expired or removed key set is dropped
/// rather than kept as a tombstone: it never resolves again, and its handle and material are free to register
/// again. That is the difference from the persistent store, which blocklists revoked material. This store has no
/// revocation, so nothing is blocklisted.
/// </para>
/// <para>
/// Rotation isn't offered. A key set lives for one visit, so a visitor registers a new handle instead;
/// <see cref="Rotate"/> refuses with <see cref="InvalidKeySetRotationException"/>.
/// </para>
/// <para>
/// Safe from several threads: one lock covers every read and write. Key sets are copied in and out, so a caller
/// can't change a stored key set through an object it holds.
/// </para>
/// </summary>
public sealed class BoundedMemoryKeySetStore : IPublicKeySetResolver, IPublicKeySetRegistrar
{
    private readonly object _lock = new object();
    private readonly Dictionary<string, StoredKeySet> _byHandle = new Dictionary<string, StoredKeySet>(StringComparer.Ordinal);
    private readonly TimeProvider _time;

    /// <summary>
    /// Creates a store with the given bounds.
    /// </summary>
    /// <param name="options">The capacity and key-set lifetime; defaults when null.</param>
    /// <param name="timeProvider">The clock that measures lifetimes; <see cref="TimeProvider.System"/> when null.</param>
    /// <exception cref="ArgumentOutOfRangeException">The capacity is below 1, or the lifetime is not positive.</exception>
    public BoundedMemoryKeySetStore(BoundedMemoryKeySetStoreOptions? options = null, TimeProvider? timeProvider = null)
    {
        Options = options ?? new BoundedMemoryKeySetStoreOptions();
        if (Options.Capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(options), Options.Capacity, "Capacity must be at least 1.");
        }
        if (Options.KeySetLifetime <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(options), Options.KeySetLifetime, "KeySetLifetime must be positive.");
        }
        _time = timeProvider ?? TimeProvider.System;
    }

    /// <summary>Gets the bounds in force.</summary>
    public BoundedMemoryKeySetStoreOptions Options { get; }

    /// <summary>Gets how many live key sets the store holds now.</summary>
    public int Count
    {
        get
        {
            lock (_lock)
            {
                SweepExpired(_time.GetUtcNow());
                return _byHandle.Count;
            }
        }
    }

    /// <inheritdoc />
    /// <exception cref="KeySetCapacityExceededException">The store already holds <see cref="BoundedMemoryKeySetStoreOptions.Capacity"/> live key sets.</exception>
    public PublicKeySetData Register(PublicKeySetData publicKeySetData)
    {
        ArgumentNullException.ThrowIfNull(publicKeySetData);
        string handle = publicKeySetData.KeySetHandle;
        if (string.IsNullOrWhiteSpace(handle))
        {
            throw new ArgumentException("A key set handle is required.", nameof(publicKeySetData));
        }
        if (string.IsNullOrEmpty(publicKeySetData.PublicRsaKey) && string.IsNullOrEmpty(publicKeySetData.PublicEccKey))
        {
            throw new InvalidPublicKeySetException(handle,
                $"A key set for handle '{handle}' must contain at least one public key (RSA or ECC).", null);
        }

        ValueTuple<string, Exception>? unparseable = PublicKeySetRegistrar.FindUnparseableKey(publicKeySetData);
        if (unparseable != null)
        {
            throw new InvalidPublicKeySetException(handle,
                $"The {unparseable.Value.Item1} public key presented for handle '{handle}' is not a parseable public key.",
                unparseable.Value.Item2);
        }

        string? mistypedSlot = PublicKeySetRegistrar.FindMistypedSlot(publicKeySetData);
        if (mistypedSlot != null)
        {
            throw new InvalidPublicKeySetException(handle,
                $"The {mistypedSlot} slot of the key set presented for handle '{handle}' does not hold {PublicKeySetRegistrar.DescribeExpectedKey(mistypedSlot)}.", null);
        }

        PublicKeySetData candidate = Copy(publicKeySetData);
        candidate.PublicRsaKey ??= string.Empty;
        candidate.PublicEccKey ??= string.Empty;
        PublicKeySetRegistrar.NormalizeServerControlledFields(candidate);
        candidate.LastRotatedUtc = null;
        IReadOnlyList<string> identities = KeyMaterialIdentity.CandidateIdentities(candidate);

        lock (_lock)
        {
            DateTimeOffset now = _time.GetUtcNow();
            SweepExpired(now);
            if (_byHandle.ContainsKey(handle))
            {
                throw new PublicKeySetConflictException(handle);
            }

            foreach (StoredKeySet stored in _byHandle.Values)
            {
                if (KeyMaterialIdentity.SharesMaterial(stored.KeySet, identities))
                {
                    throw new PublicKeySetKeyMaterialConflictException(handle, stored.KeySet.KeySetHandle);
                }
            }

            if (_byHandle.Count >= Options.Capacity)
            {
                // Not logged: a flood would write one line per refusal. The host sees the 429s.
                throw new KeySetCapacityExceededException(handle, Options.Capacity);
            }

            candidate.Created = now.UtcDateTime;
            _byHandle[handle] = new StoredKeySet(candidate, now + Options.KeySetLifetime);
            return Copy(candidate);
        }
    }

    /// <summary>
    /// Refuses: this store doesn't rotate key sets (see the type's remarks).
    /// </summary>
    /// <param name="newKeySet">The key set to rotate to.</param>
    /// <param name="rotationSignature">The rotation proof.</param>
    /// <returns>Never returns.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="newKeySet"/> is null.</exception>
    /// <exception cref="InvalidKeySetRotationException">Always.</exception>
    public PublicKeySetData Rotate(PublicKeySetData newKeySet, byte[] rotationSignature)
    {
        ArgumentNullException.ThrowIfNull(newKeySet);
        throw new InvalidKeySetRotationException(newKeySet.KeySetHandle ?? string.Empty,
            "The in-memory key-set store doesn't rotate key sets; register a new handle instead.");
    }

    /// <inheritdoc />
    public PublicKeySetData? Resolve(string keySetHandle)
    {
        return ResolveByHandle(keySetHandle);
    }

    /// <inheritdoc />
    public PublicKeySetData? ResolveByHandle(string keySetHandle)
    {
        if (string.IsNullOrEmpty(keySetHandle))
        {
            return null;
        }
        lock (_lock)
        {
            SweepExpired(_time.GetUtcNow());
            return _byHandle.TryGetValue(keySetHandle, out StoredKeySet? stored) ? Copy(stored.KeySet) : null;
        }
    }

    /// <inheritdoc />
    public PublicKeySetData? ResolveByKeyMaterial(string publicKeyPem)
    {
        if (string.IsNullOrEmpty(publicKeyPem))
        {
            return null;
        }
        IReadOnlyList<string> identities = KeyMaterialIdentity.CandidateIdentities(new PublicKeySetData { KeySetHandle = string.Empty, PublicRsaKey = publicKeyPem, PublicEccKey = string.Empty });
        lock (_lock)
        {
            SweepExpired(_time.GetUtcNow());
            foreach (StoredKeySet stored in _byHandle.Values)
            {
                if (KeyMaterialIdentity.SharesMaterial(stored.KeySet, identities))
                {
                    return Copy(stored.KeySet);
                }
            }
            return null;
        }
    }

    /// <inheritdoc />
    public IEnumerable<PublicKeySetData> FindKeyMaterialClaims(PublicKeySetData candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        IReadOnlyList<string> identities = KeyMaterialIdentity.CandidateIdentities(candidate);
        List<PublicKeySetData> claims = new List<PublicKeySetData>();
        lock (_lock)
        {
            SweepExpired(_time.GetUtcNow());
            foreach (StoredKeySet stored in _byHandle.Values)
            {
                if (!string.Equals(stored.KeySet.KeySetHandle, candidate.KeySetHandle, StringComparison.Ordinal)
                    && KeyMaterialIdentity.SharesMaterial(stored.KeySet, identities))
                {
                    claims.Add(Copy(stored.KeySet));
                }
            }
        }
        return claims.OrderBy(claim => claim.Created).ThenBy(claim => claim.Uuid, StringComparer.Ordinal).ToList();
    }

    /// <summary>
    /// Removes a handle's key set at once, before it expires. It never resolves again, and the handle and its
    /// key material are free to register again.
    /// </summary>
    /// <param name="keySetHandle">The handle to remove.</param>
    /// <returns>True when a live key set was removed; false when there was none.</returns>
    public bool Remove(string keySetHandle)
    {
        if (string.IsNullOrEmpty(keySetHandle))
        {
            return false;
        }
        lock (_lock)
        {
            SweepExpired(_time.GetUtcNow());
            return _byHandle.Remove(keySetHandle);
        }
    }

    /// <summary>Drops every key set whose lifetime has passed. Call only while holding the lock.</summary>
    private void SweepExpired(DateTimeOffset now)
    {
        List<string>? expired = null;
        foreach (KeyValuePair<string, StoredKeySet> entry in _byHandle)
        {
            if (now >= entry.Value.ExpiresUtc)
            {
                (expired ??= new List<string>()).Add(entry.Key);
            }
        }
        if (expired == null)
        {
            return;
        }
        foreach (string handle in expired)
        {
            _byHandle.Remove(handle);
        }
    }

    private static PublicKeySetData Copy(PublicKeySetData source)
    {
        return new PublicKeySetData
        {
            KeySetHandle = source.KeySetHandle,
            PublicRsaKey = source.PublicRsaKey,
            PublicEccKey = source.PublicEccKey,
            PublicRsaKeyFingerprint = source.PublicRsaKeyFingerprint,
            PublicEccKeyFingerprint = source.PublicEccKeyFingerprint,
            RevokedUtc = source.RevokedUtc,
            RevokedBy = source.RevokedBy,
            LastRotatedUtc = source.LastRotatedUtc,
            AuthorizedSuccessorFingerprint = source.AuthorizedSuccessorFingerprint,
            Created = source.Created,
            Uuid = source.Uuid,
            Cuid = source.Cuid,
            Id = source.Id,
        };
    }

    private sealed record StoredKeySet(PublicKeySetData KeySet, DateTimeOffset ExpiresUtc);
}
