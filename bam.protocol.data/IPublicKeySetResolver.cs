using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Data;

/// <summary>
/// Owns deterministic, uniqueness-aware resolution of registered public key sets — by handle or
/// by key material (bam.protocol#13).  Resolution is ALWAYS deterministic: when duplicate rows
/// exist (legacy data or direct store tampering), the earliest-created row wins (ordered by
/// <c>Created</c> then <c>Id</c>), so a later-added row can never displace the first
/// registration.  A row whose <c>Created</c> is null (legacy data written before server
/// stamping) sorts BEFORE every stamped row — a deliberate policy: an unstamped legacy row is
/// treated as the oldest claim rather than silently losing to newer stamped rows.  Key material
/// is matched by CANONICAL identity (see the fingerprint properties on
/// <see cref="PublicKeySetData"/>), never raw PEM strings.  Revoked tombstones
/// (<see cref="PublicKeySetData.RevokedUtc"/>) are never authoritative: the resolve members
/// skip them (bam.protocol#13's revoked-skip — a revoked key must not resolve for session
/// attribution), while <see cref="FindKeyMaterialClaims"/> surfaces them so admission policy
/// can enforce the material blocklist.
/// <para>
/// Implementations back their lookups with the object store's search index; indexed results
/// reflect the index (see the store's consistency contract — rows never indexed are invisible
/// here).  Admission decisions must confirm empty results by scan; the read path accepts
/// indexed authority for speed.  Composed into <see cref="IPublicKeySetRegistrar"/>
/// implementations and <see cref="IProfileRepository"/> implementations so both share one
/// resolution authority.
/// </para>
/// </summary>
public interface IPublicKeySetResolver
{
    /// <summary>
    /// Finds the authoritative ACTIVE key set registered for a handle, or null when none is
    /// registered (revoked tombstones are skipped — a revoked handle is free and its tombstone
    /// never resolves).  When duplicate active rows exist for the handle, the earliest-created
    /// row (by <c>Created</c> then <c>Id</c>) is authoritative.  Handle comparison is ordinal
    /// and case-sensitive.
    /// </summary>
    /// <param name="keySetHandle">The handle to resolve.</param>
    /// <returns>The authoritative active key set, or null when none is registered.</returns>
    PublicKeySetData? ResolveByHandle(string keySetHandle);

    /// <summary>
    /// Finds the authoritative ACTIVE key set carrying the specified public key material,
    /// matched by canonical identity against both key fields, or null when the material is not
    /// actively registered (revoked tombstones never resolve — bam.protocol#13's revoked-skip).
    /// When more than one active row carries the material, the earliest-created row wins; when
    /// those rows span more than one handle, a warning naming the handles is logged — that
    /// state is a misattribution risk that <see cref="IPublicKeySetAudit"/> exists to repair.
    /// </summary>
    /// <param name="publicKeyPem">The PEM-encoded public key material to resolve.</param>
    /// <returns>The authoritative active key set carrying the material, or null when none does.</returns>
    PublicKeySetData? ResolveByKeyMaterial(string publicKeyPem);

    /// <summary>
    /// Returns every stored row — ACTIVE or REVOKED — whose material shares canonical identity
    /// with any of the candidate's key fields, EXCLUDING the candidate handle's own active row
    /// (re-presenting material a handle actively owns is not a claim against it; its revoked
    /// tombstones ARE included so blocklisted material is always surfaced).  Ordered
    /// deterministically (<c>Created</c> then <c>Id</c>).  Backs the registrar's admission
    /// policy: the one-handle-to-one-key-material invariant and the revoked-material blocklist.
    /// </summary>
    /// <param name="candidate">The key set whose material to check, carrying the handle whose active row is exempt.</param>
    /// <returns>The matching rows, earliest first; empty when the material is claimed nowhere else.</returns>
    IEnumerable<PublicKeySetData> FindKeyMaterialClaims(PublicKeySetData candidate);
}
