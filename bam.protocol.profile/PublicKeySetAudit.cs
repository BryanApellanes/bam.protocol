using Bam.Data.Objects;
using Bam.Encryption;
using Bam.Logging;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Profile;

/// <summary>
/// Default <see cref="IPublicKeySetAudit"/>: one full-store pass groups ACTIVE key-set rows by
/// handle and by canonical key-material identity (see <see cref="KeyMaterialIdentity"/> —
/// revoked tombstones are excluded from the audit universe), orders every group by
/// <c>Created</c> then <c>Id</c> (mirroring <see cref="PublicKeySetResolver"/>, so the audit's
/// "authoritative" row is exactly the row resolution returns), and repair archives the losers
/// via <see cref="IObjectDataArchiver"/> with post-archive verification before rebuilding the
/// key-set search index.  A full scan — not indexed lookups — is correct here: the audit exists
/// precisely for stores whose index may predate or contradict the invariants, and it runs as an
/// explicit operator action, not on a request path.
/// </summary>
public class PublicKeySetAudit : IPublicKeySetAudit
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PublicKeySetAudit"/> class.
    /// </summary>
    /// <param name="repository">The object-data repository the key sets are persisted in.</param>
    /// <param name="archiver">The archiver non-authoritative rows are moved out of live storage with.</param>
    /// <param name="searchIndexer">The search indexer whose key-set index is rebuilt after a repair.</param>
    public PublicKeySetAudit(ObjectDataRepository repository, IObjectDataArchiver archiver, IObjectDataSearchIndexer searchIndexer)
    {
        this.Repository = repository;
        this.Archiver = archiver;
        this.SearchIndexer = searchIndexer;
    }

    /// <summary>
    /// Gets the object-data repository the key sets are persisted in.
    /// </summary>
    protected ObjectDataRepository Repository { get; }

    /// <summary>
    /// Gets the archiver non-authoritative rows are moved out of live storage with.
    /// </summary>
    protected IObjectDataArchiver Archiver { get; }

    /// <summary>
    /// Gets the search indexer whose key-set index is rebuilt after a repair.
    /// </summary>
    protected IObjectDataSearchIndexer SearchIndexer { get; }

    /// <inheritdoc />
    public PublicKeySetDuplicateReport FindDuplicates()
    {
        List<PublicKeySetData> activeKeySets = Repository.RetrieveAll<PublicKeySetData>()
            .Where(keySet => keySet.RevokedUtc == null)
            .ToList();

        return BuildReport(activeKeySets);
    }

    /// <inheritdoc />
    public PublicKeySetRepairResult Repair(PublicKeySetDuplicateReport report)
    {
        ArgumentNullException.ThrowIfNull(report);

        List<PublicKeySetData> archived = new List<PublicKeySetData>();
        List<PublicKeySetData> failed = new List<PublicKeySetData>();

        // Phase 1: shared-material losers. Cross-handle duplicate material is the
        // misattribution vector — it repairs first.
        foreach (PublicKeySetDuplicateGroup group in report.SharedKeyMaterialGroups)
        {
            foreach (PublicKeySetData loser in group.NonAuthoritative)
            {
                ArchiveVerified(loser, archived, failed);
            }
        }

        // Phase 2: duplicate handles, RE-EVALUATED against the live post-archive store — a row
        // already archived in phase 1 (or a row that phase 1 left as its handle's sole
        // survivor) is never targeted, so repair cannot annihilate a handle
        // (bam.protocol#24 review SF4).
        List<PublicKeySetData> liveActive = Repository.RetrieveAll<PublicKeySetData>()
            .Where(keySet => keySet.RevokedUtc == null)
            .ToList();
        foreach (IGrouping<string, PublicKeySetData> handleGroup in liveActive.GroupBy(keySet => keySet.KeySetHandle))
        {
            List<PublicKeySetData> ordered = handleGroup
                .OrderBy(keySet => keySet.Created)
                .ThenBy(keySet => keySet.Id)
                .ToList();
            foreach (PublicKeySetData loser in ordered.Skip(1))
            {
                ArchiveVerified(loser, archived, failed);
            }
        }

        // Phase 3: stamp missing canonical fingerprints on surviving legacy rows so indexed
        // canonical lookups cover them (the index-migration step; bam.protocol#24 review B1).
        int fingerprintsStamped = 0;
        foreach (PublicKeySetData survivor in Repository.RetrieveAll<PublicKeySetData>())
        {
            bool needsRsa = !string.IsNullOrEmpty(survivor.PublicRsaKey) && survivor.PublicRsaKeyFingerprint == null;
            bool needsEcc = !string.IsNullOrEmpty(survivor.PublicEccKey) && survivor.PublicEccKeyFingerprint == null;
            if (!needsRsa && !needsEcc)
            {
                continue;
            }

            KeyMaterialIdentity.StampFingerprints(survivor);
            if (survivor.PublicRsaKeyFingerprint != null || survivor.PublicEccKeyFingerprint != null)
            {
                Repository.Update(survivor);
                fingerprintsStamped++;
            }
        }

        SearchIndexer.RebuildAsync<PublicKeySetData>().GetAwaiter().GetResult();

        return new PublicKeySetRepairResult(archived, failed, fingerprintsStamped);
    }

    /// <summary>
    /// Archives a loser and VERIFIES it no longer retrieves from live storage before counting
    /// it archived — <c>ArchiveAsync</c> reports success even when nothing existed to move, so
    /// a mislocated archiver would otherwise yield "repaired" without repair
    /// (bam.protocol#24 review SF3).  Failures log at Error and land in
    /// <paramref name="failed"/>.
    /// </summary>
    private void ArchiveVerified(PublicKeySetData loser, List<PublicKeySetData> archived, List<PublicKeySetData> failed)
    {
        IObjectDataArchiveResult result;
        try
        {
            result = Archiver.ArchiveAsync(loser).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Log.Error("Archive FAILED for non-authoritative key-set row (handle '{0}', Uuid {1}): {2}", ex, loser.KeySetHandle, loser.Uuid, ex.Message);
            failed.Add(loser);
            return;
        }

        if (!result.Success)
        {
            Log.Error("Archive FAILED for non-authoritative key-set row (handle '{0}', Uuid {1}): {2}", loser.KeySetHandle, loser.Uuid, result.Message);
            failed.Add(loser);
            return;
        }

        PublicKeySetData? stillLive = Repository.Retrieve<PublicKeySetData>(loser.Uuid);
        if (stillLive != null)
        {
            Log.Error(
                "Archive reported success but the row STILL RETRIEVES from live storage (handle '{0}', Uuid {1}) — archiver/store mislocation suspected; row counted as failed.",
                loser.KeySetHandle,
                loser.Uuid);
            failed.Add(loser);
            return;
        }

        Log.Warn(
            "Archived non-authoritative key-set row for handle '{0}' (Uuid {1}) to '{2}' during duplicate repair.",
            loser.KeySetHandle,
            loser.Uuid,
            result.ArchivePath);
        archived.Add(loser);
    }

    /// <summary>
    /// Builds the duplicate report from the active rows: same-handle groups, and canonical
    /// material identities claimed by more than one handle.
    /// </summary>
    private static PublicKeySetDuplicateReport BuildReport(List<PublicKeySetData> activeKeySets)
    {
        List<PublicKeySetDuplicateGroup> duplicateHandleGroups = activeKeySets
            .GroupBy(keySet => keySet.KeySetHandle)
            .Where(group => group.Count() > 1)
            .Select(group => ToGroup(group.Key, group))
            .ToList();

        Dictionary<string, List<PublicKeySetData>> rowsByIdentity = new Dictionary<string, List<PublicKeySetData>>();
        foreach (PublicKeySetData keySet in activeKeySets)
        {
            foreach (string identity in KeyMaterialIdentity.RowIdentities(keySet))
            {
                if (!rowsByIdentity.TryGetValue(identity, out List<PublicKeySetData>? rows))
                {
                    rows = new List<PublicKeySetData>();
                    rowsByIdentity[identity] = rows;
                }
                if (!rows.Contains(keySet))
                {
                    rows.Add(keySet);
                }
            }
        }

        List<PublicKeySetDuplicateGroup> sharedKeyMaterialGroups = rowsByIdentity
            .Where(entry => entry.Value.Select(keySet => keySet.KeySetHandle).Distinct().Count() > 1)
            .Select(entry => ToGroup(entry.Key.Sha256(), entry.Value))
            .ToList();

        return new PublicKeySetDuplicateReport(duplicateHandleGroups, sharedKeyMaterialGroups);
    }

    /// <summary>
    /// Builds a group from rows sharing a key, ordering by <c>Created</c> then <c>Id</c> so the
    /// authoritative row is exactly the one deterministic resolution returns.
    /// </summary>
    private static PublicKeySetDuplicateGroup ToGroup(string groupKey, IEnumerable<PublicKeySetData> rows)
    {
        List<PublicKeySetData> ordered = rows
            .OrderBy(keySet => keySet.Created)
            .ThenBy(keySet => keySet.Id)
            .ToList();

        return new PublicKeySetDuplicateGroup(groupKey, ordered[0], ordered.Skip(1).ToList());
    }
}
