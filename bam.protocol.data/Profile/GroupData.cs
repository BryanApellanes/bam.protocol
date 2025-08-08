using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;



public class GroupData : KeyedAuditRepoData, IGroup
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public virtual List<IPerson> Persons { get; set; }
}