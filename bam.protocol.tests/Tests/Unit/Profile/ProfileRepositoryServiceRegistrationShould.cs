using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile;
using Bam.Test;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("ProfileRepositoryServiceRegistration Should", Selector = "prsr")]
public class ProfileRepositoryServiceRegistrationShould : UnitTestMenuContainer
{
    [UnitTest]
    public void ResolveProfileRepositoryHonoringRegisteredRegistrar()
    {
        string rootPath = $"./.bam/tests/{nameof(ResolveProfileRepositoryHonoringRegisteredRegistrar)}";
        if (Directory.Exists(rootPath))
        {
            Directory.Delete(rootPath, true);
        }

        When.A<IProfileRepository>("resolves IProfileRepository using the registry-supplied IPublicKeySetRegistrar",
            () =>
            {
                ServiceRegistry registry = new ServiceRegistry();
                registry.AddEncryptedProfileRepository(rootPath);
                // Override the registrar with a marker AFTER the default registration. If the
                // IProfileRepository factory honors the registry (SF1 fix), resolving and calling
                // SavePublicKeySet routes through the marker. The old first-satisfiable-ctor
                // wiring would self-compose the default policy and never see this marker.
                registry.For<IPublicKeySetRegistrar>().Use(new MarkerRegistrar());
                return registry.Get<IProfileRepository>();
            },
            (repository) =>
            {
                bool markerInvoked = false;
                try
                {
                    repository.SavePublicKeySet(new PublicKeySetData { KeySetHandle = "any", PublicRsaKey = "any" });
                }
                catch (MarkerInvokedException)
                {
                    markerInvoked = true;
                }
                return markerInvoked;
            })
        .TheTest
        .ShouldPass<bool>((because, _, markerInvoked) =>
        {
            because.ItsTrue("the registry-supplied registrar was used, not the self-composed default", markerInvoked);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ReadBackAKeySetAfterARestartWithTheSameStoreKey()
    {
        string rootPath = $"./.bam/tests/{nameof(ReadBackAKeySetAfterARestartWithTheSameStoreKey)}";
        if (Directory.Exists(rootPath))
        {
            Directory.Delete(rootPath, true);
        }
        RsaPublicPrivateKeyPair keys = new RsaPublicPrivateKeyPair();

        When.A<string>("registers a key set with a persisted store key, then rebuilds the registry over the same files with that key and with another",
            () => rootPath,
            root =>
            {
                AesKey storeKey = new AesKey();
                string persisted = storeKey.ToJson();

                ServiceRegistry first = new ServiceRegistry();
                first.AddEncryptedProfileRepository(root, storeKey);
                first.Get<IPublicKeySetRegistrar>().Register(new PublicKeySetData { KeySetHandle = "restarted", PublicRsaKey = keys.PublicKeyPem });

                ServiceRegistry restarted = new ServiceRegistry();
                restarted.AddEncryptedProfileRepository(root, persisted.FromJson<AesKey>());
                PublicKeySetData? readBack = restarted.Get<IPublicKeySetRegistrar>().Resolve("restarted");

                ServiceRegistry otherKey = new ServiceRegistry();
                otherKey.AddEncryptedProfileRepository(root, new AesKey());
                bool otherKeyReads = ReadsKeySet(otherKey, "restarted", keys.PublicKeyPem);

                return new RestartOutcome(readBack?.PublicRsaKey, keys.PublicKeyPem, otherKeyReads);
            })
        .TheTest
        .ShouldPass<RestartOutcome>((because, _, outcome) =>
        {
            because.ItsTrue("a registry rebuilt with the same persisted key reads the key set back", string.Equals(outcome.ReadBackPem, outcome.RegisteredPem, StringComparison.Ordinal));
            because.ItsFalse("a registry with a different key can't read it, so the store key is what the files depend on", outcome.OtherKeyReads);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RefuseAStoreKeySourceWithNoKey()
    {
        When.A<string>("binds the store with a null key source and with one that has no key",
            () => $"./.bam/tests/{nameof(RefuseAStoreKeySourceWithNoKey)}",
            root => new RefusalOutcome(
                Throws<ArgumentNullException>(() => new ServiceRegistry().AddEncryptedProfileRepository(root, null!)),
                Throws<ArgumentException>(() => new ServiceRegistry().AddEncryptedProfileRepository(root, new EmptyKeySource())),
                Throws<ArgumentException>(() => new ServiceRegistry().AddEncryptedProfileRepository(root, new AesKey(new byte[] { 1, 2, 3, 4, 5 }, RandomBytes(16)))),
                Throws<ArgumentException>(() => new ServiceRegistry().AddEncryptedProfileRepository(root, new AesKey(RandomBytes(32), RandomBytes(8)))),
                Throws<ArgumentException>(() => new ServiceRegistry().AddEncryptedProfileRepository(root, new AesKey(new byte[32], new byte[16])))))
        .TheTest
        .ShouldPass<RefusalOutcome>((because, _, outcome) =>
        {
            because.ItsTrue("a null key source is refused", outcome.NullRefused);
            because.ItsTrue("a key source with no key or IV is refused", outcome.EmptyRefused);
            because.ItsTrue("a 5-byte key is refused", outcome.ShortKeyRefused);
            because.ItsTrue("an 8-byte IV is refused", outcome.ShortIvRefused);
            because.ItsTrue("an all-zero key is refused", outcome.ZeroKeyRefused);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void KeepEncryptingWithTheKeyAfterTheCallerDisposesIt()
    {
        string rootPath = $"./.bam/tests/{nameof(KeepEncryptingWithTheKeyAfterTheCallerDisposesIt)}";
        if (Directory.Exists(rootPath))
        {
            Directory.Delete(rootPath, true);
        }
        RsaPublicPrivateKeyPair keys = new RsaPublicPrivateKeyPair();

        When.A<string>("registers with a key, disposes it, writes a key set, then reads back with a copy of the original key",
            () => rootPath,
            root =>
            {
                AesKey callerKey = new AesKey();
                string original = callerKey.ToJson();
                ServiceRegistry registry = new ServiceRegistry();
                registry.AddEncryptedProfileRepository(root, callerKey);
                callerKey.Dispose();
                registry.Get<IPublicKeySetRegistrar>().Register(new PublicKeySetData { KeySetHandle = "disposed", PublicRsaKey = keys.PublicKeyPem });

                ServiceRegistry restarted = new ServiceRegistry();
                restarted.AddEncryptedProfileRepository(root, original.FromJson<AesKey>());
                return ReadsKeySet(restarted, "disposed", keys.PublicKeyPem);
            })
        .TheTest
        .ShouldPass<bool>((because, _, readBack) =>
        {
            because.ItsTrue("what was written after the caller disposed its key reads back with the original key", readBack);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private static byte[] RandomBytes(int length)
    {
        return System.Security.Cryptography.RandomNumberGenerator.GetBytes(length);
    }

    private static bool ReadsKeySet(ServiceRegistry registry, string handle, string pem)
    {
        try
        {
            PublicKeySetData? found = registry.Get<IPublicKeySetRegistrar>().Resolve(handle);
            return string.Equals(found?.PublicRsaKey, pem, StringComparison.Ordinal);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool Throws<TException>(Action action) where TException : Exception
    {
        try
        {
            action();
            return false;
        }
        catch (TException)
        {
            return true;
        }
    }

    private sealed class EmptyKeySource : IAesKeySource
    {
        public AesKey GetAesKey()
        {
            return new AesKey(Array.Empty<byte>(), Array.Empty<byte>());
        }
    }

    private sealed class MarkerRegistrar : IPublicKeySetRegistrar
    {
        public PublicKeySetData Register(PublicKeySetData publicKeySetData)
        {
            throw new MarkerInvokedException();
        }

        public PublicKeySetData Rotate(PublicKeySetData newKeySet, byte[] rotationSignature)
        {
            throw new MarkerInvokedException();
        }

        public PublicKeySetData? Resolve(string keySetHandle)
        {
            throw new MarkerInvokedException();
        }
    }

    private sealed class MarkerInvokedException : Exception
    {
    }

    private sealed record RestartOutcome(string? ReadBackPem, string RegisteredPem, bool OtherKeyReads);

    private sealed record RefusalOutcome(bool NullRefused, bool EmptyRefused, bool ShortKeyRefused, bool ShortIvRefused, bool ZeroKeyRefused);
}
