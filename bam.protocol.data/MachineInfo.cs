using System.Net.NetworkInformation;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;

namespace Bam.Protocol
{
    public class MachineInfo : IMachine
    {
        public List<HostAddressData> HostAddresses { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string DnsName { get; set; } = null!;
        public List<NicData> NetworkInterfaces { get; set; } = null!;
    }
}