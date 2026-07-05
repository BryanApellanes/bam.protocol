using Bam.Data.Objects;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Profile;

public class EncryptedProfileRepository : IProfileRepository
{
    public EncryptedProfileRepository(ObjectDataRepository repository)
    {
        this.Repository = repository;
    }

    protected ObjectDataRepository Repository { get; }

    public ProfileData SaveProfile(ProfileData profileData)
    {
        return Repository.Create(profileData);
    }

    public ProfileData FindProfileByHandle(string handle)
    {
        return Repository.Query<ProfileData>(p => p.ProfileHandle == handle).FirstOrDefault()!;
    }

    public ProfileData FindProfileByPersonHandle(string personHandle)
    {
        return Repository.Query<ProfileData>(p => p.PersonHandle == personHandle).FirstOrDefault()!;
    }

    public PersonData SavePerson(PersonData personData)
    {
        return Repository.Create(personData);
    }

    public PersonData FindPersonByHandle(string handle)
    {
        return Repository.Query<PersonData>(p => p.Handle == handle).FirstOrDefault()!;
    }

    public DeviceData SaveDevice(DeviceData deviceData)
    {
        return Repository.Create(deviceData);
    }

    public DeviceData FindDeviceByHandle(string handle)
    {
        return Repository.Query<DeviceData>(d => d.Handle == handle).FirstOrDefault()!;
    }

    public OrganizationData SaveOrganization(OrganizationData organizationData)
    {
        return Repository.Create(organizationData);
    }

    public OrganizationData FindOrganizationByHandle(string handle)
    {
        return Repository.Query<OrganizationData>(o => o.Handle == handle).FirstOrDefault()!;
    }

    public AgentData SaveAgent(AgentData agentData)
    {
        return Repository.Create(agentData);
    }

    public AgentData FindAgentByHandle(string handle)
    {
        return Repository.Query<AgentData>(a => a.Handle == handle).FirstOrDefault()!;
    }

    public PublicKeySetData SavePublicKeySet(PublicKeySetData publicKeySetData)
    {
        return Repository.Create(publicKeySetData);
    }

    public PublicKeySetData FindPublicKeySetByHandle(string keySetHandle)
    {
        return Repository.Query<PublicKeySetData>(p => p.KeySetHandle == keySetHandle).FirstOrDefault()!;
    }

    public IEnumerable<PublicKeySetData> GetAllPublicKeySets()
    {
        return Repository.RetrieveAll<PublicKeySetData>();
    }

    public CertificateData SaveCertificate(CertificateData certificateData)
    {
        return Repository.Create(certificateData);
    }

    public CertificateData FindCertificateByHash(string hash)
    {
        return Repository.Query<CertificateData>(c => c.Hash == hash).FirstOrDefault()!;
    }

    public AgentCertificateData SaveAgentCertificate(AgentCertificateData agentCertificateData)
    {
        return Repository.Create(agentCertificateData);
    }

    public AgentCertificateData FindAgentCertificateByHandle(string agentHandle)
    {
        return Repository.Query<AgentCertificateData>(c => c.AgentHandle == agentHandle).FirstOrDefault()!;
    }

    public GroupData SaveGroup(GroupData groupData)
    {
        return Repository.Create(groupData);
    }

    public GroupData FindGroupByName(string name)
    {
        return Repository.Query<GroupData>(g => g.Name == name).FirstOrDefault()!;
    }

    public IEnumerable<GroupData> GetGroupsForPerson(string personHandle)
    {
        PersonData person = FindPersonByHandle(personHandle);
        if (person?.GroupDatas != null)
        {
            return person.GroupDatas;
        }
        return Enumerable.Empty<GroupData>();
    }

    public void AddPersonToGroup(string personHandle, string groupName)
    {
        PersonData person = FindPersonByHandle(personHandle);
        GroupData group = FindGroupByName(groupName);
        if (group == null)
        {
            group = SaveGroup(new GroupData { Name = groupName, Description = groupName });
        }

        person.GroupDatas ??= new List<GroupData>();
        if (!person.GroupDatas.Any(g => g.Name == groupName))
        {
            person.GroupDatas.Add(group);
            SavePerson(person);
        }
    }

    public void RemovePersonFromGroup(string personHandle, string groupName)
    {
        PersonData person = FindPersonByHandle(personHandle);
        if (person?.GroupDatas == null) return;

        var group = person.GroupDatas.FirstOrDefault(g => g.Name == groupName);
        if (group != null)
        {
            person.GroupDatas.Remove(group);
            SavePerson(person);
        }
    }
}
