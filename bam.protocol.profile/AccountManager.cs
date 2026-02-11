using Bam.Protocol.Data;
using Bam.Protocol.Data.Server;
using Bam.Protocol.Data.Server.Dao.Repository;
using Bam.Protocol.Profile;
using Bam.Protocol.Profile.Registration;

namespace Bam.Protocol;

public class AccountManager : IAccountManager
{
    public AccountManager(IProfileManager profileManager, ServerSessionSchemaRepository serverSessionRepository, string serverName)
    {
        this.ProfileManager = profileManager;
        this.ServerSessionRepository = serverSessionRepository;
        this.ServerName = serverName;
        serverSessionRepository.Initialize();
    }

    protected IProfileManager ProfileManager { get; }
    protected ServerSessionSchemaRepository ServerSessionRepository { get; }
    protected string ServerName { get; }

    public AccountData RegisterAccount(PersonRegistrationData data)
    {
        IProfile profile = ProfileManager.RegisterPersonProfile(data);

        ServerAccountData serverAccountData = new ServerAccountData
        {
            Issuer = ServerName,
            ProfileHandle = profile.ProfileHandle,
        };
        ServerSessionRepository.Save(serverAccountData);

        return new AccountData
        {
            PersonHandle = profile.PersonHandle,
        };
    }
}
