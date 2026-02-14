using System.Text;
using Bam.Data.Dynamic.Objects;
using Bam.Data.Objects;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile;
using Bam.Storage;
using Bam.Test;
using NSubstitute;
using Org.BouncyCastle.Crypto;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("KeyManager Should", Selector = "kms")]
public class KeyManagerShould : UnitTestMenuContainer
{
    private static IProfileRepository CreateRepository(string testName)
    {
        string rootPath = $"./.bam/tests/{testName}";
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
    public void GenerateRsaKeyPair()
    {
        When.A<KeyManager>("generates an RSA key pair",
            () => new KeyManager(),
            (keyManager) =>
            {
                RsaPublicPrivateKeyPair keyPair = keyManager.GenerateRsaKeyPair();
                return keyPair;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<RsaPublicPrivateKeyPair>("PublicKeyPem is not empty", kp => !string.IsNullOrEmpty(kp.PublicKeyPem));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GenerateEccKeyPair()
    {
        When.A<KeyManager>("generates an ECC key pair",
            () => new KeyManager(),
            (keyManager) =>
            {
                EccPublicPrivateKeyPair keyPair = keyManager.GenerateEccKeyPair();
                return keyPair;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<EccPublicPrivateKeyPair>("PublicKeyPem is not empty", kp => !string.IsNullOrEmpty(kp.PublicKeyPem));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GenerateAesKey()
    {
        When.A<KeyManager>("generates an AES key",
            () => new KeyManager(),
            (keyManager) =>
            {
                AesKey aesKey = keyManager.GenerateAesKey();
                return aesKey;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<AesKey>("Key is not null", k => k.Key != null)
                .As<AesKey>("IV is not null", k => k.IV != null)
                .As<AesKey>("Key has length", k => k.Key.Length > 0)
                .As<AesKey>("IV has length", k => k.IV.Length > 0);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GetSigningKey()
    {
        IPrivateKey mockPrivateKey = Substitute.For<IPrivateKey>();
        IPrivateKeyManager privateKeyManager = Substitute.For<IPrivateKeyManager>();
        privateKeyManager.GetPrivateRsaKey(Arg.Any<IPublicKey>()).Returns(mockPrivateKey);

        RsaPublicPrivateKeyPair rsaKeyPair = new RsaPublicPrivateKeyPair();

        When.A<KeyManager>("retrieves signing key for actor",
            () =>
            {
                IProfileRepository repo = CreateRepository(nameof(GetSigningKey));
                repo.SavePublicKeySet(new PublicKeySetData
                {
                    KeySetHandle = "signingActor",
                    PublicRsaKey = rsaKeyPair.PublicKeyPem,
                });
                return new KeyManager(repo, privateKeyManager);
            },
            (keyManager) =>
            {
                IActor actor = Substitute.For<IActor>();
                actor.Handle.Returns("signingActor");
                IPrivateKey result = keyManager.GetSigningKey(actor);
                return result;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
            because.ItsTrue("returns expected private key", because.Result == mockPrivateKey);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GetEncryptionKey()
    {
        IPrivateKey mockPrivateKey = Substitute.For<IPrivateKey>();
        IPrivateKeyManager privateKeyManager = Substitute.For<IPrivateKeyManager>();
        privateKeyManager.GetPrivateEccKey(Arg.Any<IPublicKey>()).Returns(mockPrivateKey);

        EccPublicPrivateKeyPair eccKeyPair = new EccPublicPrivateKeyPair();

        When.A<KeyManager>("retrieves encryption key for actor",
            () =>
            {
                IProfileRepository repo = CreateRepository(nameof(GetEncryptionKey));
                repo.SavePublicKeySet(new PublicKeySetData
                {
                    KeySetHandle = "encryptionActor",
                    PublicEccKey = eccKeyPair.PublicKeyPem,
                });
                return new KeyManager(repo, privateKeyManager);
            },
            (keyManager) =>
            {
                IActor actor = Substitute.For<IActor>();
                actor.Handle.Returns("encryptionActor");
                IPrivateKey result = keyManager.GetEncryptionKey(actor);
                return result;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
            because.ItsTrue("returns expected private key", because.Result == mockPrivateKey);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GenerateSharedAesKey()
    {
        AsymmetricCipherKeyPair fromKeyPair = EccPublicPrivateKeyPair.Generate();
        byte[] fromPemBytes = fromKeyPair.ToPem(Encoding.UTF8);
        string fromPublicPem = fromKeyPair.PublicKeyToPem();

        EccPublicPrivateKeyPair toEcc = new EccPublicPrivateKeyPair();
        string toPublicPem = toEcc.PublicKeyPem;

        IPrivateKey fromPrivateKey = Substitute.For<IPrivateKey>();
        fromPrivateKey.Pem.Returns(fromPemBytes);

        IPrivateKeyManager privateKeyManager = Substitute.For<IPrivateKeyManager>();
        privateKeyManager.GetPrivateEccKey(Arg.Any<IPublicKey>()).Returns(fromPrivateKey);

        When.A<KeyManager>("generates shared AES key between two actors",
            () =>
            {
                IProfileRepository repo = CreateRepository(nameof(GenerateSharedAesKey));
                repo.SavePublicKeySet(new PublicKeySetData
                {
                    KeySetHandle = "fromActor",
                    PublicEccKey = fromPublicPem,
                });
                repo.SavePublicKeySet(new PublicKeySetData
                {
                    KeySetHandle = "toActor",
                    PublicEccKey = toPublicPem,
                });
                return new KeyManager(repo, privateKeyManager);
            },
            (keyManager) =>
            {
                IActor fromActor = Substitute.For<IActor>();
                fromActor.Handle.Returns("fromActor");
                IActor toActor = Substitute.For<IActor>();
                toActor.Handle.Returns("toActor");

                AesKey sharedKey = keyManager.GenerateSharedAesKey(fromActor, toActor);
                return sharedKey;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<AesKey>("shared key has Key", k => k.Key != null && k.Key.Length > 0)
                .As<AesKey>("shared key has IV", k => k.IV != null && k.IV.Length > 0);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
