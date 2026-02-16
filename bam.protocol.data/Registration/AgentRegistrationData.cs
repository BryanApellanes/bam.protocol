using Bam.Data.Repositories;

namespace Bam.Protocol.Profile.Registration;

public class AgentRegistrationData : RepoData
{
    public string Handle { get; set; }
    public string Name { get; set; }
    public string PersonHandle { get; set; }
    public string DeviceHandle { get; set; }
}
