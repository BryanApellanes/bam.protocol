using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class GroupAdditionalProperties : KeyedAuditRepoData
{
    public string GroupHandle { get; set; }
    public string AdditionalPropertyHandle { get; set; }
}