using Bam.Data.Repositories;
using Newtonsoft.Json;

namespace Bam.Protocol.Data.Common;

public class HostAddressData : RepoData, IHostAddress
{
    
    public virtual ulong DeviceDataId { get; set; }

    [JsonIgnore]
    public virtual DeviceData DeviceData { get; set; } = null!;

    public virtual ulong MachineDataId { get; set; }

    [JsonIgnore]
    public virtual MachineData MachineData { get; set; } = null!;

    [CompositeKey]
    public string IpAddress { get; set; } = null!;

    [CompositeKey]
    public string AddressFamily { get; set; } = null!;

    [CompositeKey]
    public string HostName { get; set; } = null!;

}