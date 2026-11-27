using Bam.Data.Dynamic.Objects;
using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Storage;

namespace Bam.Protocol.Profile;

public static class ProfileRepositoryServiceRegistration
{
    /// <summary>
    /// Binds the encrypted profile store on <paramref name="rootPath"/> with a key made for this
    /// process only: nothing it writes can be read after a restart.  For tests and throwaway
    /// stores; a host that keeps profiles, key sets or challenges across restarts uses
    /// <see cref="AddEncryptedProfileRepository(ServiceRegistry, string, IAesKeySource)"/>
    /// (bam.protocol#45).
    /// </summary>
    /// <param name="registry">The registry to bind into.</param>
    /// <param name="rootPath">Where the store keeps its files.</param>
    /// <returns><paramref name="registry"/>.</returns>
    public static ServiceRegistry AddEncryptedProfileRepository(this ServiceRegistry registry, string rootPath)
    {
        return registry.AddEncryptedProfileRepository(rootPath, new AesKey());
    }

    /// <summary>
    /// Binds the encrypted profile store on <paramref name="rootPath"/>, encrypting with the key
    /// <paramref name="storeKey"/> supplies (bam.protocol#45).  Pass the same persisted key on
    /// every start, for example one loaded from a secret file, and the store reads back what an
    /// earlier process wrote.  The key is read once, here, and copied: the store's encryptor,
    /// decryptor and <see cref="IAesKeySource"/> all use that copy, so disposing the caller's key
    /// afterwards (which zeroes it) doesn't change what the store encrypts with.
    /// <para>
    /// A key that AES can't use (a key other than 16, 24 or 32 bytes, an IV other than 16 bytes) or
    /// an all-zero key is refused here.  A <em>wrong</em> key of the right size is not detected:
    /// the store then reads as empty and accepts new registrations, so a host that loses or swaps
    /// its key sees its actors vanish rather than an error.  Keep the key with the deployment's
    /// secrets, not beside the data.  Every file shares the key's IV under AES-CBC with no MAC, so
    /// equal leading blocks are visible at rest (bam.encryption#12).
    /// </para>
    /// </summary>
    /// <param name="registry">The registry to bind into.</param>
    /// <param name="rootPath">Where the store keeps its files.</param>
    /// <param name="storeKey">Supplies the store's AES key and IV.</param>
    /// <returns><paramref name="registry"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="storeKey"/> is null.</exception>
    /// <exception cref="ArgumentException">The key source has no key or IV, the key isn't 16, 24 or 32 bytes, the IV isn't 16 bytes, or the key is all zeros.</exception>
    public static ServiceRegistry AddEncryptedProfileRepository(this ServiceRegistry registry, string rootPath, IAesKeySource storeKey)
    {
        ArgumentNullException.ThrowIfNull(storeKey);
        AesKey source = storeKey.GetAesKey();
        if (source is null || source.Key is null || source.Key.Length == 0 || source.IV is null || source.IV.Length == 0)
        {
            throw new ArgumentException("The store key source returned no key or IV.", nameof(storeKey));
        }
        if (source.Key.Length != 16 && source.Key.Length != 24 && source.Key.Length != 32)
        {
            throw new ArgumentException("The store key must be 16, 24 or 32 bytes.", nameof(storeKey));
        }
        if (source.IV.Length != 16)
        {
            throw new ArgumentException("The store key's IV must be 16 bytes.", nameof(storeKey));
        }
        if (source.Key.All(b => b == 0))
        {
            throw new ArgumentException("The store key is all zeros.", nameof(storeKey));
        }
        // A private copy: AesKey.Dispose zeroes its arrays in place, and the caller may dispose theirs.
        AesKey aesKey = new AesKey((byte[])source.Key.Clone(), (byte[])source.IV.Clone());

        registry
            .For<IRootStorageHolder>().Use(new RootStorageHolder(rootPath))
            .For<IAesKeySource>().Use(aesKey)
            .For<IEncryptor>().Use(new AesEncryptor(aesKey))
            .For<IDecryptor>().Use(new AesDecryptor(aesKey))
            .For<ICompositeKeyCalculator>().Use<CompositeKeyCalculator>()
            .For<IObjectDataIdentityCalculator>().Use<ObjectDataIdentityCalculator>()
            .For<IObjectDataLocatorFactory>().Use<ObjectDataLocatorFactory>()
            .For<IObjectEncoderDecoder>().Use<JsonObjectDataEncoder>()
            .For<IObjectDataFactory>().Use<ObjectDataFactory>()
            .For<IObjectDataStorageManager>().Use<EncryptedFsObjectDataStorageManager>()
            .For<IObjectDataWriter>().Use<ObjectDataWriter>()
            .For<IObjectDataReader>().Use<ObjectDataReader>()
            .For<IObjectDataIndexer>().Use<ObjectDataIndexer>()
            .For<IObjectDataSearchIndexer>().Use<ObjectDataSearchIndexer>()
            .For<IObjectDataSearcher>().Use<ObjectDataSearcher>()
            .For<IObjectDataDeleter>().Use<ObjectDataDeleter>()
            .For<IObjectDataArchiver>().Use<ObjectDataArchiver>()
            .For<ObjectDataRepository>().Use<ObjectDataRepository>()
            .For<ISignatureProvider>().Use<RsaSignatureProvider>()
            // Rotation proofs (bam.protocol#72): a host adds its own (bam.useraccounts adds a
            // passkey proof) to the registered KeySetRotationProofs; the chain reads it per rotation.
            .For<KeySetRotationProofs>().Use(KeySetRotationProofChain.DefaultProofs())
            .For<IKeySetRotationVerifier>().Use<KeySetRotationProofChain>(
                serviceRegistry => new KeySetRotationProofChain(serviceRegistry.Get<KeySetRotationProofs>()))
            // Factory names the full ctor so the resolver gets the search indexer it needs to
            // gate index-served material lookups (a lookup on an unindexed column would else
            // degrade to a full scan — bam.protocol#24 round-2 security condition 1). Without
            // the factory the first-satisfiable ctor would supply null and every material
            // resolve would scan.
            .For<IPublicKeySetResolver>().Use<PublicKeySetResolver>(
                serviceRegistry => new PublicKeySetResolver(
                    serviceRegistry.Get<ObjectDataRepository>(),
                    serviceRegistry.Get<IObjectDataSearchIndexer>()))
            .For<IPublicKeySetAudit>().Use<PublicKeySetAudit>()
            // Break-glass revocation (bam.protocol#11). The admin key is left unconfigured here
            // (fail-closed — no revocation can be authorized); a consumer overrides
            // IAdminPublicKeySource with the offline YubiKey's public key per deployment.
            .For<IAdminPublicKeySource>().Use(new StaticAdminPublicKeySource(null))
            .For<IRevocationAuthority>().Use<RsaRevocationAuthority>()
            .For<IKeySetRevocation>().Use<KeySetRevocation>()
            // Construct the registrar and repository through factories that name their FULL
            // constructors: DependencyProvider.GetCtorParams selects the FIRST satisfiable
            // constructor, which for both types is a convenience ctor that self-composes
            // defaults — resolving without these factories would silently ignore the
            // registry-supplied IPublicKeySetResolver / IPublicKeySetRegistrar /
            // IKeySetRevocation (bam.protocol#8 review SF1; extended by #13 and #11).
            .For<IPublicKeySetRegistrar>().Use<PublicKeySetRegistrar>(
                serviceRegistry => new PublicKeySetRegistrar(
                    serviceRegistry.Get<ObjectDataRepository>(),
                    serviceRegistry.Get<IKeySetRotationVerifier>(),
                    serviceRegistry.Get<IPublicKeySetResolver>(),
                    serviceRegistry.Get<KeySetRotationPolicy>()))
            .For<IProfileRepository>().Use<EncryptedProfileRepository>(
                serviceRegistry => new EncryptedProfileRepository(
                    serviceRegistry.Get<ObjectDataRepository>(),
                    serviceRegistry.Get<IPublicKeySetRegistrar>(),
                    serviceRegistry.Get<IKeySetRevocation>(),
                    serviceRegistry.Get<IPublicKeySetResolver>()));

        // No rotation limit unless the host binds one (bam.protocol#89). Registered only when nothing is bound yet, so a
        // policy the host bound before this call is kept: replacing it with None would fail open, silently.
        if (!registry.Contains<KeySetRotationPolicy>())
        {
            registry.For<KeySetRotationPolicy>().Use(KeySetRotationPolicy.None);
        }

        return registry;
    }
}
