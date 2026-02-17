using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class GroupAdditionalProperties : KeyedAuditRepoData
{
    public string GroupHandle { get; set; } = null!;
    public string AdditionalPropertyHandle { get; set; } = null!;
}