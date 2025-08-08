using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class OrganizationAdditionalProperties : KeyedAuditRepoData
{
    public string OrganizationHandle { get; set; }
    public string AdditionalPropertyHandle { get; set; }
}