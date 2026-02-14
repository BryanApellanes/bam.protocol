using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Data;

public interface IProfileRepository
{
    ProfileData SaveProfile(ProfileData profileData);
    ProfileData FindProfileByHandle(string handle);
    ProfileData FindProfileByPersonHandle(string personHandle);

    PersonData SavePerson(PersonData personData);
    PersonData FindPersonByHandle(string handle);

    PublicKeySetData SavePublicKeySet(PublicKeySetData publicKeySetData);
    PublicKeySetData FindPublicKeySetByHandle(string keySetHandle);
    IEnumerable<PublicKeySetData> GetAllPublicKeySets();

    CertificateData SaveCertificate(CertificateData certificateData);
    CertificateData FindCertificateByHash(string hash);

    AgentCertificateData SaveAgentCertificate(AgentCertificateData agentCertificateData);
    AgentCertificateData FindAgentCertificateByHandle(string agentHandle);
}
