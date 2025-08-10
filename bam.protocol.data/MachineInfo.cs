using System.Net.NetworkInformation;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;

namespace Bam.Protocol
{
    public class MachineInfo : IMachine
    {
        public List<HostAddressData> HostAddresses { get; set; }
        public string Name { get; set; }
        public string DnsName { get; set; }
        public List<NicData> NetworkInterfaces { get; set; }
    }
}