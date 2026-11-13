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
    /// The UTC time this row's current key material was set: at registration, and again at each rotation.  Owned by
    /// the registrar (caller-supplied values are overwritten); it is what a minimum rotation interval is measured from
    /// (bam.protocol#89).  Null on a row that predates the field, for which the registrar uses <c>Created</c>.
    /// </summary>
    public DateTime? KeyMaterialSinceUtc { get; set; }

    /// <summary>
    /// Identifies the break-glass admin key that authorized the revocation (the SHA-256 of the
    /// admin public key), for the audit trail, or null when the key set is active.
    /// </summary>
    public string? RevokedBy { get; set; }

    /// <summary>
    /// The <see cref="RevokedBy"/> prefix of a rotation tombstone: a revoked row the registrar
    /// writes when a rotation retires keys, holding only the retired keys, followed by the name of
    /// the proof that authorized the rotation (for example <c>rotation:passkey</c>).  It
    /// blocklists the retired material but never governs re-registration of the handle
    /// (bam.protocol#72).  An admin revocation's <see cref="RevokedBy"/> is a hex SHA-256 and can't
    /// start with it.
    /// </summary>
    public const string RotationRevokedByPrefix = "rotation:";

    /// <summary>
    /// Returns whether <paramref name="keySet"/> is a rotation tombstone (see <see cref="RotationRevokedByPrefix"/>).
    /// </summary>
    /// <param name="keySet">The row to check.</param>
    /// <returns>True for a revoked row written by a rotation.</returns>
    public static bool IsRotationTombstone(PublicKeySetData keySet)
    {
        return keySet.RevokedUtc != null && keySet.RevokedBy != null && keySet.RevokedBy.StartsWith(RotationRevokedByPrefix, StringComparison.Ordinal);
    }

    /// <summary>
    /// Server-stamped canonical identity of <see cref="PublicRsaKey"/> — the SHA-256 of the
    /// parsed key's canonical DER <c>SubjectPublicKeyInfo</c> — or null when the field is empty,
    /// unparseable, or the row predates fingerprint stamping (legacy; the key-set audit stamps
    /// missing fingerprints during repair).  Being a plain string property, it is search-indexed
    /// by the object store, which is what lets key-material lookups be both indexed AND
    /// re-encoding-independent (raw PEM equality is bypassable — bam.protocol#18 C1).  Owned by
    /// the registrar (stamped at registration and rotation); caller-supplied values are
    /// overwritten.
    /// </summary>
    public string? PublicRsaKeyFingerprint { get; set; }

    /// <summary>
    /// Server-stamped canonical identity of <see cref="PublicEccKey"/>; see
    /// <see cref="PublicRsaKeyFingerprint"/> for semantics and ownership.
    /// </summary>
    public string? PublicEccKeyFingerprint { get; set; }

    /// <summary>
    /// The canonical fingerprint (<see cref="Bam.Protocol.Profile.PublicKeyFingerprint"/>) of the
    /// key the break-glass admin authorized to re-register this handle after revocation, or null
    /// when the revocation bound no successor (leaving the freed handle openly re-registrable, the
    /// pre-successor-binding behavior).  When set on the governing tombstone, re-registration of the
    /// handle is gated on the candidate's RSA identity key matching this fingerprint — closing the
    /// revoke→re-register hijack window (bam.protocol#21).  The binding is covered by the same admin
    /// proof that authorizes the revocation (a fourth field of the signed <see cref="RevocationPayload"/>).
    /// Unlike <see cref="PublicRsaKeyFingerprint"/>/<see cref="PublicEccKeyFingerprint"/> (identity of
    /// THIS row's own material), this is the identity of a DIFFERENT, future key; it is set only on a
    /// revoked tombstone and is server-normalized so a fresh registration can never carry it (the
    /// registrar clears caller-supplied revocation fields — bam.protocol#25 review B1).
    /// <para>
    /// The gate authenticates <b>presentation of the bound public key</b>, not proof of possession, and
    /// binds only the RSA identity key: the successor's public key must stay unpublished until the handle
    /// is claimed, and a claimant may attach their own ECC material (recover via rotation).  See the
    /// key-set README's successor-binding section (bam.protocol#25 review SF1 / Condition 4).
    /// </para>
    /// </summary>
    public string? AuthorizedSuccessorFingerprint { get; set; }
}
