using Bam.Data.Dynamic.Objects;
using Bam.Data.Objects;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile;
using Bam.Protocol.Profile.Registration;
using Bam.Storage;
using Bam.Test;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("DeviceRegistration Should", Selector = "drs")]
public class DeviceRegistrationShould : UnitTestMenuContainer
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
    public void RegisterDeviceProfile()
    {
        When.A<ProfileManager>("registers a device profile",
            () => new ProfileManager(CreateRepository(nameof(RegisterDeviceProfile))),
            (manager) =>
            {
                PersonRegistrationData personReg = new PersonRegistrationData
                {
                    FirstName = "Device",
                    LastName = "Tester",
                    Handle = "deviceTesterPerson",
                };

                IProfile personProfile = manager.RegisterPersonProfile(personReg);

                DeviceRegistrationData deviceReg = new DeviceRegistrationData
                {
                    Name = "TestDevice",
                    DeviceType = DeviceTypes.DesktopWindows,
                };

                IProfile updatedProfile = manager.RegisterDeviceProfile(deviceReg, personProfile.PersonHandle);
                return updatedProfile;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<IProfile>("DeviceHandle is not empty", p => !string.IsNullOrEmpty(p.DeviceHandle));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void FindDeviceByHandle()
    {
        When.A<ProfileManager>("finds a device by handle",
            () => new ProfileManager(CreateRepository(nameof(FindDeviceByHandle))),
            (manager) =>
            {
                PersonRegistrationData personReg = new PersonRegistrationData
                {
                    FirstName = "Device",
                    LastName = "Finder",
                    Handle = "deviceFinderPerson",
                };

                IProfile personProfile = manager.RegisterPersonProfile(personReg);

                DeviceRegistrationData deviceReg = new DeviceRegistrationData
                {
                    Handle = "findableDevice1",
                    Name = "FindableDevice",
                    DeviceType = DeviceTypes.DesktopLinux,
                };

                manager.RegisterDeviceProfile(deviceReg, personProfile.PersonHandle);

                DeviceData found = manager.FindDeviceByHandle("findableDevice1");
                return found;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<DeviceData>("Handle matches", d => d.Handle == "findableDevice1");
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
