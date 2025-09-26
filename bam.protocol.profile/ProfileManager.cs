using Bam.Encryption;
using Bam.Protocol.Profile;
using Bam.Protocol.Profile.Registration;

namespace Bam.Protocol.Data;

public class ProfileManager : IProfileManager
{
    public IProfile RegisterPersonProfile(PersonRegistrationData personRegistrationData)
    {
        throw new NotImplementedException();
    }

    public IProfile GetProfile(string handle, bool createIfNotExists = false)
    {
        throw new NotImplementedException();
    }

    public IProfile CreateProfile()
    {
        throw new NotImplementedException();
    }

    public IProfile FindProfileByHandle(string handle)
    {
        throw new NotImplementedException();
    }

    public IProfile FindProfileByPublicKey(string publicKeyPemSha)
    {
        throw new NotImplementedException();
    }

    public IProfile FindProfileByPublicKey(IPublicKey publicKey)
    {
        throw new NotImplementedException();
    }
}