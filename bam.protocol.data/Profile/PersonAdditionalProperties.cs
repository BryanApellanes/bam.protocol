using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class PersonAdditionalProperties : KeyedAuditRepoData
{
    public string PersonHandle { get; set; }
    public string AdditionalPropertyHandle { get; set; }
}