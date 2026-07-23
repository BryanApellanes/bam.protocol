using Bam.Data.Dynamic.Objects;
using Bam.Data.Objects;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile;
using Bam.Storage;
using Bam.Test;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("EncryptedProfileRepository Should", Selector = "epr")]
public class EncryptedProfileRepositoryShould : UnitTestMenuContainer
{
    private static IProfileRepository CreateRepository(string testName)
    {
        string rootPath = $"./.bam/tests/{testName}";
        // key-set registration is first-registration-wins; a store left over from a prior
        // run would reject this run's SavePublicKeySet, so each run starts from a clean store
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
        IObjectDataArchiver archiver = new ObjectDataArchiver();
        ObjectDataRepository repo = new ObjectDataRepository(factory, writer, indexer, deleter, archiver, reader, searcher, searchIndexer, compositeKeyCalculator);
        return new EncryptedProfileRepository(repo);
    }

    [UnitTest]
    public void SurfaceConflictOnDuplicateKeySetSave()
    {
        RsaPublicPrivateKeyPair firstKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair secondKeyPair = new RsaPublicPrivateKeyPair();

        When.A<IProfileRepository>("surfaces the registration policy through SavePublicKeySet",
            () => CreateRepository(nameof(SurfaceConflictOnDuplicateKeySetSave)),
            (repository) =>
            {
                repository.SavePublicKeySet(new PublicKeySetData
                {
                    KeySetHandle = "delegated",
                    PublicRsaKey = firstKeyPair.PublicKeyPem,
                });

                bool conflictThrown = false;
                try
                {
                    repository.SavePublicKeySet(new PublicKeySetData
                    {
                        KeySetHandle = "delegated",
                        PublicRsaKey = secondKeyPair.PublicKeyPem,
                    });
                }
                catch (PublicKeySetConflictException)
                {
                    conflictThrown = true;
                }

                PublicKeySetData resolved = repository.FindPublicKeySetByHandle("delegated");
                return new DelegationOutcome(conflictThrown, resolved.PublicRsaKey);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<DelegationOutcome>("the repository surfaced PublicKeySetConflictException", o => o.ConflictThrown)
                .As<DelegationOutcome>("FindPublicKeySetByHandle still returns the first-registered key", o => o.ResolvedRsaKey == firstKeyPair.PublicKeyPem);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed record DelegationOutcome(bool ConflictThrown, string ResolvedRsaKey);
}
