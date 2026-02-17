using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class ProfileAdditionalProperties : KeyedAuditRepoData
{
    public string ProfileHandle { get; set; } = null!;
    public string AdditionalPropertyHandle { get; set; } = null!;
}