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

[UnitTestMenu("PublicKeySetResolver Should", Selector = "pksres")]
public class PublicKeySetResolverShould : UnitTestMenuContainer
{
    private static ObjectDataRepository CreateObjectDataRepository(string testName)
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
        return new ObjectDataRepository(factory, writer, indexer, deleter, archiver, reader, searcher, searchIndexer, compositeKeyCalculator);
    }

    // Plants a key-set row DIRECTLY in the store, bypassing the registrar — the legacy-data /
    // store-tampering scenario the resolver's determinism exists for.
    private static PublicKeySetData PlantKeySet(ObjectDataRepository repository, string handle, string? rsaPem, string? eccPem, DateTime created)
    {
        PublicKeySetData keySet = new PublicKeySetData
        {
            KeySetHandle = handle,
            PublicRsaKey = rsaPem!,
            PublicEccKey = eccPem!,
            Created = created,
            Uuid = Guid.NewGuid().ToString(),
            Cuid = Bam.Cuid.Generate()
        };
        return repository.Create(keySet);
    }

    [UnitTest]
    public void ResolveByHandleReturningEarliestCreatedRow()
    {
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(ResolveByHandleReturningEarliestCreatedRow));
        DateTime baseline = DateTime.UtcNow;

        When.A<PublicKeySetResolver>("resolves a handle with planted duplicate rows",
            () => new PublicKeySetResolver(repository),
            (resolver) =>
            {
                // insert the LATER row first so insertion order cannot be what wins
                PublicKeySetData later = PlantKeySet(repository, "dup-handle", "rsa-later", null, baseline);
                PublicKeySetData earlier = PlantKeySet(repository, "dup-handle", "rsa-earlier", null, baseline.AddMinutes(-5));

                PublicKeySetData? resolved = resolver.ResolveByHandle("dup-handle");
                PublicKeySetData? missing = resolver.ResolveByHandle("no-such-handle");
                PublicKeySetData? empty = resolver.ResolveByHandle(string.Empty);

                return new ResolveByHandleOutcome(resolved?.Uuid, earlier.Uuid, missing == null, empty == null);
            })
        .TheTest
        .ShouldPass<ResolveByHandleOutcome>((because, outcome) =>
        {
            because.ItsTrue("the earliest-created row won", outcome.ExpectedUuid.Equals(outcome.ResolvedUuid));
            because.ItsTrue("an unregistered handle resolves to null", outcome.MissingIsNull);
            because.ItsTrue("an empty handle resolves to null", outcome.EmptyIsNull);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ResolveByKeyMaterialAcrossRsaAndEccFields()
    {
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(ResolveByKeyMaterialAcrossRsaAndEccFields));
        DateTime baseline = DateTime.UtcNow;

        When.A<PublicKeySetResolver>("resolves key material stored in either key field",
            () => new PublicKeySetResolver(repository),
            (resolver) =>
            {
                PublicKeySetData rsaCarrier = PlantKeySet(repository, "rsa-owner", "material-rsa", null, baseline);
                PublicKeySetData eccCarrier = PlantKeySet(repository, "ecc-owner", null, "material-ecc", baseline);

                PublicKeySetData? byRsa = resolver.ResolveByKeyMaterial("material-rsa");
                PublicKeySetData? byEcc = resolver.ResolveByKeyMaterial("material-ecc");
                PublicKeySetData? unknown = resolver.ResolveByKeyMaterial("material-unknown");

                return new ByMaterialOutcome(
                    byRsa?.KeySetHandle,
                    byEcc?.KeySetHandle,
                    unknown == null);
            })
        .TheTest
        .ShouldPass<ByMaterialOutcome>((because, outcome) =>
        {
            because.ItsTrue("RSA-field material resolves its owner", "rsa-owner".Equals(outcome.RsaHandle));
            because.ItsTrue("ECC-field material resolves its owner", "ecc-owner".Equals(outcome.EccHandle));
            because.ItsTrue("unregistered material resolves to null", outcome.UnknownIsNull);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ResolveCrossHandleDuplicateMaterialToTheEarliestRegistration()
    {
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(ResolveCrossHandleDuplicateMaterialToTheEarliestRegistration));
        DateTime baseline = DateTime.UtcNow;

        When.A<PublicKeySetResolver>("resolves material claimed by two handles",
            () => new PublicKeySetResolver(repository),
            (resolver) =>
            {
                // the attacker/tampered row is inserted FIRST and created LATER
                PlantKeySet(repository, "late-claimant", "shared-material", null, baseline);
                PublicKeySetData original = PlantKeySet(repository, "original-owner", "shared-material", null, baseline.AddHours(-1));

                PublicKeySetData? firstCall = resolver.ResolveByKeyMaterial("shared-material");
                PublicKeySetData? secondCall = resolver.ResolveByKeyMaterial("shared-material");

                return new DuplicateMaterialOutcome(
                    firstCall?.KeySetHandle,
                    secondCall?.KeySetHandle,
                    original.KeySetHandle);
            })
        .TheTest
        .ShouldPass<DuplicateMaterialOutcome>((because, outcome) =>
        {
            because.ItsTrue("the earliest registration wins", outcome.ExpectedHandle.Equals(outcome.FirstHandle));
            because.ItsTrue("resolution is stable across calls", string.Equals(outcome.FirstHandle, outcome.SecondHandle));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void FindKeyMaterialClaimsExemptingTheCandidatesOwnActiveRow()
    {
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(FindKeyMaterialClaimsExemptingTheCandidatesOwnActiveRow));
        DateTime baseline = DateTime.UtcNow;

        When.A<PublicKeySetResolver>("checks candidate material for claims",
            () => new PublicKeySetResolver(repository),
            (resolver) =>
            {
                PlantKeySet(repository, "owner", "owned-material", null, baseline);

                PublicKeySetData sameOwner = new PublicKeySetData { KeySetHandle = "owner", PublicRsaKey = "owned-material" };
                PublicKeySetData otherHandle = new PublicKeySetData { KeySetHandle = "someone-else", PublicRsaKey = "owned-material" };
                PublicKeySetData freshMaterial = new PublicKeySetData { KeySetHandle = "someone-else", PublicRsaKey = "fresh-material" };
                PublicKeySetData eccConflict = new PublicKeySetData { KeySetHandle = "someone-else", PublicEccKey = "owned-material" };

                return new ConflictOutcome(
                    resolver.FindKeyMaterialClaims(sameOwner).FirstOrDefault()?.KeySetHandle,
                    resolver.FindKeyMaterialClaims(otherHandle).FirstOrDefault()?.KeySetHandle,
                    resolver.FindKeyMaterialClaims(freshMaterial).FirstOrDefault()?.KeySetHandle,
                    resolver.FindKeyMaterialClaims(eccConflict).FirstOrDefault()?.KeySetHandle);
            })
        .TheTest
        .ShouldPass<ConflictOutcome>((because, outcome) =>
        {
            because.ItsTrue("re-presenting your own material is not a claim against you", outcome.SameOwnerConflict == null);
            because.ItsTrue("another handle presenting owned material is claimed", "owner".Equals(outcome.OtherHandleConflict));
            because.ItsTrue("fresh material is claimed nowhere", outcome.FreshMaterialConflict == null);
            because.ItsTrue("material claims match across key fields (ECC candidate vs RSA owner)", "owner".Equals(outcome.EccFieldConflict));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void SkipRevokedRowsOnEveryResolvePathButSurfaceThemAsClaims()
    {
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(SkipRevokedRowsOnEveryResolvePathButSurfaceThemAsClaims));
        DateTime baseline = DateTime.UtcNow;

        When.A<PublicKeySetResolver>("resolves against a store holding a revoked tombstone",
            () => new PublicKeySetResolver(repository),
            (resolver) =>
            {
                PublicKeySetData tombstone = PlantKeySet(repository, "revoked-handle", "revoked-material", null, baseline.AddHours(-2));
                tombstone.RevokedUtc = baseline.AddHours(-1);
                tombstone.RevokedBy = "admin-fingerprint";
                repository.Update(tombstone);

                // A revoked key must not resolve — not by handle, not by material
                // (the #13-scoped revoked-skip).
                PublicKeySetData? byHandle = resolver.ResolveByHandle("revoked-handle");
                PublicKeySetData? byMaterial = resolver.ResolveByKeyMaterial("revoked-material");

                // But the blocklist must still see it: another handle presenting the revoked
                // material gets the tombstone as a claim.
                PublicKeySetData attacker = new PublicKeySetData { KeySetHandle = "attacker", PublicRsaKey = "revoked-material" };
                PublicKeySetData? claim = resolver.FindKeyMaterialClaims(attacker).FirstOrDefault();

                return new RevokedSkipOutcome(byHandle == null, byMaterial == null, claim?.KeySetHandle, claim?.RevokedUtc != null);
            })
        .TheTest
        .ShouldPass<RevokedSkipOutcome>((because, outcome) =>
        {
            because.ItsTrue("a revoked handle does not resolve", outcome.ByHandleIsNull);
            because.ItsTrue("revoked material does not resolve", outcome.ByMaterialIsNull);
            because.ItsTrue("the tombstone still surfaces as a material claim", "revoked-handle".Equals(outcome.ClaimHandle));
            because.ItsTrue("the claim is marked revoked for blocklist policy", outcome.ClaimIsRevoked);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed record ResolveByHandleOutcome(string? ResolvedUuid, string ExpectedUuid, bool MissingIsNull, bool EmptyIsNull);

    private sealed record ByMaterialOutcome(string? RsaHandle, string? EccHandle, bool UnknownIsNull);

    private sealed record DuplicateMaterialOutcome(string? FirstHandle, string? SecondHandle, string ExpectedHandle);

    private sealed record ConflictOutcome(string? SameOwnerConflict, string? OtherHandleConflict, string? FreshMaterialConflict, string? EccFieldConflict);

    private sealed record RevokedSkipOutcome(bool ByHandleIsNull, bool ByMaterialIsNull, string? ClaimHandle, bool ClaimIsRevoked);

    [UnitTest]
    public void DeriveIdentityFromMaterialNotFromTheStampedFingerprint()
    {
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(DeriveIdentityFromMaterialNotFromTheStampedFingerprint));
        DateTime baseline = DateTime.UtcNow;

        When.A<PublicKeySetResolver>("resolves against a row whose stamped fingerprint is wrong",
            () => new PublicKeySetResolver(repository),
            (resolver) =>
            {
                // A drifted/tampered stamp (bam.protocol#24 review round 2, SF6): the row
                // carries victim-material but its fingerprint column claims something else.
                PublicKeySetData driftedRow = new PublicKeySetData
                {
                    KeySetHandle = "drifted",
                    PublicRsaKey = "victim-material",
                    PublicRsaKeyFingerprint = "forged-or-stale-fingerprint",
                    Created = baseline,
                    Uuid = Guid.NewGuid().ToString(),
                    Cuid = Bam.Cuid.Generate()
                };
                repository.Create(driftedRow);

                // Identity must derive from the MATERIAL: byte-identical material still claims
                // the row despite the wrong stamp...
                PublicKeySetData sameMaterialCandidate = new PublicKeySetData { KeySetHandle = "claimant", PublicRsaKey = "victim-material" };
                PublicKeySetData? materialClaim = resolver.FindKeyMaterialClaims(sameMaterialCandidate).FirstOrDefault();

                // ...and material matching the FORGED identity must not resolve or claim the row.
                PublicKeySetData forgedIdentityCandidate = new PublicKeySetData { KeySetHandle = "claimant", PublicRsaKey = "forged-or-stale-fingerprint" };
                PublicKeySetData? forgedClaim = resolver.FindKeyMaterialClaims(forgedIdentityCandidate).FirstOrDefault();
                PublicKeySetData? forgedResolve = resolver.ResolveByKeyMaterial("forged-or-stale-fingerprint");

                return new StampDriftOutcome(materialClaim?.KeySetHandle, forgedClaim == null, forgedResolve == null);
            })
        .TheTest
        .ShouldPass<StampDriftOutcome>((because, outcome) =>
        {
            because.ItsTrue("byte-identical material claims the row despite the wrong stamp", "drifted".Equals(outcome.MaterialClaimHandle));
            because.ItsTrue("the forged identity claims nothing", outcome.ForgedClaimIsNull);
            because.ItsTrue("the forged identity resolves nothing", outcome.ForgedResolveIsNull);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed record StampDriftOutcome(string? MaterialClaimHandle, bool ForgedClaimIsNull, bool ForgedResolveIsNull);
}
