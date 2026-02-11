using Bam.Data.SQLite;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Server;
using Bam.Protocol.Data.Server.Dao.Repository;
using Bam.Protocol.Profile;
using Bam.Protocol.Profile.Registration;
using Bam.Test;
using NSubstitute;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("AccountManager Should", Selector = "ams")]
public class AccountManagerShould : UnitTestMenuContainer
{
    private static ServerSessionSchemaRepository CreateServerRepository(string testName)
    {
        return new ServerSessionSchemaRepository()
        {
            Database = new SQLiteDatabase(new FileInfo($"./.bam/tests/{testName}.sqlite"))
        };
    }

    private static IProfileManager CreateMockProfileManager(string personHandle = "testPerson", string profileHandle = "testProfile")
    {
        IProfileManager profileManager = Substitute.For<IProfileManager>();
        IProfile mockProfile = Substitute.For<IProfile>();
        mockProfile.PersonHandle.Returns(personHandle);
        mockProfile.ProfileHandle.Returns(profileHandle);
        profileManager.RegisterPersonProfile(Arg.Any<PersonRegistrationData>()).Returns(mockProfile);
        return profileManager;
    }

    [UnitTest]
    public void RegisterAccount()
    {
        When.A<AccountManager>("registers an account",
            () => new AccountManager(
                CreateMockProfileManager(),
                CreateServerRepository(nameof(RegisterAccount)),
                "test.server.com"),
            (manager) =>
            {
                PersonRegistrationData registration = new PersonRegistrationData
                {
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test@example.com",
                };
                AccountData account = manager.RegisterAccount(registration);
                return account;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<AccountData>("PersonHandle is set", a => !string.IsNullOrEmpty(a.PersonHandle));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RegisterAccountCreatesProfile()
    {
        IProfileManager mockProfileManager = CreateMockProfileManager();

        When.A<AccountManager>("creates a profile during registration",
            () => new AccountManager(
                mockProfileManager,
                CreateServerRepository(nameof(RegisterAccountCreatesProfile)),
                "test.server.com"),
            (manager) =>
            {
                PersonRegistrationData registration = new PersonRegistrationData
                {
                    FirstName = "Test",
                    LastName = "User",
                };
                manager.RegisterAccount(registration);
                return mockProfileManager;
            })
        .TheTest
        .ShouldPass(because =>
        {
            IProfileManager pm = because.TheResult.As<IProfileManager>();
            pm.Received(1).RegisterPersonProfile(Arg.Any<PersonRegistrationData>());
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RegisterAccountCreatesServerAccountData()
    {
        ServerSessionSchemaRepository repo = CreateServerRepository(nameof(RegisterAccountCreatesServerAccountData));

        When.A<AccountManager>("persists server account data",
            () => new AccountManager(
                CreateMockProfileManager("person1", "profile1"),
                repo,
                "my.server.com"),
            (manager) =>
            {
                PersonRegistrationData registration = new PersonRegistrationData
                {
                    FirstName = "Test",
                    LastName = "User",
                };
                manager.RegisterAccount(registration);
                return repo;
            })
        .TheTest
        .ShouldPass(because =>
        {
            ServerSessionSchemaRepository r = because.TheResult.As<ServerSessionSchemaRepository>();
            ServerAccountData saved = r.OneServerAccountDataWhere(c => c.ProfileHandle == "profile1");
            because.ItsTrue("ServerAccountData was persisted", saved != null);
            because.ItsTrue("ProfileHandle is correct", saved?.ProfileHandle == "profile1");
            because.ItsTrue("Issuer is correct", saved?.Issuer == "my.server.com");
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
