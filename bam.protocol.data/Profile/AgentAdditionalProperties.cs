using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class AgentAdditionalProperties : KeyedAuditRepoData
{
    public string AgentHandle { get; set; } = null!;
    public string AdditionalPropertyHandle { get; set; } = null!;
}