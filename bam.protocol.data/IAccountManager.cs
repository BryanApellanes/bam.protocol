using Bam.Protocol.Data.Profile;
using Bam.Protocol.Data.Server;
using Bam.Protocol.Profile.Registration;

namespace Bam.Protocol;

public interface IAccountManager
{
    AccountData RegisterAccount(PersonRegistrationData data);
    
}