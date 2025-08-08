namespace Bam.Protocol.Profile;

public interface IProfileManager
{
    
    Profile GetProfile(string handle, bool createIfNotExists = false);
    Profile CreateProfile();
}