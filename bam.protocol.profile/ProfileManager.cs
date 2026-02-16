using System.Runtime.InteropServices;
using Bam;
using Bam.Encryption;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile;
using Bam.Protocol.Profile.Registration;

namespace Bam.Protocol.Data;

public class ProfileManager : IProfileManager
{
    public ProfileManager(IProfileRepository repository)
    {
        this.Repository = repository;
    }

    protected IProfileRepository Repository { get; }

    public IProfile RegisterPersonProfile(PersonRegistrationData personRegistrationData)
    {
        PersonData personData = new PersonData
        {
            Handle = string.IsNullOrEmpty(personRegistrationData.Handle)
                ? 6.SecureAlphaNumericCharacters()
                : personRegistrationData.Handle,
            FirstName = personRegistrationData.FirstName,
            LastName = personRegistrationData.LastName,
            MiddleName = personRegistrationData.MiddleName,
            Phone = personRegistrationData.Phone,
            Email = personRegistrationData.Email,
            Name = string.IsNullOrEmpty(personRegistrationData.Name)
                ? $"{personRegistrationData.FirstName} {personRegistrationData.LastName}"
                : personRegistrationData.Name,
        };

        personData = Repository.SavePerson(personData);

        ProfileData profileData = new ProfileData
        {
            PersonHandle = personData.Handle,
            Name = personData.Name,
        };

        profileData = Repository.SaveProfile(profileData);

        return profileData;
    }

    public IProfile RegisterDeviceProfile(DeviceRegistrationData deviceRegistrationData, string personHandle)
    {
        // Use DeviceData(true) to force initialization first (DNS, NIC enumeration),
        // then override with registration data. This prevents the lazy-init Handle
        // getter from overwriting explicitly set values during serialization.
        DeviceData deviceData = new DeviceData(true);
        deviceData.Handle = string.IsNullOrEmpty(deviceRegistrationData.Handle)
            ? Guid.NewGuid().ToString()
            : deviceRegistrationData.Handle;
        deviceData.Name = string.IsNullOrEmpty(deviceRegistrationData.Name)
            ? Environment.MachineName
            : deviceRegistrationData.Name;
        deviceData.DeviceType = deviceRegistrationData.DeviceType == DeviceTypes.Invalid
            ? DetectDeviceType()
            : deviceRegistrationData.DeviceType;

        deviceData = Repository.SaveDevice(deviceData);

        ProfileData profileData = Repository.FindProfileByPersonHandle(personHandle);
        if (profileData != null)
        {
            profileData.DeviceHandle = deviceData.Handle;
            profileData = Repository.SaveProfile(profileData);
        }

        return profileData;
    }

    public DeviceData FindDeviceByHandle(string handle)
    {
        return Repository.FindDeviceByHandle(handle);
    }

    public OrganizationData RegisterOrganization(OrganizationRegistrationData organizationRegistrationData)
    {
        OrganizationData organizationData = new OrganizationData
        {
            Handle = string.IsNullOrEmpty(organizationRegistrationData.Handle)
                ? 6.SecureAlphaNumericCharacters()
                : organizationRegistrationData.Handle,
            Name = organizationRegistrationData.Name,
        };

        return Repository.SaveOrganization(organizationData);
    }

    public OrganizationData FindOrganizationByHandle(string handle)
    {
        return Repository.FindOrganizationByHandle(handle);
    }

    public AgentData RegisterAgent(AgentRegistrationData agentRegistrationData)
    {
        AgentData agentData = new AgentData
        {
            Handle = string.IsNullOrEmpty(agentRegistrationData.Handle)
                ? 6.SecureAlphaNumericCharacters()
                : agentRegistrationData.Handle,
            Name = agentRegistrationData.Name,
        };

        PersonData person = Repository.FindPersonByHandle(agentRegistrationData.PersonHandle);
        if (person != null)
        {
            agentData.ActorData = new ActorData
            {
                Handle = person.Handle,
                Name = person.Name,
            };
        }

        DeviceData device = Repository.FindDeviceByHandle(agentRegistrationData.DeviceHandle);
        if (device != null)
        {
            agentData.DeviceData = device;
        }

        return Repository.SaveAgent(agentData);
    }

    public AgentData FindAgentByHandle(string handle)
    {
        return Repository.FindAgentByHandle(handle);
    }

    public IProfile CreateProfile()
    {
        ProfileData profileData = new ProfileData();
        profileData = Repository.SaveProfile(profileData);
        return profileData;
    }

    public IProfile GetProfile(string handle, bool createIfNotExists = false)
    {
        IProfile result = FindProfileByHandle(handle);
        if (result == null && createIfNotExists)
        {
            result = CreateProfile();
        }

        return result;
    }

    public IProfile FindProfileByHandle(string handle)
    {
        ProfileData result = Repository.FindProfileByHandle(handle);
        if (result == null)
        {
            result = Repository.FindProfileByPersonHandle(handle);
        }

        return result;
    }

    public IProfile FindProfileByPublicKey(IPublicKey publicKey)
    {
        return FindProfileByPublicKey(publicKey.Pem.Sha256());
    }

    public IProfile FindProfileByPublicKey(string publicKeyPemSha)
    {
        IEnumerable<PublicKeySetData> keySets = Repository.GetAllPublicKeySets();
        foreach (PublicKeySetData keySet in keySets)
        {
            if ((!string.IsNullOrEmpty(keySet.PublicRsaKey) && keySet.PublicRsaKey.Sha256() == publicKeyPemSha) ||
                (!string.IsNullOrEmpty(keySet.PublicEccKey) && keySet.PublicEccKey.Sha256() == publicKeyPemSha))
            {
                return FindProfileByHandle(keySet.KeySetHandle);
            }
        }

        return null;
    }

    private static DeviceTypes DetectDeviceType()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return DeviceTypes.DesktopWindows;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return DeviceTypes.DesktopMac;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return DeviceTypes.DesktopLinux;
        return DeviceTypes.Invalid;
    }
}
