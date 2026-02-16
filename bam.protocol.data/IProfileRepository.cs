using Bam.Protocol.Data.Common;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Data;

public interface IProfileRepository
{
    ProfileData SaveProfile(ProfileData profileData);
    ProfileData FindProfileByHandle(string handle);
    ProfileData FindProfileByPersonHandle(string personHandle);

    PersonData SavePerson(PersonData personData);
    PersonData FindPersonByHandle(string handle);

    DeviceData SaveDevice(DeviceData deviceData);
    DeviceData FindDeviceByHandle(string handle);

    OrganizationData SaveOrganization(OrganizationData organizationData);
    OrganizationData FindOrganizationByHandle(string handle);

    AgentData SaveAgent(AgentData agentData);
    AgentData FindAgentByHandle(string handle);

    PublicKeySetData SavePublicKeySet(PublicKeySetData publicKeySetData);
    PublicKeySetData FindPublicKeySetByHandle(string keySetHandle);
    IEnumerable<PublicKeySetData> GetAllPublicKeySets();

    CertificateData SaveCertificate(CertificateData certificateData);
    CertificateData FindCertificateByHash(string hash);

    AgentCertificateData SaveAgentCertificate(AgentCertificateData agentCertificateData);
    AgentCertificateData FindAgentCertificateByHandle(string agentHandle);
}
