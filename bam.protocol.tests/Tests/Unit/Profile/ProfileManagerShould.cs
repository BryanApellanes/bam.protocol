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

[UnitTestMenu("ProfileManager Should", Selector = "pms")]
public class ProfileManagerShould : UnitTestMenuContainer
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
    public void RegisterPersonProfile()
    {
        When.A<ProfileManager>("registers a person profile",
            () => new ProfileManager(CreateRepository(nameof(RegisterPersonProfile))),
            (manager) =>
            {
                PersonRegistrationData registration = new PersonRegistrationData
                {
                    FirstName = "Test",
                    LastName = "User",
                    Phone = "555-1234",
                    Email = "test@example.com",
                };

                IProfile profile = manager.RegisterPersonProfile(registration);
                return profile;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<IProfile>("ProfileHandle is not empty", p => !string.IsNullOrEmpty(p.ProfileHandle))
                .As<IProfile>("PersonHandle is not empty", p => !string.IsNullOrEmpty(p.PersonHandle))
                .As<IProfile>("Name is set", p => !string.IsNullOrEmpty(p.Name));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void FindProfileByProfileHandle()
    {
        When.A<ProfileManager>("finds a profile by profile handle",
            () => new ProfileManager(CreateRepository(nameof(FindProfileByProfileHandle))),
            (manager) =>
            {
                PersonRegistrationData registration = new PersonRegistrationData
                {
                    FirstName = "Find",
                    LastName = "ByHandle",
                    Handle = "findHandle1",
                };

                IProfile registered = manager.RegisterPersonProfile(registration);
                IProfile found = manager.FindProfileByHandle(registered.ProfileHandle);
                return new object[] { registered, found };
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .As<object[]>("found is not null", r => r[1] != null)
                .As<object[]>("ProfileHandle matches", r => ((IProfile)r[1])?.ProfileHandle == ((IProfile)r[0]).ProfileHandle);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void FindProfileByPersonHandle()
    {
        When.A<ProfileManager>("finds a profile by person handle",
            () => new ProfileManager(CreateRepository(nameof(FindProfileByPersonHandle))),
            (manager) =>
            {
                PersonRegistrationData registration = new PersonRegistrationData
                {
                    FirstName = "Find",
                    LastName = "ByPerson",
                    Handle = "personHandle1",
                };

                IProfile registered = manager.RegisterPersonProfile(registration);
                IProfile found = manager.FindProfileByHandle(registered.PersonHandle);
                return new object[] { registered, found };
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .As<object[]>("found is not null", r => r[1] != null)
                .As<object[]>("PersonHandle matches", r => ((IProfile)r[1])?.PersonHandle == ((IProfile)r[0]).PersonHandle);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GetProfileCreatesIfNotExists()
    {
        When.A<ProfileManager>("creates profile if not exists",
            () => new ProfileManager(CreateRepository(nameof(GetProfileCreatesIfNotExists))),
            (manager) =>
            {
                IProfile profile = manager.GetProfile("nonexistent", createIfNotExists: true);
                return profile;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<IProfile>("ProfileHandle is not empty", p => !string.IsNullOrEmpty(p.ProfileHandle));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GetProfileReturnsNullIfNotExists()
    {
        When.A<ProfileManager>("returns null if profile not exists",
            () => new ProfileManager(CreateRepository(nameof(GetProfileReturnsNullIfNotExists))),
            (manager) =>
            {
                IProfile profile = manager.GetProfile("nonexistent", createIfNotExists: false);
                return profile;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("result is null", because.Result == null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void CreateProfile()
    {
        When.A<ProfileManager>("creates a blank profile",
            () => new ProfileManager(CreateRepository(nameof(CreateProfile))),
            (manager) =>
            {
                IProfile profile = manager.CreateProfile();
                return profile;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<IProfile>("ProfileHandle is not empty", p => !string.IsNullOrEmpty(p.ProfileHandle));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

}
