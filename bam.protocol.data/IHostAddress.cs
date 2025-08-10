using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Data.Profile;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509.Qualified;

namespace Bam.Protocol;

[JsonConverter(typeof(InterfaceTypeConverter<IHostAddress, HostAddressData>))]
public interface IHostAddress
{
    ulong DeviceDataId {get;set;}

    [JsonIgnore]
    DeviceData DeviceData { get; set; }
    
    ulong MachineDataId { get; set; }
    
    [JsonIgnore]
    MachineData MachineData { get; set; }
    string IpAddress { get; set; }
    string AddressFamily { get; set; }
    string HostName { get; set; }
}