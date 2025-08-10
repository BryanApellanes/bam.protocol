using Bam.Data.Repositories;
using Bam.Protocol.Data.Common;

namespace Bam.Protocol.Data.Profile;

public class PersonData : ActorData, IPerson
{
    public virtual List<OrganizationData> Organizations { get; set; }
    
    public virtual List<GroupData> GroupDatas { get; set; }

    public string Phone { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
}