using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Newtonsoft.Json;

namespace Bam.Protocol;

[JsonConverter(typeof(InterfaceTypeConverter<IMachine, MachineData>))]
public interface IMachine
{
    List<HostAddressData> HostAddresses { get; set; }
    string Name { get; set; }
    string DnsName { get; set; }
    List<NicData> NetworkInterfaces { get; set; }
}