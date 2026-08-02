using Bam.Data.Dynamic.Objects;
using Bam.Data.Objects;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile;
using Bam.Storage;
using Bam.Test;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("KeySetRevocation Should", Selector = "ksrev")]
public class KeySetRevocationShould : UnitTestMenuContainer
{
    private static ObjectDataRepository CreateObjectDataRepository(string testName)
    {
        string rootPath = $"./.bam/tests/{testName}";
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

    private static PublicKeySetRegistrar CreateRegistrar(ObjectDataRepository repository)
    {
        return new PublicKeySetRegistrar(repository, new RsaKeySetRotationVerifier(new RsaSignatureProvider()));
    }

    private static KeySetRevocation CreateRevocation(ObjectDataRepository repository, RsaPublicPrivateKeyPair adminKeyPair)
    {
        StaticAdminPublicKeySource adminSource = new StaticAdminPublicKeySource(adminKeyPair.PublicKeyPem);
        return new KeySetRevocation(repository, new RsaRevocationAuthority(new RsaSignatureProvider(), adminSource));
    }

    private static byte[] SignRevocation(RsaPublicPrivateKeyPair adminKeyPair, PublicKeySetData target)
    {
        return SignRevocation(adminKeyPair, target, null);
    }

    private static byte[] SignRevocation(RsaPublicPrivateKeyPair adminKeyPair, PublicKeySetData target, string? successorFingerprint)
    {
        RsaSignatureProvider signatureProvider = new RsaSignatureProvider();
        ISignature signature = signatureProvider.Sign(adminKeyPair, RevocationPayload.Compose(target, successorFingerprint), RsaRevocationAuthority.Algorithm);
        return signature.SignatureBytes;
    }

    private static byte[] SignRotation(RsaPublicPrivateKeyPair signingKeyPair, string currentPublicRsaKeyPem, PublicKeySetData proposed)
    {
        RsaSignatureProvider signatureProvider = new RsaSignatureProvider();
        string payload = KeySetRotationPayload.Compose(currentPublicRsaKeyPem.Sha256(), proposed);
        ISignature signature = signatureProvider.Sign(signingKeyPair, payload, RsaKeySetRotationVerifier.Algorithm);
        return signature.SignatureBytes;
    }

    [UnitTest]
    public void RevokeTombstonesAndDeauthorizesTheKeySet()
    {
        RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RevokeTombstonesAndDeauthorizesTheKeySet));

