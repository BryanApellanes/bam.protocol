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

[UnitTestMenu("AgentRegistration Should", Selector = "ars")]
public class AgentRegistrationShould : UnitTestMenuContainer
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
    public void RegisterAgent()
    {
        When.A<ProfileManager>("registers an agent",
            () => new ProfileManager(CreateRepository(nameof(RegisterAgent))),
            (manager) =>
            {
                PersonRegistrationData personReg = new PersonRegistrationData
                {
                    FirstName = "Agent",
                    LastName = "Tester",
                    Handle = "agentTesterPerson",
                };
                IProfile personProfile = manager.RegisterPersonProfile(personReg);

                DeviceRegistrationData deviceReg = new DeviceRegistrationData
                {
                    Handle = "agentTesterDevice",
                    Name = "AgentDevice",
                    DeviceType = DeviceTypes.DesktopWindows,
                };
                manager.RegisterDeviceProfile(deviceReg, personProfile.PersonHandle);

                AgentRegistrationData agentReg = new AgentRegistrationData
                {
                    Handle = "agentHandle1",
                    Name = "TestAgent",
                    PersonHandle = "agentTesterPerson",
                    DeviceHandle = "agentTesterDevice",
                };

                AgentData agent = manager.RegisterAgent(agentReg);
                return agent;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<AgentData>("Handle matches", a => a.Handle == "agentHandle1")
                .As<AgentData>("Name matches", a => a.Name == "TestAgent");
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void FindAgentByHandle()
    {
        When.A<ProfileManager>("finds an agent by handle",
            () => new ProfileManager(CreateRepository(nameof(FindAgentByHandle))),
            (manager) =>
            {
                PersonRegistrationData personReg = new PersonRegistrationData
                {
                    FirstName = "Agent",
                    LastName = "Finder",
                    Handle = "agentFinderPerson",
                };
                IProfile personProfile = manager.RegisterPersonProfile(personReg);

                DeviceRegistrationData deviceReg = new DeviceRegistrationData
                {
                    Handle = "agentFinderDevice",
                    Name = "FinderDevice",
                    DeviceType = DeviceTypes.DesktopLinux,
                };
                manager.RegisterDeviceProfile(deviceReg, personProfile.PersonHandle);

                AgentRegistrationData agentReg = new AgentRegistrationData
                {
                    Handle = "findableAgent1",
                    Name = "FindableAgent",
                    PersonHandle = "agentFinderPerson",
                    DeviceHandle = "agentFinderDevice",
                };
                manager.RegisterAgent(agentReg);

                AgentData found = manager.FindAgentByHandle("findableAgent1");
                return found;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<AgentData>("Handle matches", a => a.Handle == "findableAgent1")
                .As<AgentData>("Name matches", a => a.Name == "FindableAgent");
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
