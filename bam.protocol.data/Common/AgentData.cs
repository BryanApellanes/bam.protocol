using System.Text.Json.Serialization;
using Bam.Data.Repositories;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Server;

namespace Bam.Protocol.Data.Common;

public class AgentData : KeyedAuditRepoData, IActor, IAgent
{
    public virtual ulong ActorDataId { get; set; }
    
    [JsonIgnore]
    public virtual ActorData ActorData { get; set; }
    
    public virtual ulong DeviceDataId { get; set; }
    
    [JsonIgnore]
    public virtual DeviceData DeviceData { get; set; }
    
    public virtual ulong ProcessDescriptorDataId { get; set; }
    
    [JsonIgnore]
    public virtual ProcessDescriptorData ProcessDescriptorData { get; set; }
    
    public string Handle { get; }
    public string Name { get; }
}