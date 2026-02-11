using Bam;
using Bam.Encryption;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Data.Profile.Dao.Repository;
using Bam.Protocol.Profile;
using Bam.Protocol.Profile.Registration;

namespace Bam.Protocol.Data;

public class ProfileManager : IProfileManager
{
    public ProfileManager(ProfileSchemaRepository repository)
    {
        this.Repository = repository;
        repository.Initialize();
    }

    protected ProfileSchemaRepository Repository { get; }

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

        personData = Repository.Save(personData);

        ProfileData profileData = new ProfileData
        {
            PersonHandle = personData.Handle,
            Name = personData.Name,
        };

        profileData = Repository.Save(profileData);

        return profileData;
    }

    public IProfile CreateProfile()
    {
        ProfileData profileData = new ProfileData();
        profileData = Repository.Save(profileData);
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
        ProfileData result = Repository.OneProfileDataWhere(c => c.ProfileHandle == handle);
        if (result == null)
        {
            result = Repository.OneProfileDataWhere(c => c.PersonHandle == handle);
        }

        return result;
    }

    public IProfile FindProfileByPublicKey(IPublicKey publicKey)
    {
        return FindProfileByPublicKey(publicKey.Pem.Sha256());
    }

    public IProfile FindProfileByPublicKey(string publicKeyPemSha)
    {
        IEnumerable<PublicKeySetData> keySets = Repository.PublicKeySetDatasWhere(c => c.Id > 0);
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
}
