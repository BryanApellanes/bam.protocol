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

    /// <summary>
    /// The canonical fingerprint (<see cref="Bam.Protocol.Profile.PublicKeyFingerprint"/>) of the
    /// key the break-glass admin authorized to re-register this handle after revocation, or null
    /// when the revocation bound no successor (leaving the freed handle openly re-registrable, the
    /// pre-successor-binding behavior).  When set on the governing tombstone, re-registration of the
    /// handle is gated on the candidate's RSA identity key matching this fingerprint — closing the
    /// revoke→re-register hijack window (bam.protocol#21).  The binding is covered by the same admin
    /// proof that authorizes the revocation (a fourth field of the signed <see cref="RevocationPayload"/>).
    /// </summary>
    public string? AuthorizedSuccessorFingerprint { get; set; }
}