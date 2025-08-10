using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Newtonsoft.Json;

namespace Bam.Protocol;


[JsonConverter(typeof(InterfaceTypeConverter<IPerson, PersonData>))]
public interface IPerson: IActor
{
    string Phone { get; set; }
    string Email { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    string MiddleName { get; set; }
}