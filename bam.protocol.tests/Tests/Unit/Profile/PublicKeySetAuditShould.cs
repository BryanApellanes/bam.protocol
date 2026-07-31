using Bam;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Objects;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile;
using Bam.Storage;
using Bam.Test;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("PublicKeySetAudit Should", Selector = "pksa")]
public class PublicKeySetAuditShould : UnitTestMenuContainer
{
    private sealed record AuditFixture(ObjectDataRepository Repository, PublicKeySetAudit Audit, PublicKeySetResolver Resolver);

    private static AuditFixture CreateFixture(string testName)
    {
        string rootPath = $"./.bam/tests/{testName}";
        // each run starts from a clean store so planted-duplicate scenarios are repeatable
        if (Directory.Exists(rootPath))
        {
            Directory.Delete(rootPath, true);
        }
        AesKey aesKey = new AesKey();
        ICompositeKeyCalculator compositeKeyCalculator = new CompositeKeyCalculator();
        IObjectDataIdentityCalculator identityCalculator = new ObjectDataIdentityCalculator();
        IObjectDataLocatorFactory locatorFactory = new ObjectDataLocatorFactory(identityCalculator);
        IObjectEncoderDecoder encoderDecoder = new JsonObjectDataEncoder();
        IObjectDataFactory factory = new ObjectDataFactory(locatorFactory, encoderDecoder);
        IRootStorageHolder rootStorage = new RootStorageHolder(rootPath);
        IObjectDataStorageManager storageManager = new EncryptedFsObjectDataStorageManager(rootStorage, factory, new AesEncryptor(aesKey), new AesDecryptor(aesKey));
        IObjectDataWriter writer = new ObjectDataWriter(factory, storageManager);
        IObjectDataReader reader = new ObjectDataReader(storageManager);
        IObjectDataIndexer indexer = new ObjectDataIndexer(storageManager, compositeKeyCalculator);
        IObjectDataSearchIndexer searchIndexer = new ObjectDataSearchIndexer(storageManager, indexer);
        IObjectDataSearcher searcher = new ObjectDataSearcher(searchIndexer, reader, indexer);
        IObjectDataDeleter deleter = new ObjectDataDeleter(factory, storageManager, compositeKeyCalculator);
        IObjectDataArchiver archiver = new ObjectDataArchiver(factory, storageManager, compositeKeyCalculator);
        ObjectDataRepository repository = new ObjectDataRepository(factory, writer, indexer, deleter, archiver, reader, searcher, searchIndexer, compositeKeyCalculator);
        return new AuditFixture(
            repository,
            new PublicKeySetAudit(repository, archiver, searchIndexer),
            new PublicKeySetResolver(repository));
    }

    private static PublicKeySetData PlantKeySet(ObjectDataRepository repository, string handle, string? rsaPem, DateTime created)
    {
        PublicKeySetData keySet = new PublicKeySetData
        {
            KeySetHandle = handle,
            PublicRsaKey = rsaPem!,
            Created = created,
            Uuid = Guid.NewGuid().ToString(),
            Cuid = Bam.Cuid.Generate()
        };
        return repository.Create(keySet);
    }

