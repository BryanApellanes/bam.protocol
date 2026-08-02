namespace Bam.Protocol.Data;

/// <summary>
/// Detects and repairs duplicate key-set rows in an existing store (bam.protocol#13).  The
/// live registration and rotation paths enforce one-row-per-handle and
/// one-handle-per-key-material (by canonical key identity), so duplicates only exist in stores
/// written before those invariants (legacy data) or tampered with directly; while present,
/// deterministic resolution (<see cref="IPublicKeySetResolver"/>) bounds them — the
/// earliest-created row always wins — and this audit removes them.  Revoked tombstones
/// (<see cref="Profile.PublicKeySetData.RevokedUtc"/>) are outside the audit's universe: they
/// are never grouped, never counted as duplicates, and never archived — their material
/// blocklist role must persist, and a tombstone coexisting with an active row for the same
/// handle is the legal post-revocation re-registration shape (bam.protocol#11).  Detection is
/// read-only; repair is a separate, explicitly invoked operation that ARCHIVES the losing rows
/// (removes them from live storage while preserving their bytes) rather than deleting them, so
/// a repair is manually reversible.
/// </summary>
public interface IPublicKeySetAudit
{
    /// <summary>
    /// Scans every ACTIVE stored key set once and reports all duplicate-handle and
    /// shared-key-material groups (material compared by canonical identity), identifying the
    /// authoritative row (earliest <c>Created</c>, then <c>Id</c>) per group.  Read-only —
    /// never modifies the store.
    /// </summary>
    /// <returns>The duplicate report; <see cref="PublicKeySetDuplicateReport.HasDuplicates"/> is false for a healthy store.</returns>
    PublicKeySetDuplicateReport FindDuplicates();

    /// <summary>
    /// Repairs the duplicates the report describes and returns the verified outcome.  Repair
    /// proceeds in dependency order so it can never leave a handle with zero live rows:
    /// shared-material losers are archived first, then duplicate-handle groups are re-evaluated
    /// against the LIVE post-archive store and only rows still duplicated are archived — a row
    /// that has become its handle's sole survivor is never archived, regardless of what the
    /// report said.  Every archive is verified post-hoc (the row must no longer retrieve);
    /// unverifiable archives are reported as failures, logged at Error, and left for a re-run.
    /// The pass also stamps missing canonical key fingerprints on legacy rows (the
    /// index-migration step) and finally rebuilds the key-set search index so no index entry
    /// references an archived row.  Callers obtain the report from
    /// <see cref="FindDuplicates"/>, review it, and pass it here — the split keeps "what would
    /// be repaired" inspectable before anything changes.
    /// </summary>
    /// <param name="report">The duplicate report to repair from.</param>
    /// <returns>The verified repair outcome; see <see cref="PublicKeySetRepairResult"/>.</returns>
    PublicKeySetRepairResult Repair(PublicKeySetDuplicateReport report);
}
