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
            .For<IPublicKeySetRegistrar>().Use<PublicKeySetRegistrar>()
            // Construct EncryptedProfileRepository via its two-arg constructor through a
            // factory: DependencyProvider.GetCtorParams selects the FIRST satisfiable
            // constructor, which is the one-arg convenience ctor — resolving IProfileRepository
            // without this factory would self-compose the default policy and silently ignore the
            // IPublicKeySetRegistrar registered above (bam.protocol#8 review SF1).
            .For<IProfileRepository>().Use<EncryptedProfileRepository>(
                serviceRegistry => new EncryptedProfileRepository(
                    serviceRegistry.Get<ObjectDataRepository>(),
                    serviceRegistry.Get<IPublicKeySetRegistrar>()));

        return registry;
    }
}
