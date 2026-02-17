using System.Text.Json.Serialization;
using Bam.Data.Repositories;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Server;

namespace Bam.Protocol.Data.Common;

public class AgentData : RepoData, IActor, IAgent, IHasHandle
{
    public virtual ulong ActorDataId { get; set; }
    
    [JsonIgnore]
    public virtual ActorData ActorData { get; set; } = null!;

    public virtual ulong DeviceDataId { get; set; }

    [JsonIgnore]
    public virtual DeviceData DeviceData { get; set; } = null!;

    public virtual ulong ProcessDescriptorDataId { get; set; }

    [JsonIgnore]
    public virtual ProcessDescriptorData ProcessDescriptorData { get; set; } = null!;

    public string Handle { get; set; } = null!;
    public string Name { get; set; } = null!;
}