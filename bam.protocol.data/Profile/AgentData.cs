using Bam.Protocol.Data.Profile;
using Bam.Protocol.Server;

namespace Bam.Protocol.Data;

public class AgentData : IActor, IAgent
{
    public virtual ulong PersonId { get; set; }
    public virtual IPerson Person { get; set; }
    
    public virtual ulong DeviceId { get; set; }
    public virtual IDevice Device { get; set; }
    
    public virtual ulong ProcessDecsriptorId { get; set; }
    public virtual IProcessDescriptor ProcessDescriptor { get; set; }
    
    public string Handle { get; }
    public string Name { get; }
}