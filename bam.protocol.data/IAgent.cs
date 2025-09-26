using Bam.Protocol.Data;
using Bam.Protocol.Data.Common;
using Bam.Protocol.Data.Profile;
using Newtonsoft.Json;

namespace Bam.Protocol;

//[JsonConverter(typeof(InterfaceTypeConverter<IAgent, AgentData>))]
public interface IAgent : IActor
{
    ActorData ActorData { get; set; }
    DeviceData DeviceData { get; set; }
}