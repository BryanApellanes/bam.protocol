using System.Collections.Concurrent;
using Bam.Encryption;

namespace Bam.Protocol.Profile;

/// <summary>
/// Named key storage held in process memory (bam.protocol#119). Single-process and non-persistent: every key is
/// lost when the process ends, so a host that generates its server key into it at start-up gets a new key at
/// each restart, and tokens it issued before the restart stop verifying. Safe from several threads. Bytes are
/// copied in and out, so a caller that clears its own array doesn't clear the stored key.
/// </summary>
public sealed class MemoryNamedKeyStorage : INamedKeyStorage
{
    private readonly ConcurrentDictionary<string, byte[]> _keys = new ConcurrentDictionary<string, byte[]>(StringComparer.Ordinal);

    /// <inheritdoc />
    /// <exception cref="ArgumentException"><paramref name="keyName"/> is null, empty or whitespace.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="keyBytes"/> is null.</exception>
    public bool SaveNamedKey(string keyName, byte[] keyBytes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyName);
        ArgumentNullException.ThrowIfNull(keyBytes);
        _keys[keyName] = (byte[])keyBytes.Clone();
        return true;
    }

    /// <inheritdoc />
    public byte[]? GetNamedKey(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        return _keys.TryGetValue(name, out byte[]? bytes) ? (byte[])bytes.Clone() : null;
    }
}
