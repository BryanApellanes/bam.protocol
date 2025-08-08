using System.Net.NetworkInformation;
using Bam.Protocol.Data;

namespace Bam.Protocol
{
    public class MachineInfo : IMachine
    {
        public List<IHostAddress> HostAddresses { get; set; }
        public string Name { get; set; }
        public string DnsName { get; set; }
        public List<INicData> NetworkInterfaces { get; set; }
    }
}