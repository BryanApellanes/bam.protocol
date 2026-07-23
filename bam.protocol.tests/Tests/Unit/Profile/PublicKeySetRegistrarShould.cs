using Bam.Data.Dynamic.Objects;
using Bam.Data.Objects;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile;
using Bam.Storage;
using Bam.Test;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("PublicKeySetRegistrar Should", Selector = "pksr")]
public class PublicKeySetRegistrarShould : UnitTestMenuContainer
{
    private static ObjectDataRepository CreateObjectDataRepository(string testName)
    {
        string rootPath = $"./.bam/tests/{testName}";
        // each run starts from a clean store so first-registration-wins tests are repeatable
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
        return new ObjectDataRepository(factory, writer, indexer, deleter, archiver, reader, searcher, searchIndexer, compositeKeyCalculator);
    }

    private static PublicKeySetRegistrar CreateRegistrar(ObjectDataRepository repository)
    {
        return new PublicKeySetRegistrar(repository, new RsaKeySetRotationVerifier(new RsaSignatureProvider()));
    }

    private static byte[] SignRotation(RsaPublicPrivateKeyPair signingKeyPair, PublicKeySetData proposed)
    {
        RsaSignatureProvider signatureProvider = new RsaSignatureProvider();
        ISignature signature = signatureProvider.Sign(signingKeyPair, KeySetRotationPayload.Compose(proposed), RsaKeySetRotationVerifier.Algorithm);
        return signature.SignatureBytes;
    }

