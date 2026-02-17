using Bam.Data.Repositories;
using Bam.Protocol.Data.Common;

namespace Bam.Protocol.Data.Profile;

public class PersonData : ActorData, IPerson
{
    public virtual List<OrganizationData> Organizations { get; set; } = null!;

    public virtual List<GroupData> GroupDatas { get; set; } = null!;

    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string MiddleName { get; set; } = null!;
}