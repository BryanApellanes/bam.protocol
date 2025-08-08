using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class HostAddressData: KeyedAuditRepoData, IHostAddress
{
    public virtual ulong MachineId { get; set; }
    public virtual IMachine Machine { get; set; }
        
    [CompositeKey]
    public string IpAddress { get; set; }
    [CompositeKey]
    public string AddressFamily { get; set; }
    [CompositeKey]
    public string HostName { get; set; }
}