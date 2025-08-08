using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class PersonData : ActorData, IPerson
{
    
    public virtual List<IOrganization> Organizations { get; set; }
    
    public virtual List<IGroup> GroupDatas { get; set; }

    public string Phone { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
}