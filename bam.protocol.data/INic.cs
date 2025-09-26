using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Newtonsoft.Json;

namespace Bam.Protocol;

//[JsonConverter(typeof(InterfaceTypeConverter<INic, NicData>))]
public interface INic
{
    ulong DeviceDataId { get; set; }

    [JsonIgnore]
    DeviceData DeviceData { get; set; }
    
    ulong MachineDataId { get; set; }
    
    [JsonIgnore]
    MachineData MachineData { get; set; }
    string AddressFamily { get; set; }
    string Address { get; set; }
    string MacAddress { get; set; }
}