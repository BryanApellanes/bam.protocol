using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;



public class GroupData : KeyedAuditRepoData, IGroup
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public virtual List<PersonData> PersonDatas { get; set; } = null!;
}