using Bam.Data.Repositories;

namespace Bam.Protocol.Profile.Registration;

public class AgentRegistrationData : RepoData
{
    public string Handle { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string PersonHandle { get; set; } = null!;
    public string DeviceHandle { get; set; } = null!;
}
