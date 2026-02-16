using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile.Registration;

namespace Bam.Protocol.Profile;

public interface IProfileManager
{
    IProfile RegisterPersonProfile(PersonRegistrationData personRegistrationData);
    IProfile RegisterDeviceProfile(DeviceRegistrationData deviceRegistrationData, string personHandle);
    DeviceData FindDeviceByHandle(string handle);

    OrganizationData RegisterOrganization(OrganizationRegistrationData organizationRegistrationData);
    OrganizationData FindOrganizationByHandle(string handle);

    AgentData RegisterAgent(AgentRegistrationData agentRegistrationData);
    AgentData FindAgentByHandle(string handle);

    IProfile GetProfile(string handle, bool createIfNotExists = false);
    IProfile CreateProfile();
    IProfile FindProfileByHandle(string handle);
    IProfile FindProfileByPublicKey(string publicKeyPemSha);
    IProfile FindProfileByPublicKey(IPublicKey publicKey);
}