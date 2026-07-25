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

    private static byte[] SignRotation(RsaPublicPrivateKeyPair signingKeyPair, string currentPublicRsaKeyPem, PublicKeySetData proposed)
    {
        RsaSignatureProvider signatureProvider = new RsaSignatureProvider();
        string payload = KeySetRotationPayload.Compose(currentPublicRsaKeyPem.Sha256(), proposed);
        ISignature signature = signatureProvider.Sign(signingKeyPair, payload, RsaKeySetRotationVerifier.Algorithm);
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
                PublicKeySetData? resolved = registrar.Resolve("first-registrant");
                return resolved!;
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

                PublicKeySetData? resolved = registrar.Resolve("victim");
                return new DuplicateRegistrationOutcome(conflictThrown, resolved!.PublicRsaKey);
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

                PublicKeySetData? resolved = registrar.Resolve("contested");
                return resolved!;
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
    public void StampCreatedServerSideIgnoringCallerValue()
    {
        RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair();
        DateTime beforeRegister = DateTime.UtcNow.AddSeconds(-1);

        When.A<PublicKeySetRegistrar>("stamps Created server-side ignoring a caller-supplied value",
            () => CreateRegistrar(CreateObjectDataRepository(nameof(StampCreatedServerSideIgnoringCallerValue))),
            (registrar) =>
            {
                // an attacker submits Created = MinValue hoping to win earliest-created-wins;
                // the registrar must stamp the real UtcNow and ignore the supplied value (C3)
                registrar.Register(new PublicKeySetData
                {
                    KeySetHandle = "stamped",
                    PublicRsaKey = keyPair.PublicKeyPem,
                    Created = DateTime.MinValue,
                });
                // capture the upper bound AFTER Register returns but BEFORE resolving: a Created
                // that is stamped at READ time (i.e. persistence regressed, e.g. a stale bam.data
                // where DateTime? is dropped) reads back LATER than this and fails the test, and
                // two fresh resolves would return different read-time values (C3c — the test must
                // be capable of failing, not merely assert >= a lower bound the lazy getter meets).
                DateTime afterRegister = DateTime.UtcNow;
                PublicKeySetData? first = registrar.Resolve("stamped");
                PublicKeySetData? second = registrar.Resolve("stamped");
                return new StampOutcome(first!.Created, second!.Created, beforeRegister, afterRegister);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<StampOutcome>("Created is not null", o => o.First != null)
                .As<StampOutcome>("Created is at or after the register call began", o => o.First!.Value >= o.Before)
                .As<StampOutcome>("Created is at or before Register returned, so it was stamped at register time not read time", o => o.First!.Value <= o.After)
                .As<StampOutcome>("Created is stable across two fresh resolves, so it is persisted not re-stamped per read", o => o.First == o.Second);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void NormalizeUuidAndCuidServerSide()
    {
        RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair();
        string attackerUuid = "00000000-0000-0000-0000-000000000000";
        string attackerCuid = "aaaaaaaaaaaaaaaaaaaaaaaa";

        When.A<PublicKeySetRegistrar>("regenerates Uuid and Cuid server-side so the caller cannot grind the Id tiebreak",
            () => CreateRegistrar(CreateObjectDataRepository(nameof(NormalizeUuidAndCuidServerSide))),
            (registrar) =>
            {
                // Id = CalculateULongKey(Uuid, Cuid); both are publicly settable on RepoData, so an
                // attacker could choose them to win the Created-tie tiebreak. Register must ignore
                // caller-supplied identifiers (C3b).
                registrar.Register(new PublicKeySetData
                {
                    KeySetHandle = "normalized",
                    PublicRsaKey = keyPair.PublicKeyPem,
                    Uuid = attackerUuid,
                    Cuid = attackerCuid,
                });
                PublicKeySetData? resolved = registrar.Resolve("normalized");
                return new IdentifierOutcome(resolved!.Uuid, resolved.Cuid);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<IdentifierOutcome>("the persisted Uuid is server-generated, not the caller's", o => o.Uuid != attackerUuid)
                .As<IdentifierOutcome>("the persisted Cuid is server-generated, not the caller's", o => o.Cuid != attackerCuid);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectRotationToKeyMaterialOfAnotherHandle()
    {
        RsaPublicPrivateKeyPair victimKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair attackerKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RejectRotationToKeyMaterialOfAnotherHandle));

        When.A<PublicKeySetRegistrar>("rejects rotating a handle to another handle's registered key material",
            () => CreateRegistrar(repository),
            (registrar) =>
            {
                registrar.Register(new PublicKeySetData { KeySetHandle = "victim", PublicRsaKey = victimKeyPair.PublicKeyPem });
                registrar.Register(new PublicKeySetData { KeySetHandle = "attacker", PublicRsaKey = attackerKeyPair.PublicKeyPem });

                // The attacker rotates THEIR OWN handle to the victim's public key, signing with
                // the attacker's key (the current registered key for 'attacker') — a legitimately
                // valid possession proof. Without the rotation-path uniqueness guard this plants
                // the victim's key under 'attacker' (C4 / challenger B1).
                PublicKeySetData proposed = new PublicKeySetData { KeySetHandle = "attacker", PublicRsaKey = victimKeyPair.PublicKeyPem };
                byte[] signature = SignRotation(attackerKeyPair, attackerKeyPair.PublicKeyPem, proposed);

                bool rejected = false;
                try
                {
                    registrar.Rotate(proposed, signature);
                }
                catch (PublicKeySetKeyMaterialConflictException)
                {
                    rejected = true;
                }

                PublicKeySetData? attackerRow = registrar.Resolve("attacker");
                int victimKeyRowCount = repository.Query<PublicKeySetData>(p => p.PublicRsaKey == victimKeyPair.PublicKeyPem).Count();
                return new RotationBypassOutcome(rejected, attackerRow!.PublicRsaKey == attackerKeyPair.PublicKeyPem, victimKeyRowCount);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<RotationBypassOutcome>("the cross-handle rotation threw PublicKeySetKeyMaterialConflictException", o => o.Rejected)
                .As<RotationBypassOutcome>("the attacker handle still resolves to its own key", o => o.AttackerRowUnchanged)
                .As<RotationBypassOutcome>("the victim's key material remains under exactly one handle", o => o.VictimKeyRowCount == 1);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectRotationToEmptyRsaKey()
    {
        RsaPublicPrivateKeyPair currentKeyPair = new RsaPublicPrivateKeyPair();

        When.A<PublicKeySetRegistrar>("rejects rotating to an empty RSA key that would brick the handle",
            () => CreateRegistrar(CreateObjectDataRepository(nameof(RejectRotationToEmptyRsaKey))),
            (registrar) =>
            {
                registrar.Register(new PublicKeySetData { KeySetHandle = "rotator", PublicRsaKey = currentKeyPair.PublicKeyPem });

                PublicKeySetData proposed = new PublicKeySetData { KeySetHandle = "rotator", PublicRsaKey = "" };

                bool rejected = false;
                try
                {
                    registrar.Rotate(proposed, new byte[] { 1, 2, 3 });
                }
                catch (InvalidKeySetRotationException)
                {
                    rejected = true;
                }

                PublicKeySetData? resolved = registrar.Resolve("rotator");
                return new RotationRejectionOutcome(rejected, resolved!.PublicRsaKey);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<RotationRejectionOutcome>("rotating to an empty RSA key threw InvalidKeySetRotationException", o => o.Rejected)
                .As<RotationRejectionOutcome>("the handle still resolves to the original key", o => o.ResolvedRsaKey == currentKeyPair.PublicKeyPem);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectRegistrationWithNoKeyMaterial()
    {
        When.A<PublicKeySetRegistrar>("rejects registering a key set with no RSA or ECC material",
            () => CreateRegistrar(CreateObjectDataRepository(nameof(RejectRegistrationWithNoKeyMaterial))),
            (registrar) =>
            {
                bool rejected = false;
                try
                {
                    registrar.Register(new PublicKeySetData { KeySetHandle = "empty" });
                }
                catch (InvalidPublicKeySetException)
                {
                    rejected = true;
                }
                PublicKeySetData? resolved = registrar.Resolve("empty");
                return new RejectionOutcome(rejected, resolved == null);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<RejectionOutcome>("registering with no key material threw InvalidPublicKeySetException", o => o.Rejected)
                .As<RejectionOutcome>("no row was persisted", o => o.StoreUnchanged);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectRsaKeyMaterialAlreadyRegisteredUnderAnotherHandle()
    {
        RsaPublicPrivateKeyPair victimKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RejectRsaKeyMaterialAlreadyRegisteredUnderAnotherHandle));

        When.A<PublicKeySetRegistrar>("rejects registering the same RSA key under a different handle",
            () => CreateRegistrar(repository),
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
                        KeySetHandle = "evil",
                        PublicRsaKey = victimKeyPair.PublicKeyPem,
                    });
                }
                catch (PublicKeySetKeyMaterialConflictException)
                {
                    conflictThrown = true;
                }

                PublicKeySetData? evil = registrar.Resolve("evil");
                return new KeyMaterialConflictOutcome(conflictThrown, evil == null);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<KeyMaterialConflictOutcome>("the reused-RSA registration threw PublicKeySetKeyMaterialConflictException", o => o.ConflictThrown)
                .As<KeyMaterialConflictOutcome>("no row was created for the attacker handle", o => o.NoRowForSecondHandle);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectEccKeyMaterialAlreadyRegisteredUnderAnotherHandle()
    {
        EccPublicPrivateKeyPair victimKeyPair = new EccPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RejectEccKeyMaterialAlreadyRegisteredUnderAnotherHandle));

        When.A<PublicKeySetRegistrar>("rejects registering the same ECC key under a different handle",
            () => CreateRegistrar(repository),
            (registrar) =>
            {
                registrar.Register(new PublicKeySetData
                {
                    KeySetHandle = "victim",
                    PublicEccKey = victimKeyPair.PublicKeyPem,
                });

                bool conflictThrown = false;
                try
                {
                    registrar.Register(new PublicKeySetData
                    {
                        KeySetHandle = "evil",
                        PublicEccKey = victimKeyPair.PublicKeyPem,
                    });
                }
                catch (PublicKeySetKeyMaterialConflictException)
                {
                    conflictThrown = true;
                }

                PublicKeySetData? evil = registrar.Resolve("evil");
                return new KeyMaterialConflictOutcome(conflictThrown, evil == null);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<KeyMaterialConflictOutcome>("the reused-ECC registration threw PublicKeySetKeyMaterialConflictException", o => o.ConflictThrown)
                .As<KeyMaterialConflictOutcome>("no row was created for the attacker handle", o => o.NoRowForSecondHandle);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectUnparseableKeyOnRegister()
    {
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RejectUnparseableKeyOnRegister));

        When.A<PublicKeySetRegistrar>("rejects registering unparseable key material and leaves the store unchanged",
            () => CreateRegistrar(repository),
            (registrar) =>
            {
                bool rejected = false;
                try
                {
                    registrar.Register(new PublicKeySetData
                    {
                        KeySetHandle = "garbage",
                        PublicRsaKey = "not a pem",
                    });
                }
                catch (InvalidPublicKeySetException)
                {
                    rejected = true;
                }

                PublicKeySetData? resolved = registrar.Resolve("garbage");
                return new RejectionOutcome(rejected, resolved == null);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<RejectionOutcome>("registering garbage threw InvalidPublicKeySetException", o => o.Rejected)
                .As<RejectionOutcome>("no row was persisted for the garbage key", o => o.StoreUnchanged);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectUnparseableKeyOnRotate()
    {
        RsaPublicPrivateKeyPair currentKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RejectUnparseableKeyOnRotate));

        When.A<PublicKeySetRegistrar>("rejects rotating to unparseable key material and keeps the current key",
            () => CreateRegistrar(repository),
            (registrar) =>
            {
                registrar.Register(new PublicKeySetData
                {
                    KeySetHandle = "rotator",
                    PublicRsaKey = currentKeyPair.PublicKeyPem,
                });

                PublicKeySetData garbageProposal = new PublicKeySetData
                {
                    KeySetHandle = "rotator",
                    PublicRsaKey = "not a pem",
                };

                bool rejected = false;
                try
                {
                    // signature content is irrelevant — parse validation runs before verification
                    registrar.Rotate(garbageProposal, new byte[] { 1, 2, 3 });
                }
                catch (InvalidKeySetRotationException)
                {
                    rejected = true;
                }

                PublicKeySetData? resolved = registrar.Resolve("rotator");
                return new RotationRejectionOutcome(rejected, resolved!.PublicRsaKey);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<RotationRejectionOutcome>("rotating to garbage threw InvalidKeySetRotationException", o => o.Rejected)
                .As<RotationRejectionOutcome>("the handle still resolves to the original key", o => o.ResolvedRsaKey == currentKeyPair.PublicKeyPem);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectNullOrEmptyHandleAndNullKeySet()
    {
        RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair();

        When.A<PublicKeySetRegistrar>("argument-validates the key set and handle",
            () => CreateRegistrar(CreateObjectDataRepository(nameof(RejectNullOrEmptyHandleAndNullKeySet))),
            (registrar) =>
            {
                bool nullRejected = false;
                try
                {
                    registrar.Register(null!);
                }
                catch (ArgumentNullException)
                {
                    nullRejected = true;
                }

                bool emptyRejected = false;
                try
                {
                    registrar.Register(new PublicKeySetData { KeySetHandle = "", PublicRsaKey = keyPair.PublicKeyPem });
                }
                catch (ArgumentException)
                {
                    emptyRejected = true;
                }

                bool whitespaceRejected = false;
                try
                {
                    registrar.Register(new PublicKeySetData { KeySetHandle = "   ", PublicRsaKey = keyPair.PublicKeyPem });
                }
                catch (ArgumentException)
                {
                    whitespaceRejected = true;
                }

                return new ValidationOutcome(nullRejected, emptyRejected, whitespaceRejected);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<ValidationOutcome>("null key set threw ArgumentNullException", o => o.NullRejected)
                .As<ValidationOutcome>("empty handle threw ArgumentException", o => o.EmptyRejected)
                .As<ValidationOutcome>("whitespace handle threw ArgumentException", o => o.WhitespaceRejected);
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
                byte[] rotationSignature = SignRotation(currentKeyPair, currentKeyPair.PublicKeyPem, proposed);

                registrar.Rotate(proposed, rotationSignature);

                PublicKeySetData? resolved = registrar.Resolve("rotator");
                int rowCount = repository.Query<PublicKeySetData>(p => p.KeySetHandle == "rotator").Count();
                return new RotationSuccessOutcome(resolved!.PublicRsaKey, rowCount);
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
                // the attacker binds the correct current-key SHA but signs with their own key
                byte[] forgedSignature = SignRotation(attackerKeyPair, currentKeyPair.PublicKeyPem, proposed);

                bool rotationRejected = false;
                try
                {
                    registrar.Rotate(proposed, forgedSignature);
                }
                catch (InvalidKeySetRotationException)
                {
                    rotationRejected = true;
                }

                PublicKeySetData? resolved = registrar.Resolve("target");
                return new RotationRejectionOutcome(rotationRejected, resolved!.PublicRsaKey);
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
                byte[] rotationSignature = SignRotation(keyPair, keyPair.PublicKeyPem, proposed);

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

    private sealed record KeyMaterialConflictOutcome(bool ConflictThrown, bool NoRowForSecondHandle);

    private sealed record RejectionOutcome(bool Rejected, bool StoreUnchanged);

    private sealed record StampOutcome(DateTime? First, DateTime? Second, DateTime Before, DateTime After);

    private sealed record IdentifierOutcome(string Uuid, string Cuid);

    private sealed record RotationBypassOutcome(bool Rejected, bool AttackerRowUnchanged, int VictimKeyRowCount);

    private sealed record ValidationOutcome(bool NullRejected, bool EmptyRejected, bool WhitespaceRejected);

    private sealed record RotationSuccessOutcome(string ResolvedRsaKey, int RowCount);

    private sealed record RotationRejectionOutcome(bool Rejected, string ResolvedRsaKey);
}
