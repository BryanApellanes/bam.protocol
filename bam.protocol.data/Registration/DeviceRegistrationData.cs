using Bam.Data.Repositories;

namespace Bam.Protocol.Profile.Registration;

public class DeviceRegistrationData : RepoData
{
    public string Handle { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DeviceTypes DeviceType { get; set; }
}
