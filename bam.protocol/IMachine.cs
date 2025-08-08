using Bam.Protocol.Data;

namespace Bam.Protocol;

public interface IMachine
{
    List<IHostAddress> HostAddresses { get; set; }
    string Name { get; set; }
    string DnsName { get; set; }
    List<INicData> NetworkInterfaces { get; set; }
}