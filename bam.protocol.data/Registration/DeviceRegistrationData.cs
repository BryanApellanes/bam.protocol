using Bam.Data.Repositories;

namespace Bam.Protocol.Profile.Registration;

public abstract class DeviceRegistrationData : RepoData
{
    public DeviceRegistrationData()
    {
    }

    protected abstract Task InitializeAsync();
    
    // TODO: set this using OSInfo.Current for Windows, Mac and Linux
    // For android or iOS use specific class definitions from
    // within those platform client apps
    public DeviceTypes DeviceType { get; set; }
    
    
    
}