using Bam.Data.Dynamic.Objects;
using Bam.Data.Objects;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile;
using Bam.Protocol.Profile.Registration;
using Bam.Storage;
using Bam.Test;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("OrganizationRegistration Should", Selector = "ors")]
public class OrganizationRegistrationShould : UnitTestMenuContainer
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
    public void RegisterOrganization()
    {
        When.A<ProfileManager>("registers an organization",
            () => new ProfileManager(CreateRepository(nameof(RegisterOrganization))),
            (manager) =>
            {
                OrganizationRegistrationData registration = new OrganizationRegistrationData
                {
                    Handle = "testOrg1",
                    Name = "Test Organization",
                };

                OrganizationData org = manager.RegisterOrganization(registration);
                return org;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<OrganizationData>("Handle matches", o => o.Handle == "testOrg1")
                .As<OrganizationData>("Name matches", o => o.Name == "Test Organization");
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void FindOrganizationByHandle()
    {
        When.A<ProfileManager>("finds an organization by handle",
            () => new ProfileManager(CreateRepository(nameof(FindOrganizationByHandle))),
            (manager) =>
            {
                OrganizationRegistrationData registration = new OrganizationRegistrationData
                {
                    Handle = "findableOrg1",
                    Name = "Findable Org",
                };

                manager.RegisterOrganization(registration);

                OrganizationData found = manager.FindOrganizationByHandle("findableOrg1");
                return found;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<OrganizationData>("Handle matches", o => o.Handle == "findableOrg1")
                .As<OrganizationData>("Name matches", o => o.Name == "Findable Org");
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
