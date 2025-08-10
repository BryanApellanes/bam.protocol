using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Newtonsoft.Json;

namespace Bam.Protocol;


[JsonConverter(typeof(InterfaceTypeConverter<IOrganization, OrganizationData>))]
public interface IOrganization
{
    string Handle { get; set; }
    string Name { get; set; }
    List<PersonData> People { get; set; }
}