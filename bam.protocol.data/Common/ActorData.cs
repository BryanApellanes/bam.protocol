using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Common;

public class ActorData : KeyedAuditRepoData, IActor
{
    public string Name { get; set; }
    
    [CompositeKey]
    public string Handle { get; set; }
}