using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class OrganizationData : KeyedAuditRepoData, IOrganization
{
    [CompositeKey]
    public string Handle { get; set; } = null!;

    public string Name { get; set; } = null!;
    public virtual List<PersonData> People { get; set; } = null!;
}