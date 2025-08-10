using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Profile.Registration;

public interface IRegistrationManager
{
    ProfileAccountData RegisterProfile(ProfileRegistrationInfo info);
}