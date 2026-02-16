using Bam.Data.Repositories;

namespace Bam.Protocol.Profile.Registration;

public class OrganizationRegistrationData : RepoData
{
    public string Handle { get; set; }
    public string Name { get; set; }
}
