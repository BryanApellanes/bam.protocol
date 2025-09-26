using Bam.Data.Repositories;
using Newtonsoft.Json;

namespace Bam.Protocol.Data.Common;

public class HostAddressData : RepoData, IHostAddress
{
    
    public virtual ulong DeviceDataId { get; set; }

    [JsonIgnore]
    public virtual DeviceData DeviceData { get; set; }
    
    public virtual ulong MachineDataId { get; set; }
    
    [JsonIgnore]
    public virtual MachineData MachineData { get; set; }

    [CompositeKey]
    public string IpAddress { get; set; }
    
    [CompositeKey]
    public string AddressFamily { get; set; }
    
    [CompositeKey]
    public string HostName { get; set; }

}