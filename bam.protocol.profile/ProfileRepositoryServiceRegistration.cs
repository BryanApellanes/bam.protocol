using Bam.Data.Dynamic.Objects;
using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Storage;

namespace Bam.Protocol.Profile;

public static class ProfileRepositoryServiceRegistration
{
    public static ServiceRegistry AddEncryptedProfileRepository(this ServiceRegistry registry, string rootPath)
    {
        AesKey aesKey = new AesKey();

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
            .For<IKeySetRotationVerifier>().Use<RsaKeySetRotationVerifier>()
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
                    serviceRegistry.Get<IPublicKeySetResolver>()))
            .For<IProfileRepository>().Use<EncryptedProfileRepository>(
                serviceRegistry => new EncryptedProfileRepository(
                    serviceRegistry.Get<ObjectDataRepository>(),
                    serviceRegistry.Get<IPublicKeySetRegistrar>(),
                    serviceRegistry.Get<IKeySetRevocation>(),
                    serviceRegistry.Get<IPublicKeySetResolver>()));

        return registry;
    }
}
