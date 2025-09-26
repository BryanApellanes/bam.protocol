using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Newtonsoft.Json;

namespace Bam.Protocol;

public interface IMachine
{
    [JsonIgnore]
    List<HostAddressData> HostAddresses { get; set; }
    string Name { get; set; }
    string DnsName { get; set; }
    [JsonIgnore]
    List<NicData> NetworkInterfaces { get; set; }
}