using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class ActorData : KeyedAuditRepoData, IActor
{
    public string Name { get; set; }
    
    [CompositeKey]
    public string Handle { get; set; }
}