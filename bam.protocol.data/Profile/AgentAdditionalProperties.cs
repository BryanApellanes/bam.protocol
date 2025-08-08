using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class AgentAdditionalProperties : KeyedAuditRepoData
{
    public string AgentHandle { get; set; }
    public string AdditionalPropertyHandle { get; set; }
}