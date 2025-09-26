using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Profile.Registration;

namespace Bam.Protocol.Profile;

public interface IProfileManager
{
    IProfile RegisterPersonProfile(PersonRegistrationData personRegistrationData);
    IProfile GetProfile(string handle, bool createIfNotExists = false);
    IProfile CreateProfile();
    IProfile FindProfileByHandle(string handle);
    IProfile FindProfileByPublicKey(string publicKeyPemSha);
    IProfile FindProfileByPublicKey(IPublicKey publicKey);
}