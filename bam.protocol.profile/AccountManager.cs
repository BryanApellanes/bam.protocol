using Bam.Protocol.Data;
using Bam.Protocol.Data.Server;
using Bam.Protocol.Profile;
using Bam.Protocol.Profile.Registration;

namespace Bam.Protocol;

public class AccountManager : IAccountManager
{
    public AccountManager(IProfileManager profileManager, IAccountRepository accountRepository, IServerIdentity serverIdentity)
    {
        this.ProfileManager = profileManager;
        this.AccountRepository = accountRepository;
        this.ServerIdentity = serverIdentity;
    }

    protected IProfileManager ProfileManager { get; }
    protected IAccountRepository AccountRepository { get; }
    protected IServerIdentity ServerIdentity { get; }

    public AccountData RegisterAccount(PersonRegistrationData data)
    {
        IProfile profile = ProfileManager.RegisterPersonProfile(data);

        ServerAccountData serverAccountData = new ServerAccountData
        {
            Issuer = ServerIdentity.ServerName,
            ProfileHandle = profile.ProfileHandle,
        };
        AccountRepository.Save(serverAccountData);

        return new AccountData
        {
            PersonHandle = profile.PersonHandle,
        };
    }

    public AccountData RegisterAccountWithDevice(PersonRegistrationData personData, DeviceRegistrationData deviceData)
    {
        IProfile profile = ProfileManager.RegisterPersonProfile(personData);

        IProfile updatedProfile = ProfileManager.RegisterDeviceProfile(deviceData, profile.PersonHandle);

        AgentRegistrationData agentRegistrationData = new AgentRegistrationData
        {
            Name = $"{profile.Name}@{deviceData.Name ?? Environment.MachineName}",
            PersonHandle = profile.PersonHandle,
            DeviceHandle = updatedProfile?.DeviceHandle!,
        };
        ProfileManager.RegisterAgent(agentRegistrationData);

        ServerAccountData serverAccountData = new ServerAccountData
        {
            Issuer = ServerIdentity.ServerName,
            ProfileHandle = profile.ProfileHandle,
        };
        AccountRepository.Save(serverAccountData);

        return new AccountData
        {
            PersonHandle = profile.PersonHandle,
        };
    }
}
