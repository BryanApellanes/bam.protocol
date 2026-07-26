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
        IObjectDataArchiver archiver = new ObjectDataArchiver();
        return new ObjectDataRepository(factory, writer, indexer, deleter, archiver, reader, searcher, searchIndexer, compositeKeyCalculator);
    }

    private static PublicKeySetRegistrar CreateRegistrar(ObjectDataRepository repository)
    {
        return new PublicKeySetRegistrar(repository, new RsaKeySetRotationVerifier(new RsaSignatureProvider()));
    }

    private static KeySetRevocation CreateRevocation(ObjectDataRepository repository, RsaPublicPrivateKeyPair adminKeyPair)
    {
        StaticAdminPublicKeySource adminSource = new StaticAdminPublicKeySource(adminKeyPair.PublicKeyPem);
        return new KeySetRevocation(repository, new RsaRevocationAuthority(new RsaSignatureProvider(), adminSource), adminSource);
    }

    private static byte[] SignRevocation(RsaPublicPrivateKeyPair adminKeyPair, PublicKeySetData target)
    {
        RsaSignatureProvider signatureProvider = new RsaSignatureProvider();
        ISignature signature = signatureProvider.Sign(adminKeyPair, RevocationPayload.Compose(target), RsaRevocationAuthority.Algorithm);
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
                PublicKeySetData tombstoned = revocation.Revoke("holder", proof);

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
                revocation.Revoke("recovered", SignRevocation(adminKeyPair, active));

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
                revocation.Revoke("victim", SignRevocation(adminKeyPair, active));

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
                    revocation.Revoke("holder", forgedProof);
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
                    revocation.Revoke("unregistered", new byte[] { 1, 2, 3 });
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
                revocation.Revoke("target", capturedProof);

                // the owner recovers the handle with a fresh key
                registrar.Register(new PublicKeySetData { KeySetHandle = "target", PublicRsaKey = reRegisteredKeyPair.PublicKeyPem });

                // replaying the captured proof must not revoke the re-registration (target-bound to the old Uuid/key)
                bool replayRejected = false;
                try
                {
                    revocation.Revoke("target", capturedProof);
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

    private sealed record RevokeOutcome(bool RevokedUtcSet, bool RevokedBySet, bool ResolvesNullAfter);

    private sealed record BlocklistOutcome(bool BlockedUnderNewHandle, bool BlockedUnderSameHandle);

    private sealed record RejectRevokeOutcome(bool Rejected, bool KeySetUnchanged);

    private sealed record ReplayOutcome(bool ReplayRejected, bool ReRegistrationStillActive);
}
