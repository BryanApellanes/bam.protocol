using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Profile.Registration;

public interface IRegistrationManager
{
    ProfileData RegisterProfile(ProfileRegistrationInfo info);
}