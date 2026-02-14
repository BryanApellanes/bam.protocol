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
            .For<IProfileRepository>().Use<EncryptedProfileRepository>();

        return registry;
    }
}
