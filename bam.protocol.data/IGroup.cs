using System.Text.Json.Serialization;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol;

[JsonConverter(typeof(InterfaceTypeConverter<IGroup, GroupData>))]
public interface IGroup
{
    string Name { get; set; }
    string Description { get; set; }
    List<PersonData> PersonDatas { get; set; }
}