        When.A<KeySetRevocation>("tombstones the key set and de-authorizes it on revocation",
            () => CreateRevocation(repository, adminKeyPair),
            (revocation) =>
            {
                PublicKeySetRegistrar registrar = CreateRegistrar(repository);
                registrar.Register(new PublicKeySetData { KeySetHandle = "holder", PublicRsaKey = keyPair.PublicKeyPem });

                PublicKeySetData active = registrar.Resolve("holder")!;
                byte[] proof = SignRevocation(adminKeyPair, active);
                PublicKeySetData tombstoned = revocation.Revoke("holder", proof, null);

                PublicKeySetData? afterRevoke = registrar.Resolve("holder");
                return new RevokeOutcome(tombstoned.RevokedUtc != null, tombstoned.RevokedBy != null, afterRevoke == null);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<RevokeOutcome>("the row is tombstoned (RevokedUtc set)", o => o.RevokedUtcSet)
                .As<RevokeOutcome>("the authorizing admin key is recorded", o => o.RevokedBySet)
                .As<RevokeOutcome>("the revoked key set is no longer authoritative (Resolve returns null)", o => o.ResolvesNullAfter);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void FreeTheHandleForReRegistrationWithFreshMaterial()
    {
        RsaPublicPrivateKeyPair originalKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair freshKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(FreeTheHandleForReRegistrationWithFreshMaterial));

        When.A<KeySetRevocation>("frees the handle so it can be re-registered with fresh material",
            () => CreateRevocation(repository, adminKeyPair),
            (revocation) =>
            {
                PublicKeySetRegistrar registrar = CreateRegistrar(repository);
                registrar.Register(new PublicKeySetData { KeySetHandle = "recovered", PublicRsaKey = originalKeyPair.PublicKeyPem });
                PublicKeySetData active = registrar.Resolve("recovered")!;
                revocation.Revoke("recovered", SignRevocation(adminKeyPair, active), null);

                registrar.Register(new PublicKeySetData { KeySetHandle = "recovered", PublicRsaKey = freshKeyPair.PublicKeyPem });
                PublicKeySetData? resolved = registrar.Resolve("recovered");
                return resolved!;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<PublicKeySetData>("the handle now resolves to the fresh re-registered key", k => k.PublicRsaKey == freshKeyPair.PublicKeyPem);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void BlocklistRevokedKeyMaterialFromReRegistration()
    {
        RsaPublicPrivateKeyPair victimKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(BlocklistRevokedKeyMaterialFromReRegistration));

        When.A<KeySetRevocation>("keeps revoked key material blocklisted from re-registration under any handle",
            () => CreateRevocation(repository, adminKeyPair),
            (revocation) =>
            {
                PublicKeySetRegistrar registrar = CreateRegistrar(repository);
                registrar.Register(new PublicKeySetData { KeySetHandle = "victim", PublicRsaKey = victimKeyPair.PublicKeyPem });
                PublicKeySetData active = registrar.Resolve("victim")!;
                revocation.Revoke("victim", SignRevocation(adminKeyPair, active), null);

                bool blockedUnderNewHandle = false;
                try
                {
                    registrar.Register(new PublicKeySetData { KeySetHandle = "evil", PublicRsaKey = victimKeyPair.PublicKeyPem });
                }
                catch (RevokedKeyMaterialException)
                {
                    blockedUnderNewHandle = true;
                }

                bool blockedUnderSameHandle = false;
                try
                {
                    registrar.Register(new PublicKeySetData { KeySetHandle = "victim", PublicRsaKey = victimKeyPair.PublicKeyPem });
                }
                catch (RevokedKeyMaterialException)
                {
                    blockedUnderSameHandle = true;
                }

                return new BlocklistOutcome(blockedUnderNewHandle, blockedUnderSameHandle);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<BlocklistOutcome>("re-registering the revoked material under a new handle is blocked", o => o.BlockedUnderNewHandle)
                .As<BlocklistOutcome>("re-registering the revoked material under the original handle is blocked", o => o.BlockedUnderSameHandle);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectRevocationWithForgedProof()
    {
        RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair attackerKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RejectRevocationWithForgedProof));

        When.A<KeySetRevocation>("rejects a revocation whose proof was not signed by the admin key",
            () => CreateRevocation(repository, adminKeyPair),
            (revocation) =>
            {
                PublicKeySetRegistrar registrar = CreateRegistrar(repository);
                registrar.Register(new PublicKeySetData { KeySetHandle = "holder", PublicRsaKey = keyPair.PublicKeyPem });
                PublicKeySetData active = registrar.Resolve("holder")!;

                // the attacker signs the correct target payload but with their own key
                byte[] forgedProof = SignRevocation(attackerKeyPair, active);

                bool rejected = false;
                try
                {
                    revocation.Revoke("holder", forgedProof, null);
                }
                catch (UnauthorizedRevocationException)
                {
                    rejected = true;
                }

                PublicKeySetData? afterAttempt = registrar.Resolve("holder");
                return new RejectRevokeOutcome(rejected, afterAttempt != null && afterAttempt.PublicRsaKey == keyPair.PublicKeyPem);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<RejectRevokeOutcome>("the forged revocation threw UnauthorizedRevocationException", o => o.Rejected)
                .As<RejectRevokeOutcome>("the key set is unchanged and still active", o => o.KeySetUnchanged);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectRevocationWhenNoActiveKeySet()
    {
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RejectRevocationWhenNoActiveKeySet));

        When.A<KeySetRevocation>("rejects revoking a handle with no active key set",
            () => CreateRevocation(repository, adminKeyPair),
            (revocation) =>
            {
                bool rejected = false;
                try
                {
                    revocation.Revoke("unregistered", new byte[] { 1, 2, 3 }, null);
                }
                catch (KeySetRevocationException)
                {
                    rejected = true;
                }
                return rejected;
            })
        .TheTest
        .ShouldPass<bool>((because, _, rejected) =>
        {
            because.ItsTrue("revoking an unregistered handle threw KeySetRevocationException", rejected);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectReplayOfRevocationProofAgainstReRegistration()
    {
        RsaPublicPrivateKeyPair originalKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair reRegisteredKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RejectReplayOfRevocationProofAgainstReRegistration));

        When.A<KeySetRevocation>("rejects replaying a captured revocation proof against a re-registration",
            () => CreateRevocation(repository, adminKeyPair),
            (revocation) =>
            {
                PublicKeySetRegistrar registrar = CreateRegistrar(repository);
                registrar.Register(new PublicKeySetData { KeySetHandle = "target", PublicRsaKey = originalKeyPair.PublicKeyPem });
                PublicKeySetData original = registrar.Resolve("target")!;

                // capture a valid proof for the original registration, then legitimately revoke it
                byte[] capturedProof = SignRevocation(adminKeyPair, original);
                revocation.Revoke("target", capturedProof, null);

                // the owner recovers the handle with a fresh key
                registrar.Register(new PublicKeySetData { KeySetHandle = "target", PublicRsaKey = reRegisteredKeyPair.PublicKeyPem });

                // replaying the captured proof must not revoke the re-registration (target-bound to the old Uuid/key)
                bool replayRejected = false;
                try
                {
                    revocation.Revoke("target", capturedProof, null);
                }
                catch (UnauthorizedRevocationException)
                {
                    replayRejected = true;
                }

                PublicKeySetData? afterReplay = registrar.Resolve("target");
                return new ReplayOutcome(replayRejected, afterReplay != null && afterReplay.PublicRsaKey == reRegisteredKeyPair.PublicKeyPem);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<ReplayOutcome>("the replayed proof threw UnauthorizedRevocationException", o => o.ReplayRejected)
                .As<ReplayOutcome>("the re-registered key set remains active", o => o.ReRegistrationStillActive);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectRotationBackToOwnRevokedMaterial()
    {
        RsaPublicPrivateKeyPair compromisedKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair freshKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RejectRotationBackToOwnRevokedMaterial));

        When.A<KeySetRevocation>("blocks rotating a handle back to its own revoked key material",
            () => CreateRevocation(repository, adminKeyPair),
            (revocation) =>
            {
                PublicKeySetRegistrar registrar = CreateRegistrar(repository);
                registrar.Register(new PublicKeySetData { KeySetHandle = "holder", PublicRsaKey = compromisedKeyPair.PublicKeyPem });
                PublicKeySetData active = registrar.Resolve("holder")!;
                revocation.Revoke("holder", SignRevocation(adminKeyPair, active), null);

                registrar.Register(new PublicKeySetData { KeySetHandle = "holder", PublicRsaKey = freshKeyPair.PublicKeyPem });

                PublicKeySetData proposed = new PublicKeySetData { KeySetHandle = "holder", PublicRsaKey = compromisedKeyPair.PublicKeyPem };
                byte[] rotationSignature = SignRotation(freshKeyPair, freshKeyPair.PublicKeyPem, proposed);

                bool blocked = false;
                try
                {
                    registrar.Rotate(proposed, rotationSignature);
                }
                catch (RevokedKeyMaterialException)
                {
                    blocked = true;
                }

                PublicKeySetData? resolved = registrar.Resolve("holder");
                return new RotationBlocklistOutcome(blocked, resolved != null && resolved.PublicRsaKey == compromisedKeyPair.PublicKeyPem);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<RotationBlocklistOutcome>("rotating back to the handle's own revoked material is blocked", o => o.Blocked)
                .As<RotationBlocklistOutcome>("the revoked material did not become authoritative again", o => !o.RevokedMaterialActiveAgain);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectRotationToAnotherHandlesRevokedMaterial()
    {
        RsaPublicPrivateKeyPair victimKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair attackerKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RejectRotationToAnotherHandlesRevokedMaterial));

        When.A<KeySetRevocation>("blocks rotating a handle to another handle's revoked key material",
            () => CreateRevocation(repository, adminKeyPair),
            (revocation) =>
            {
                PublicKeySetRegistrar registrar = CreateRegistrar(repository);
                registrar.Register(new PublicKeySetData { KeySetHandle = "victim", PublicRsaKey = victimKeyPair.PublicKeyPem });
                PublicKeySetData victimActive = registrar.Resolve("victim")!;
                revocation.Revoke("victim", SignRevocation(adminKeyPair, victimActive), null);

                registrar.Register(new PublicKeySetData { KeySetHandle = "attacker", PublicRsaKey = attackerKeyPair.PublicKeyPem });

                PublicKeySetData proposed = new PublicKeySetData { KeySetHandle = "attacker", PublicRsaKey = victimKeyPair.PublicKeyPem };
                byte[] rotationSignature = SignRotation(attackerKeyPair, attackerKeyPair.PublicKeyPem, proposed);

                bool blocked = false;
                try
                {
                    registrar.Rotate(proposed, rotationSignature);
                }
                catch (RevokedKeyMaterialException)
                {
                    blocked = true;
                }
                return blocked;
            })
        .TheTest
        .ShouldPass<bool>((because, _, blocked) =>
        {
            because.ItsTrue("rotating to another handle's revoked material is blocked", blocked);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectReRegisteringWhitespaceVariantOfRevokedMaterial()
    {
        RsaPublicPrivateKeyPair victimKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RejectReRegisteringWhitespaceVariantOfRevokedMaterial));

        When.A<KeySetRevocation>("blocklists a whitespace-re-encoded variant of revoked key material",
            () => CreateRevocation(repository, adminKeyPair),
            (revocation) =>
            {
                PublicKeySetRegistrar registrar = CreateRegistrar(repository);
                registrar.Register(new PublicKeySetData { KeySetHandle = "victim", PublicRsaKey = victimKeyPair.PublicKeyPem });
                PublicKeySetData active = registrar.Resolve("victim")!;
                revocation.Revoke("victim", SignRevocation(adminKeyPair, active), null);

                // the same key, re-encoded with an appended newline: string- and SHA-unequal, but
                // parses to the identical key — must still be caught by the canonical comparison
                string reEncoded = victimKeyPair.PublicKeyPem + "\n";
                bool blocked = false;
                try
                {
                    registrar.Register(new PublicKeySetData { KeySetHandle = "evil", PublicRsaKey = reEncoded });
                }
                catch (RevokedKeyMaterialException)
                {
                    blocked = true;
                }
                return blocked;
            })
        .TheTest
        .ShouldPass<bool>((because, _, blocked) =>
        {
            because.ItsTrue("a re-encoded variant of revoked material is blocklisted", blocked);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectRevocationOfAnAlreadyRevokedHandle()
    {
        RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RejectRevocationOfAnAlreadyRevokedHandle));

        When.A<KeySetRevocation>("rejects revoking a handle whose key set is already revoked",
            () => CreateRevocation(repository, adminKeyPair),
            (revocation) =>
            {
                PublicKeySetRegistrar registrar = CreateRegistrar(repository);
                registrar.Register(new PublicKeySetData { KeySetHandle = "holder", PublicRsaKey = keyPair.PublicKeyPem });
                PublicKeySetData active = registrar.Resolve("holder")!;
                byte[] proof = SignRevocation(adminKeyPair, active);
                revocation.Revoke("holder", proof, null);

                bool secondRejected = false;
                try
                {
                    revocation.Revoke("holder", proof, null);
                }
                catch (KeySetRevocationException)
                {
                    secondRejected = true;
                }
                return secondRejected;
            })
        .TheTest
        .ShouldPass<bool>((because, _, secondRejected) =>
        {
            because.ItsTrue("revoking an already-revoked handle threw KeySetRevocationException (no active key set)", secondRejected);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void FreeTheRevokeToReRegisterWindowToAnyCaller()
    {
        // Regression guard for the UNBOUND revocation path (bam.protocol#21): when a revocation
        // binds no successor, behavior is unchanged from #11 — the framework does not authenticate
        // callers, so a freed handle is re-registrable by whoever calls first (first-registration-
        // wins), and hijack resistance during the window is the consumer's authz responsibility
        // (bamsvc#6, socialkeyinfrastructure.io#9). The BOUND path that closes the hijack window is
        // covered by RejectReRegistrationOfABoundHandleByANonSuccessorKey.
        RsaPublicPrivateKeyPair originalKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair successorKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(FreeTheRevokeToReRegisterWindowToAnyCaller));

        When.A<KeySetRevocation>("frees the handle to any caller during the revoke-to-re-register window (consumer-gated)",
            () => CreateRevocation(repository, adminKeyPair),
            (revocation) =>
            {
                PublicKeySetRegistrar registrar = CreateRegistrar(repository);
                registrar.Register(new PublicKeySetData { KeySetHandle = "handle", PublicRsaKey = originalKeyPair.PublicKeyPem });
                PublicKeySetData active = registrar.Resolve("handle")!;
                revocation.Revoke("handle", SignRevocation(adminKeyPair, active), null);

                // any caller with fresh (non-blocklisted) material can claim the freed handle
                registrar.Register(new PublicKeySetData { KeySetHandle = "handle", PublicRsaKey = successorKeyPair.PublicKeyPem });
                PublicKeySetData? resolved = registrar.Resolve("handle");
                return resolved!;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<PublicKeySetData>("the freed handle is re-registrable with fresh successor material", k => k.PublicRsaKey == successorKeyPair.PublicKeyPem);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void PersistTheAuthorizedSuccessorFingerprintOnTheTombstone()
    {
        RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair successorKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(PersistTheAuthorizedSuccessorFingerprintOnTheTombstone));

        When.A<KeySetRevocation>("records the admin-authorized successor fingerprint on the tombstone",
            () => CreateRevocation(repository, adminKeyPair),
            (revocation) =>
            {
                PublicKeySetRegistrar registrar = CreateRegistrar(repository);
                registrar.Register(new PublicKeySetData { KeySetHandle = "holder", PublicRsaKey = keyPair.PublicKeyPem });
                PublicKeySetData active = registrar.Resolve("holder")!;

                string successorFingerprint = PublicKeyFingerprint.Of(successorKeyPair.PublicKeyPem)!;
                byte[] proof = SignRevocation(adminKeyPair, active, successorFingerprint);
                PublicKeySetData tombstoned = revocation.Revoke("holder", proof, successorFingerprint);

                return new SuccessorPersistedOutcome(
                    tombstoned.RevokedUtc != null,
                    tombstoned.AuthorizedSuccessorFingerprint == successorFingerprint);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<SuccessorPersistedOutcome>("the row is tombstoned", o => o.RevokedUtcSet)
                .As<SuccessorPersistedOutcome>("the authorized successor fingerprint is persisted", o => o.SuccessorFingerprintPersisted);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void AllowTheBoundSuccessorToReRegisterTheFreedHandle()
    {
        RsaPublicPrivateKeyPair originalKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair successorKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(AllowTheBoundSuccessorToReRegisterTheFreedHandle));

        When.A<KeySetRevocation>("lets the admin-authorized successor re-register the freed handle",
            () => CreateRevocation(repository, adminKeyPair),
            (revocation) =>
            {
                PublicKeySetRegistrar registrar = CreateRegistrar(repository);
                registrar.Register(new PublicKeySetData { KeySetHandle = "handle", PublicRsaKey = originalKeyPair.PublicKeyPem });
                PublicKeySetData active = registrar.Resolve("handle")!;

                string successorFingerprint = PublicKeyFingerprint.Of(successorKeyPair.PublicKeyPem)!;
                revocation.Revoke("handle", SignRevocation(adminKeyPair, active, successorFingerprint), successorFingerprint);

                registrar.Register(new PublicKeySetData { KeySetHandle = "handle", PublicRsaKey = successorKeyPair.PublicKeyPem });
                PublicKeySetData? resolved = registrar.Resolve("handle");
                return resolved!;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<PublicKeySetData>("the handle resolves to the bound successor's key", k => k.PublicRsaKey == successorKeyPair.PublicKeyPem);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectReRegistrationOfABoundHandleByANonSuccessorKey()
    {
        // The bam.protocol#11 hijack-window test: a revocation that binds a successor closes the
        // revoke->re-register window — a non-successor first caller is rejected with
        // UnauthorizedSuccessorException, and the bound successor can still claim the handle after.
        RsaPublicPrivateKeyPair originalKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair successorKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair attackerKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RejectReRegistrationOfABoundHandleByANonSuccessorKey));

        When.A<KeySetRevocation>("rejects a non-successor re-registration of a handle bound to a successor",
            () => CreateRevocation(repository, adminKeyPair),
            (revocation) =>
            {
                PublicKeySetRegistrar registrar = CreateRegistrar(repository);
                registrar.Register(new PublicKeySetData { KeySetHandle = "handle", PublicRsaKey = originalKeyPair.PublicKeyPem });
                PublicKeySetData active = registrar.Resolve("handle")!;

                string successorFingerprint = PublicKeyFingerprint.Of(successorKeyPair.PublicKeyPem)!;
                revocation.Revoke("handle", SignRevocation(adminKeyPair, active, successorFingerprint), successorFingerprint);

                // an attacker with a fresh (non-blocklisted) key tries to seize the freed handle
                bool hijackRejected = false;
                try
                {
                    registrar.Register(new PublicKeySetData { KeySetHandle = "handle", PublicRsaKey = attackerKeyPair.PublicKeyPem });
                }
                catch (UnauthorizedSuccessorException)
                {
                    hijackRejected = true;
                }
                bool noActiveAfterHijack = registrar.Resolve("handle") == null;

                // the legitimate successor can still claim the handle
                registrar.Register(new PublicKeySetData { KeySetHandle = "handle", PublicRsaKey = successorKeyPair.PublicKeyPem });
                PublicKeySetData? afterSuccessor = registrar.Resolve("handle");

                return new HijackOutcome(
                    hijackRejected,
                    noActiveAfterHijack,
                    afterSuccessor != null && afterSuccessor.PublicRsaKey == successorKeyPair.PublicKeyPem);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<HijackOutcome>("the non-successor re-registration threw UnauthorizedSuccessorException", o => o.HijackRejected)
                .As<HijackOutcome>("the rejected hijack left the handle unregistered", o => o.NoActiveAfterHijack)
                .As<HijackOutcome>("the bound successor could still claim the handle", o => o.SuccessorClaimed);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void LetTheLatestTombstoneGovernAndConsumeTheBindingOnReRegistration()
    {
        // Multiple tombstones can accrue for a handle across revoke->re-register cycles. The LATEST
        // revocation governs the successor gate, and once the successor claims the handle the new
        // active row makes further registration a conflict — the binding is consumed structurally
        // (bam.protocol#21).
        RsaPublicPrivateKeyPair originalKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair successorAKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair successorBKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair laterKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(LetTheLatestTombstoneGovernAndConsumeTheBindingOnReRegistration));

        When.A<KeySetRevocation>("lets the latest tombstone govern and consumes the binding once re-registered",
            () => CreateRevocation(repository, adminKeyPair),
            (revocation) =>
            {
                PublicKeySetRegistrar registrar = CreateRegistrar(repository);

                // cycle 1: original -> revoke bound to successorA -> successorA claims
                registrar.Register(new PublicKeySetData { KeySetHandle = "handle", PublicRsaKey = originalKeyPair.PublicKeyPem });
                PublicKeySetData firstActive = registrar.Resolve("handle")!;
                string fpA = PublicKeyFingerprint.Of(successorAKeyPair.PublicKeyPem)!;
                revocation.Revoke("handle", SignRevocation(adminKeyPair, firstActive, fpA), fpA);
                registrar.Register(new PublicKeySetData { KeySetHandle = "handle", PublicRsaKey = successorAKeyPair.PublicKeyPem });

                // cycle 2: revoke successorA's row bound to successorB (a SECOND, later tombstone)
                PublicKeySetData secondActive = registrar.Resolve("handle")!;
                string fpB = PublicKeyFingerprint.Of(successorBKeyPair.PublicKeyPem)!;
                revocation.Revoke("handle", SignRevocation(adminKeyPair, secondActive, fpB), fpB);

                // the latest tombstone (bound to successorB) governs: successorA is no longer the
                // authorized successor and is rejected
                bool staleSuccessorRejected = false;
                try
                {
                    registrar.Register(new PublicKeySetData { KeySetHandle = "handle", PublicRsaKey = laterKeyPair.PublicKeyPem });
                }
                catch (UnauthorizedSuccessorException)
                {
                    staleSuccessorRejected = true;
                }

                // successorB (the latest binding) can claim the handle
                registrar.Register(new PublicKeySetData { KeySetHandle = "handle", PublicRsaKey = successorBKeyPair.PublicKeyPem });
                PublicKeySetData? resolved = registrar.Resolve("handle");

                // the binding is now consumed: an active row blocks any further registration
                bool furtherRegistrationBlocked = false;
                try
                {
                    registrar.Register(new PublicKeySetData { KeySetHandle = "handle", PublicRsaKey = laterKeyPair.PublicKeyPem });
                }
                catch (PublicKeySetConflictException)
                {
                    furtherRegistrationBlocked = true;
                }

                return new GoverningTombstoneOutcome(
                    staleSuccessorRejected,
                    resolved != null && resolved.PublicRsaKey == successorBKeyPair.PublicKeyPem,
                    furtherRegistrationBlocked);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<GoverningTombstoneOutcome>("a key that is not the latest bound successor is rejected", o => o.StaleSuccessorRejected)
                .As<GoverningTombstoneOutcome>("the latest bound successor claims the handle", o => o.LatestSuccessorClaimed)
                .As<GoverningTombstoneOutcome>("once claimed, the active row blocks further registration (binding consumed)", o => o.FurtherRegistrationBlocked);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectRevocationWhenTheSuccessorFingerprintWasNotTheOneSigned()
    {
        // The PR's core property: the successor is covered by the admin signature, so a caller cannot
        // substitute a different successor than the admin signed (bam.protocol#25 review SF2). Verified
        // end to end through Revoke, not just at the Compose string level.
        RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair successorA = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair successorB = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RejectRevocationWhenTheSuccessorFingerprintWasNotTheOneSigned));

        When.A<KeySetRevocation>("rejects a revocation whose successor parameter differs from the one the admin signed",
            () => CreateRevocation(repository, adminKeyPair),
            (revocation) =>
            {
                PublicKeySetRegistrar registrar = CreateRegistrar(repository);
                registrar.Register(new PublicKeySetData { KeySetHandle = "holder", PublicRsaKey = keyPair.PublicKeyPem });
                PublicKeySetData active = registrar.Resolve("holder")!;

                string fpA = PublicKeyFingerprint.Of(successorA.PublicKeyPem)!;
                string fpB = PublicKeyFingerprint.Of(successorB.PublicKeyPem)!;

                // proof signed binding successor A, but the call names successor B
                byte[] proofBoundToA = SignRevocation(adminKeyPair, active, fpA);
                bool mismatchRejected = false;
                try
                {
                    revocation.Revoke("holder", proofBoundToA, fpB);
                }
                catch (UnauthorizedRevocationException)
                {
                    mismatchRejected = true;
                }

                // proof signed UNBOUND, but the call names a successor
                byte[] proofUnbound = SignRevocation(adminKeyPair, active, null);
                bool unboundVsBoundRejected = false;
                try
                {
                    revocation.Revoke("holder", proofUnbound, fpA);
                }
                catch (UnauthorizedRevocationException)
                {
                    unboundVsBoundRejected = true;
                }

                bool stillActive = registrar.Resolve("holder") != null;
                return new SignatureMismatchOutcome(mismatchRejected, unboundVsBoundRejected, stillActive);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<SignatureMismatchOutcome>("a successor other than the one signed is rejected", o => o.MismatchedRejected)
                .As<SignatureMismatchOutcome>("a bound call against an unbound-signed proof is rejected", o => o.UnboundVsBoundRejected)
                .As<SignatureMismatchOutcome>("the key set stays active after the rejected revocations", o => o.StillActive);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void TreatAnEmptyStringSuccessorAsUnboundAndLeaveTheHandleOpen()
    {
        // An empty-string successor with a legitimately-signed UNBOUND proof must not arm the gate:
        // null and "" are one "unbound" value end to end, so the freed handle stays openly
        // re-registrable rather than bricking (bam.protocol#25 review B1/B2).
        RsaPublicPrivateKeyPair originalKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair successorKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(TreatAnEmptyStringSuccessorAsUnboundAndLeaveTheHandleOpen));

        When.A<KeySetRevocation>("treats an empty-string successor as unbound and leaves the handle openly re-registrable",
            () => CreateRevocation(repository, adminKeyPair),
            (revocation) =>
            {
                PublicKeySetRegistrar registrar = CreateRegistrar(repository);
                registrar.Register(new PublicKeySetData { KeySetHandle = "handle", PublicRsaKey = originalKeyPair.PublicKeyPem });
                PublicKeySetData active = registrar.Resolve("handle")!;

                // admin signs an UNBOUND revocation; the caller passes "" instead of null
                byte[] unboundProof = SignRevocation(adminKeyPair, active, null);
                PublicKeySetData tombstoned = revocation.Revoke("handle", unboundProof, "");

                // any successor can claim the freed handle — the gate was NOT armed by ""
                registrar.Register(new PublicKeySetData { KeySetHandle = "handle", PublicRsaKey = successorKeyPair.PublicKeyPem });
                PublicKeySetData? resolved = registrar.Resolve("handle");

                return new EmptySuccessorOutcome(
                    tombstoned.AuthorizedSuccessorFingerprint == null,
                    resolved != null && resolved.PublicRsaKey == successorKeyPair.PublicKeyPem);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<EmptySuccessorOutcome>("the tombstone records no successor binding (empty normalized to null)", o => o.NoSuccessorBound)
                .As<EmptySuccessorOutcome>("the freed handle is openly re-registrable", o => o.OpenlyReRegistrable);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RejectRevocationWithAMalformedSuccessorFingerprint()
    {
        // A non-null successor must be a canonical fingerprint (64-char lowercase hex). A malformed
        // value — uppercase hex, truncated, a raw PEM — could never match a candidate recomputed via
        // PublicKeyFingerprint.Of and would brick the handle, so it is rejected at bind time
        // (bam.protocol#25 review B2 / Condition 3).
        RsaPublicPrivateKeyPair keyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair adminKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(RejectRevocationWithAMalformedSuccessorFingerprint));

        When.A<KeySetRevocation>("rejects a revocation whose successor fingerprint is not canonical",
            () => CreateRevocation(repository, adminKeyPair),
            (revocation) =>
            {
                PublicKeySetRegistrar registrar = CreateRegistrar(repository);
                registrar.Register(new PublicKeySetData { KeySetHandle = "holder", PublicRsaKey = keyPair.PublicKeyPem });
                PublicKeySetData active = registrar.Resolve("holder")!;

                byte[] proof = SignRevocation(adminKeyPair, active, "not-a-canonical-fingerprint");
                bool rejected = false;
                try
                {
                    revocation.Revoke("holder", proof, "not-a-canonical-fingerprint");
                }
                catch (ArgumentException)
                {
                    rejected = true;
                }

                bool stillActive = registrar.Resolve("holder") != null;
                return new MalformedFingerprintOutcome(rejected, stillActive);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<MalformedFingerprintOutcome>("a malformed successor fingerprint threw ArgumentException", o => o.Rejected)
                .As<MalformedFingerprintOutcome>("the key set stays active (no bricked tombstone persisted)", o => o.StillActive);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void NeutralizeACallerPlantedGoverningTombstone()
    {
        // A caller cannot plant a governing tombstone by supplying RevokedUtc +
        // AuthorizedSuccessorFingerprint on a fresh registration: the registrar server-clears those
        // fields, so the row is persisted ACTIVE (a normal first-registration) and cannot later
        // authorize an attacker's real key as a "successor" (bam.protocol#25 review B1).
        RsaPublicPrivateKeyPair throwawayKeyPair = new RsaPublicPrivateKeyPair();
        RsaPublicPrivateKeyPair attackerKeyPair = new RsaPublicPrivateKeyPair();
        ObjectDataRepository repository = CreateObjectDataRepository(nameof(NeutralizeACallerPlantedGoverningTombstone));

        When.A<PublicKeySetRegistrar>("neutralizes a caller-planted governing tombstone by clearing caller revocation fields",
            () => CreateRegistrar(repository),
            (registrar) =>
            {
                // attacker plants a tombstone: a far-future RevokedUtc + a binding to their own key
                registrar.Register(new PublicKeySetData
                {
                    KeySetHandle = "handle",
                    PublicRsaKey = throwawayKeyPair.PublicKeyPem,
                    RevokedUtc = DateTime.MaxValue,
                    RevokedBy = "planted",
                    AuthorizedSuccessorFingerprint = PublicKeyFingerprint.Of(attackerKeyPair.PublicKeyPem)
                });

                // the planted row must be ACTIVE (RevokedUtc cleared) and carry no binding
                PublicKeySetData? resolved = registrar.Resolve("handle");
                bool resolvesActive = resolved != null && resolved.PublicRsaKey == throwawayKeyPair.PublicKeyPem;
                bool fieldsCleared = resolved != null && resolved.RevokedUtc == null && resolved.AuthorizedSuccessorFingerprint == null;

                // the attacker's real key cannot ride a planted binding into the handle — it is now
                // an ordinary taken handle, so registration conflicts (NOT an authorized-successor pass)
                bool realKeyBlocked = false;
                try
                {
                    registrar.Register(new PublicKeySetData { KeySetHandle = "handle", PublicRsaKey = attackerKeyPair.PublicKeyPem });
                }
                catch (PublicKeySetConflictException)
                {
                    realKeyBlocked = true;
                }

                return new PlantedTombstoneOutcome(resolvesActive, fieldsCleared, realKeyBlocked);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<PlantedTombstoneOutcome>("the planted row is persisted active, not as a tombstone", o => o.ResolvesActive)
                .As<PlantedTombstoneOutcome>("the caller-supplied revocation fields were cleared server-side", o => o.FieldsCleared)
                .As<PlantedTombstoneOutcome>("the attacker's real key cannot claim the handle via a planted binding", o => o.RealKeyBlocked);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed record SignatureMismatchOutcome(bool MismatchedRejected, bool UnboundVsBoundRejected, bool StillActive);

    private sealed record EmptySuccessorOutcome(bool NoSuccessorBound, bool OpenlyReRegistrable);

    private sealed record MalformedFingerprintOutcome(bool Rejected, bool StillActive);

    private sealed record PlantedTombstoneOutcome(bool ResolvesActive, bool FieldsCleared, bool RealKeyBlocked);

    private sealed record SuccessorPersistedOutcome(bool RevokedUtcSet, bool SuccessorFingerprintPersisted);

    private sealed record HijackOutcome(bool HijackRejected, bool NoActiveAfterHijack, bool SuccessorClaimed);

    private sealed record GoverningTombstoneOutcome(bool StaleSuccessorRejected, bool LatestSuccessorClaimed, bool FurtherRegistrationBlocked);

    private sealed record RotationBlocklistOutcome(bool Blocked, bool RevokedMaterialActiveAgain);

    private sealed record RevokeOutcome(bool RevokedUtcSet, bool RevokedBySet, bool ResolvesNullAfter);

    private sealed record BlocklistOutcome(bool BlockedUnderNewHandle, bool BlockedUnderSameHandle);

    private sealed record RejectRevokeOutcome(bool Rejected, bool KeySetUnchanged);

    private sealed record ReplayOutcome(bool ReplayRejected, bool ReRegistrationStillActive);
}
