using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Common;

public class HostAddressData: RepoData, IHostAddress
{
    
    public virtual ulong DeviceDataId { get; set; }

    public virtual DeviceData DeviceData { get; set; }
    
    public virtual ulong MachineDataId { get; set; }
    public virtual MachineData MachineData { get; set; }
        
    
    [CompositeKey]
    public string IpAddress { get; set; }
    [CompositeKey]
    public string AddressFamily { get; set; }
    [CompositeKey]
    public string HostName { get; set; }
}