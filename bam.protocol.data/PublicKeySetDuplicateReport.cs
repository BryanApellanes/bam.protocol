using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Data;

/// <summary>
/// One group of duplicate key-set rows sharing a handle or a piece of public key material,
/// with the authoritative row (earliest <c>Created</c>, then <c>Id</c> — the row resolution
/// returns) separated from the non-authoritative rows a repair would archive.
/// </summary>
/// <param name="GroupKey">What the rows collide on: the shared handle for duplicate-handle groups, or the SHA-256 digest of the shared PEM for shared-key-material groups.</param>
/// <param name="Authoritative">The row that wins deterministic resolution.</param>
/// <param name="NonAuthoritative">The rows deterministic resolution never returns; the repair targets.</param>
public sealed record PublicKeySetDuplicateGroup(
    string GroupKey,
    PublicKeySetData Authoritative,
    IReadOnlyList<PublicKeySetData> NonAuthoritative);

/// <summary>
/// The result of a <see cref="IPublicKeySetAudit.FindDuplicates"/> pass over the key-set
/// store: every violation of the one-row-per-handle and one-handle-per-key-material
/// invariants, grouped with the authoritative row identified per group.  Duplicates can only
/// arise from legacy data (rows written before the invariants were enforced) or direct store
/// tampering — the live registration and rotation paths reject them.
/// </summary>
/// <param name="DuplicateHandleGroups">Groups of rows registered under the same handle (violates one-row-per-handle).</param>
/// <param name="SharedKeyMaterialGroups">Groups of rows spanning MORE THAN ONE handle that carry the same public key material (violates one-handle-per-key-material — the misattribution vector).</param>
public sealed record PublicKeySetDuplicateReport(
    IReadOnlyList<PublicKeySetDuplicateGroup> DuplicateHandleGroups,
    IReadOnlyList<PublicKeySetDuplicateGroup> SharedKeyMaterialGroups)
{
    /// <summary>
    /// Gets a value indicating whether any duplicate groups were found.
    /// </summary>
    public bool HasDuplicates => DuplicateHandleGroups.Count > 0 || SharedKeyMaterialGroups.Count > 0;
}