    [UnitTest]
    public void RegisterFirstKeySetForHandle()
    {
        RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair();

        When.A<PublicKeySetRegistrar>("registers the first key set for a handle",
            () => CreateRegistrar(CreateObjectDataRepository(nameof(RegisterFirstKeySetForHandle))),
            (registrar) =>
            {
                registrar.Register(new PublicKeySetData
                {
                    KeySetHandle = "first-registrant",
                    PublicRsaKey = keyPair.PublicKeyPem,
                });
                PublicKeySetData resolved = registrar.Resolve("first-registrant");
                return resolved;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<PublicKeySetData>("resolved key set carries the registered RSA key", k => k.PublicRsaKey == keyPair.PublicKeyPem);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectDuplicateRegistrationAndPreserveIdentity()
    {
        RsaPublicPrivateKeyPair victimKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair attackerKeyPair = new RsaPublicPrivateKeyPair();

        When.A<PublicKeySetRegistrar>("rejects a second registration under an already-registered handle",
            () => CreateRegistrar(CreateObjectDataRepository(nameof(RejectDuplicateRegistrationAndPreserveIdentity))),
            (registrar) =>
            {
                registrar.Register(new PublicKeySetData
                {
                    KeySetHandle = "victim",
                    PublicRsaKey = victimKeyPair.PublicKeyPem,
                });

                bool conflictThrown = false;
                try
                {
                    registrar.Register(new PublicKeySetData
                    {
                        KeySetHandle = "victim",
                        PublicRsaKey = attackerKeyPair.PublicKeyPem,
                    });
                }
                catch (PublicKeySetConflictException)
                {
                    conflictThrown = true;
                }

                PublicKeySetData resolved = registrar.Resolve("victim");
                return new DuplicateRegistrationOutcome(conflictThrown, resolved.PublicRsaKey);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<DuplicateRegistrationOutcome>("the duplicate registration threw PublicKeySetConflictException", o => o.ConflictThrown)
                .As<DuplicateRegistrationOutcome>("the handle still resolves to the first-registered key", o => o.ResolvedRsaKey == victimKeyPair.PublicKeyPem);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ResolveEarliestCreatedWhenDuplicatesExist()
    {
        RsaPublicPrivateKeyPair victimKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair attackerKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(ResolveEarliestCreatedWhenDuplicatesExist));

        When.A<PublicKeySetRegistrar>("resolves the earliest-created row when duplicate rows exist",
            () => CreateRegistrar(repository),
            (registrar) =>
            {
                // seed duplicates directly through the store, bypassing the registrar, to
                // simulate legacy data or direct store tampering; the attacker row is
                // inserted FIRST to prove resolution orders by Created, not storage order
                repository.Create(new PublicKeySetData
                {
                    KeySetHandle = "contested",
                    PublicRsaKey = attackerKeyPair.PublicKeyPem,
                    Created = new DateTime(2026, 7, 2, 0, 0, 0, DateTimeKind.Utc),
                });
                repository.Create(new PublicKeySetData
                {
                    KeySetHandle = "contested",
                    PublicRsaKey = victimKeyPair.PublicKeyPem,
                    Created = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                });

                PublicKeySetData resolved = registrar.Resolve("contested");
                return resolved;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<PublicKeySetData>("the earliest-created key set wins regardless of insertion order", k => k.PublicRsaKey == victimKeyPair.PublicKeyPem);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RotateWithValidProofOfPossession()
    {
        RsaPublicPrivateKeyPair currentKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair nextKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RotateWithValidProofOfPossession));

        When.A<PublicKeySetRegistrar>("rotates a key set given a valid signature by the current key",
            () => CreateRegistrar(repository),
            (registrar) =>
            {
                registrar.Register(new PublicKeySetData
                {
                    KeySetHandle = "rotator",
                    PublicRsaKey = currentKeyPair.PublicKeyPem,
                });

                PublicKeySetData proposed = new PublicKeySetData
                {
                    KeySetHandle = "rotator",
                    PublicRsaKey = nextKeyPair.PublicKeyPem,
                };
                byte[] rotationSignature = SignRotation(currentKeyPair, proposed);

                registrar.Rotate(proposed, rotationSignature);

                PublicKeySetData resolved = registrar.Resolve("rotator");
                int rowCount = repository.Query<PublicKeySetData>(p => p.KeySetHandle == "rotator").Count();
                return new RotationSuccessOutcome(resolved.PublicRsaKey, rowCount);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<RotationSuccessOutcome>("the handle resolves to the rotated-to key", o => o.ResolvedRsaKey == nextKeyPair.PublicKeyPem)
                .As<RotationSuccessOutcome>("rotation updated in place leaving exactly one row", o => o.RowCount == 1);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectRotationSignedByDifferentKey()
    {
        RsaPublicPrivateKeyPair currentKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair attackerKeyPair = new RsaPublicPrivateKeyPair();

        When.A<PublicKeySetRegistrar>("rejects a rotation whose signature was not made by the current key",
            () => CreateRegistrar(CreateObjectDataRepository(nameof(RejectRotationSignedByDifferentKey))),
            (registrar) =>
            {
                registrar.Register(new PublicKeySetData
                {
                    KeySetHandle = "target",
                    PublicRsaKey = currentKeyPair.PublicKeyPem,
                });

                PublicKeySetData proposed = new PublicKeySetData
                {
                    KeySetHandle = "target",
                    PublicRsaKey = attackerKeyPair.PublicKeyPem,
                };
                // the attacker signs with their own key, not the currently registered key
                byte[] forgedSignature = SignRotation(attackerKeyPair, proposed);

                bool rotationRejected = false;
                try
                {
                    registrar.Rotate(proposed, forgedSignature);
                }
                catch (InvalidKeySetRotationException)
                {
                    rotationRejected = true;
                }

                PublicKeySetData resolved = registrar.Resolve("target");
                return new RotationRejectionOutcome(rotationRejected, resolved.PublicRsaKey);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<RotationRejectionOutcome>("the forged rotation threw InvalidKeySetRotationException", o => o.Rejected)
                .As<RotationRejectionOutcome>("the handle still resolves to the original key", o => o.ResolvedRsaKey == currentKeyPair.PublicKeyPem);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectRotationWhenNoKeySetExists()
    {
        RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair();

        When.A<PublicKeySetRegistrar>("rejects a rotation for a handle with no registered key set",
            () => CreateRegistrar(CreateObjectDataRepository(nameof(RejectRotationWhenNoKeySetExists))),
            (registrar) =>
            {
                PublicKeySetData proposed = new PublicKeySetData
                {
                    KeySetHandle = "unregistered",
                    PublicRsaKey = keyPair.PublicKeyPem,
                };
                byte[] rotationSignature = SignRotation(keyPair, proposed);

                bool rotationRejected = false;
                try
                {
                    registrar.Rotate(proposed, rotationSignature);
                }
                catch (InvalidKeySetRotationException)
                {
                    rotationRejected = true;
                }
                return new RotationRejectionOutcome(rotationRejected, string.Empty);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<RotationRejectionOutcome>("rotation of an unregistered handle threw InvalidKeySetRotationException", o => o.Rejected);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed record DuplicateRegistrationOutcome(bool ConflictThrown, string ResolvedRsaKey);

    private sealed record RotationSuccessOutcome(string ResolvedRsaKey, int RowCount);

    private sealed record RotationRejectionOutcome(bool Rejected, string ResolvedRsaKey);
}