    [UnitTest]
    public void ReportCleanStoreAsHavingNoDuplicates()
    {
        AuditFixture fixture = CreateFixture(nameof(ReportCleanStoreAsHavingNoDuplicates));
        DateTime baseline = DateTime.UtcNow;

        When.A<PublicKeySetAudit>("audits a store with no duplicates",
            () => fixture.Audit,
            (audit) =>
            {
                PlantKeySet(fixture.Repository, "handle-one", "material-one", baseline);
                PlantKeySet(fixture.Repository, "handle-two", "material-two", baseline);

                PublicKeySetDuplicateReport report = audit.FindDuplicates();
                return new CleanStoreOutcome(report.HasDuplicates, report.DuplicateHandleGroups.Count, report.SharedKeyMaterialGroups.Count);
            })
        .TheTest
        .ShouldPass<CleanStoreOutcome>((because, outcome) =>
        {
            because.ItsTrue("a clean store has no duplicates", !outcome.HasDuplicates);
            because.ItsTrue("no duplicate-handle groups", outcome.HandleGroupCount == 0);
            because.ItsTrue("no shared-material groups", outcome.MaterialGroupCount == 0);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ReportDuplicateHandlesAndSharedMaterialWithTheAuthoritativeRowFirst()
    {
        AuditFixture fixture = CreateFixture(nameof(ReportDuplicateHandlesAndSharedMaterialWithTheAuthoritativeRowFirst));
        DateTime baseline = DateTime.UtcNow;

        When.A<PublicKeySetAudit>("audits a store with both duplicate kinds",
            () => fixture.Audit,
            (audit) =>
            {
                // duplicate handle: same handle twice, distinct material
                PublicKeySetData handleLater = PlantKeySet(fixture.Repository, "dup-handle", "material-a", baseline);
                PublicKeySetData handleEarlier = PlantKeySet(fixture.Repository, "dup-handle", "material-b", baseline.AddMinutes(-10));

                // shared material: two handles carrying the same material
                PublicKeySetData materialLater = PlantKeySet(fixture.Repository, "claimant-two", "shared-material", baseline);
                PublicKeySetData materialEarlier = PlantKeySet(fixture.Repository, "claimant-one", "shared-material", baseline.AddMinutes(-10));

                PublicKeySetDuplicateReport report = audit.FindDuplicates();
                PublicKeySetDuplicateGroup? handleGroup = report.DuplicateHandleGroups.FirstOrDefault();
                PublicKeySetDuplicateGroup? materialGroup = report.SharedKeyMaterialGroups.FirstOrDefault();

                return new ReportOutcome(
                    report.HasDuplicates,
                    report.DuplicateHandleGroups.Count,
                    report.SharedKeyMaterialGroups.Count,
                    handleGroup?.Authoritative.Uuid,
                    handleEarlier.Uuid,
                    handleGroup?.NonAuthoritative.Count ?? -1,
                    materialGroup?.Authoritative.KeySetHandle,
                    materialEarlier.KeySetHandle,
                    materialGroup?.GroupKey,
                    "shared-material".Sha256());
            })
        .TheTest
        .ShouldPass<ReportOutcome>((because, outcome) =>
        {
            because.ItsTrue("duplicates were found", outcome.HasDuplicates);
            because.ItsTrue("one duplicate-handle group", outcome.HandleGroupCount == 1);
            because.ItsTrue("one shared-material group", outcome.MaterialGroupCount == 1);
            because.ItsTrue("handle group authoritative row is the earliest-created", outcome.HandleAuthoritativeExpectedUuid.Equals(outcome.HandleAuthoritativeUuid));
            because.ItsTrue("handle group has one non-authoritative row", outcome.HandleNonAuthoritativeCount == 1);
            because.ItsTrue("material group authoritative row is the earliest-created", outcome.MaterialAuthoritativeExpectedHandle.Equals(outcome.MaterialAuthoritativeHandle));
            because.ItsTrue("material group key is the material digest", outcome.ExpectedMaterialGroupKey.Equals(outcome.MaterialGroupKey));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void NotReportSameHandleDuplicatesAsSharedMaterial()
    {
        AuditFixture fixture = CreateFixture(nameof(NotReportSameHandleDuplicatesAsSharedMaterial));
        DateTime baseline = DateTime.UtcNow;

        When.A<PublicKeySetAudit>("audits duplicate rows of ONE handle sharing material",
            () => fixture.Audit,
            (audit) =>
            {
                PlantKeySet(fixture.Repository, "one-handle", "one-material", baseline);
                PlantKeySet(fixture.Repository, "one-handle", "one-material", baseline.AddMinutes(-1));

                PublicKeySetDuplicateReport report = audit.FindDuplicates();
                return new SingleHandleOutcome(report.DuplicateHandleGroups.Count, report.SharedKeyMaterialGroups.Count);
            })
        .TheTest
        .ShouldPass<SingleHandleOutcome>((because, outcome) =>
        {
            because.ItsTrue("the rows form a duplicate-handle group", outcome.HandleGroupCount == 1);
            because.ItsTrue("material confined to one handle is not a shared-material group", outcome.MaterialGroupCount == 0);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ArchiveNonAuthoritativeRowsOnRepairAndLeaveAHealthyStore()
    {
        AuditFixture fixture = CreateFixture(nameof(ArchiveNonAuthoritativeRowsOnRepairAndLeaveAHealthyStore));
        DateTime baseline = DateTime.UtcNow;

        When.A<PublicKeySetAudit>("repairs a store with cross-handle duplicate material",
            () => fixture.Audit,
            (audit) =>
            {
                PublicKeySetData loser = PlantKeySet(fixture.Repository, "late-claimant", "victim-material", baseline);
                PublicKeySetData winner = PlantKeySet(fixture.Repository, "original-owner", "victim-material", baseline.AddHours(-1));
                PublicKeySetData bystander = PlantKeySet(fixture.Repository, "unrelated", "unrelated-material", baseline);

                PublicKeySetDuplicateReport report = audit.FindDuplicates();
                audit.Repair(report);

                List<PublicKeySetData> remaining = fixture.Repository.RetrieveAll<PublicKeySetData>().ToList();
                PublicKeySetData? resolved = fixture.Resolver.ResolveByKeyMaterial("victim-material");
                PublicKeySetDuplicateReport afterRepair = audit.FindDuplicates();

                return new RepairOutcome(
                    remaining.Count,
                    remaining.Any(keySet => keySet.Uuid == winner.Uuid),
                    remaining.Any(keySet => keySet.Uuid == loser.Uuid),
                    remaining.Any(keySet => keySet.Uuid == bystander.Uuid),
                    resolved?.KeySetHandle,
                    afterRepair.HasDuplicates);
            })
        .TheTest
        .ShouldPass<RepairOutcome>((because, outcome) =>
        {
            because.ItsTrue("two rows remain after repair", outcome.RemainingCount == 2);
            because.ItsTrue("the authoritative row survives", outcome.WinnerSurvives);
            because.ItsTrue("the non-authoritative row is gone from live storage", !outcome.LoserSurvives);
            because.ItsTrue("unrelated rows are untouched", outcome.BystanderSurvives);
            because.ItsTrue("material resolves to the survivor", "original-owner".Equals(outcome.ResolvedHandle));
            because.ItsTrue("a re-audit finds a healthy store", !outcome.HasDuplicatesAfterRepair);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed record CleanStoreOutcome(bool HasDuplicates, int HandleGroupCount, int MaterialGroupCount);

    private sealed record ReportOutcome(
        bool HasDuplicates,
        int HandleGroupCount,
        int MaterialGroupCount,
        string? HandleAuthoritativeUuid,
        string HandleAuthoritativeExpectedUuid,
        int HandleNonAuthoritativeCount,
        string? MaterialAuthoritativeHandle,
        string MaterialAuthoritativeExpectedHandle,
        string? MaterialGroupKey,
        string ExpectedMaterialGroupKey);

    private sealed record SingleHandleOutcome(int HandleGroupCount, int MaterialGroupCount);

    private sealed record RepairOutcome(
        int RemainingCount,
        bool WinnerSurvives,
        bool LoserSurvives,
        bool BystanderSurvives,
        string? ResolvedHandle,
        bool HasDuplicatesAfterRepair);

    [UnitTest]
    public void NeverAnnihilateAHandleWhenGroupsOverlap()
    {
        AuditFixture fixture = CreateFixture(nameof(NeverAnnihilateAHandleWhenGroupsOverlap));
        DateTime baseline = DateTime.UtcNow;

        When.A<PublicKeySetAudit>("repairs overlapping material and handle groups",
            () => fixture.Audit,
            (audit) =>
            {
                // The bam.protocol#24 SF4 shape: X1 is authoritative for handle X but loses the
                // material group to Y1; X2 loses the handle group to X1. A naive union archives
                // both X1 and X2, leaving handle X with zero live rows.
                PublicKeySetData y1 = PlantKeySet(fixture.Repository, "handle-y", "shared-material", baseline.AddHours(-3));
                PublicKeySetData x1 = PlantKeySet(fixture.Repository, "handle-x", "shared-material", baseline.AddHours(-2));
                PublicKeySetData x2 = PlantKeySet(fixture.Repository, "handle-x", "x-only-material", baseline.AddHours(-1));

                PublicKeySetDuplicateReport report = audit.FindDuplicates();
                PublicKeySetRepairResult result = audit.Repair(report);

                List<PublicKeySetData> live = fixture.Repository.RetrieveAll<PublicKeySetData>().ToList();
                bool handleXAlive = live.Any(keySet => keySet.KeySetHandle == "handle-x");
                bool x2Survives = live.Any(keySet => keySet.Uuid == x2.Uuid);
                bool x1Archived = live.All(keySet => keySet.Uuid != x1.Uuid);
                bool y1Survives = live.Any(keySet => keySet.Uuid == y1.Uuid);

                return new OverlapOutcome(result.Succeeded, handleXAlive, x2Survives, x1Archived, y1Survives, live.Count);
            })
        .TheTest
        .ShouldPass<OverlapOutcome>((because, outcome) =>
        {
            because.ItsTrue("the repair verifiably succeeded", outcome.Succeeded);
            because.ItsTrue("handle-x still has a live row (no annihilation)", outcome.HandleXAlive);
            because.ItsTrue("x2 became handle-x's survivor after x1 lost the material group", outcome.X2Survives);
            because.ItsTrue("x1 (material-group loser) was archived", outcome.X1Archived);
            because.ItsTrue("y1 (material-group winner) survives", outcome.Y1Survives);
            because.ItsTrue("exactly two rows remain", outcome.LiveCount == 2);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ExcludeRevokedTombstonesFromTheAuditUniverse()
    {
        AuditFixture fixture = CreateFixture(nameof(ExcludeRevokedTombstonesFromTheAuditUniverse));
        DateTime baseline = DateTime.UtcNow;

        When.A<PublicKeySetAudit>("audits a store holding a tombstone plus its re-registered handle",
            () => fixture.Audit,
            (audit) =>
            {
                // The legal post-revocation shape (bam.protocol#11): a revoked tombstone and an
                // active row for the same handle. Not a duplicate; never archived.
                PublicKeySetData tombstone = PlantKeySet(fixture.Repository, "reborn-handle", "old-material", baseline.AddHours(-2));
                tombstone.RevokedUtc = baseline.AddHours(-1);
                fixture.Repository.Update(tombstone);
                PlantKeySet(fixture.Repository, "reborn-handle", "new-material", baseline);

                PublicKeySetDuplicateReport report = audit.FindDuplicates();
                PublicKeySetRepairResult result = audit.Repair(report);

                List<PublicKeySetData> everything = fixture.Repository.RetrieveAll<PublicKeySetData>().ToList();
                bool tombstonePreserved = everything.Any(keySet => keySet.Uuid == tombstone.Uuid && keySet.RevokedUtc != null);

                return new TombstoneOutcome(report.HasDuplicates, result.Succeeded, result.Archived.Count, tombstonePreserved, everything.Count);
            })
        .TheTest
        .ShouldPass<TombstoneOutcome>((because, outcome) =>
        {
            because.ItsTrue("a tombstone plus an active row is not a duplicate", !outcome.HasDuplicates);
            because.ItsTrue("repair succeeded trivially", outcome.Succeeded);
            because.ItsTrue("nothing was archived", outcome.ArchivedCount == 0);
            because.ItsTrue("the tombstone survives (blocklist role preserved)", outcome.TombstonePreserved);
            because.ItsTrue("both rows remain in the store", outcome.RowCount == 2);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed record OverlapOutcome(bool Succeeded, bool HandleXAlive, bool X2Survives, bool X1Archived, bool Y1Survives, int LiveCount);

    private sealed record TombstoneOutcome(bool HasDuplicates, bool Succeeded, int ArchivedCount, bool TombstonePreserved, int RowCount);
}
