using System.Text;
using Bam.Data.SQLite;
using Bam.Encryption;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Data.Profile.Dao.Repository;
using Bam.Protocol.Profile;
using Bam.Test;
using NSubstitute;
using Org.BouncyCastle.Crypto;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("KeyManager Should", Selector = "kms")]
public class KeyManagerShould : UnitTestMenuContainer
{
    private static ProfileSchemaRepository CreateRepository(string testName)
    {
        ProfileSchemaRepository repo = new ProfileSchemaRepository()
        {
            Database = new SQLiteDatabase(new FileInfo($"./.bam/tests/{testName}.sqlite"))
        };
        repo.Initialize();
        return repo;
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
            because.TheResult.IsNotNull();
            RsaPublicPrivateKeyPair keyPair = (RsaPublicPrivateKeyPair)because.Result;
            because.ItsTrue("PublicKeyPem is not empty", !string.IsNullOrEmpty(keyPair.PublicKeyPem));
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
            because.TheResult.IsNotNull();
            EccPublicPrivateKeyPair keyPair = (EccPublicPrivateKeyPair)because.Result;
            because.ItsTrue("PublicKeyPem is not empty", !string.IsNullOrEmpty(keyPair.PublicKeyPem));
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
            because.TheResult.IsNotNull();
            AesKey aesKey = (AesKey)because.Result;
            because.ItsTrue("Key is not null", aesKey.Key != null);
            because.ItsTrue("IV is not null", aesKey.IV != null);
            because.ItsTrue("Key has length", aesKey.Key.Length > 0);
            because.ItsTrue("IV has length", aesKey.IV.Length > 0);
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
                ProfileSchemaRepository repo = CreateRepository(nameof(GetSigningKey));
                repo.Save(new PublicKeySetData
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
                ProfileSchemaRepository repo = CreateRepository(nameof(GetEncryptionKey));
                repo.Save(new PublicKeySetData
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
                ProfileSchemaRepository repo = CreateRepository(nameof(GenerateSharedAesKey));
                repo.Save(new PublicKeySetData
                {
                    KeySetHandle = "fromActor",
                    PublicEccKey = fromPublicPem,
                });
                repo.Save(new PublicKeySetData
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
            because.TheResult.IsNotNull();
            AesKey sharedKey = (AesKey)because.Result;
            because.ItsTrue("shared key has Key", sharedKey.Key != null && sharedKey.Key.Length > 0);
            because.ItsTrue("shared key has IV", sharedKey.IV != null && sharedKey.IV.Length > 0);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
