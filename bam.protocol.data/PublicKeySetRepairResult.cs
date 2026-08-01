using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Data;

/// <summary>
/// The verified outcome of a <see cref="IPublicKeySetAudit.Repair"/> pass.  Every archived row
/// was CONFIRMED gone from live storage after archiving (a retrieve by identity returns
/// nothing) — an archiver that reports success without moving bytes lands the row in
/// <see cref="Failed"/>, never <see cref="Archived"/> — so <see cref="Succeeded"/> can be
/// trusted by operators deciding whether the misattribution risk is actually repaired.
/// </summary>
/// <param name="Archived">Rows removed from live storage (bytes preserved under the store's archive area), verified post-archive.</param>
/// <param name="Failed">Rows the repair could NOT remove from live storage (archive error, or archive reported success while the row still retrieves); these remain live and resolvable — re-run the audit after addressing the cause.</param>
/// <param name="FingerprintsStamped">The number of legacy rows whose missing canonical key fingerprints were stamped during this pass (the index-migration step).</param>
public sealed record PublicKeySetRepairResult(
    IReadOnlyList<PublicKeySetData> Archived,
    IReadOnlyList<PublicKeySetData> Failed,
    int FingerprintsStamped)
{
    /// <summary>
    /// Gets a value indicating whether every targeted row was verifiably removed from live
    /// storage.
    /// </summary>
    public bool Succeeded => Failed.Count == 0;
}
