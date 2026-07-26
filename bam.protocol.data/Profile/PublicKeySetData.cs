using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;


public class PublicKeySetData : RepoData, IKeySet
{
    public string KeySetHandle { get; set; } = null!;

    public string PublicRsaKey { get; set; } = null!;
    public string PublicEccKey { get; set; } = null!;

    /// <summary>
    /// The UTC time this key set was revoked, or null when it is active.  A revoked row is a
    /// tombstone: it is no longer authoritative (resolution and device-key confirmation skip it),
    /// its handle is free to re-register, but its key material stays blocklisted so a compromised
    /// key cannot be re-registered.  Revocation is authorized by a break-glass admin proof; see
    /// <see cref="IKeySetRevocation"/> and bam.protocol#11.
    /// </summary>
    public DateTime? RevokedUtc { get; set; }

    /// <summary>
    /// Identifies the break-glass admin key that authorized the revocation (the SHA-256 of the
    /// admin public key), for the audit trail, or null when the key set is active.
    /// </summary>
    public string? RevokedBy { get; set; }
}