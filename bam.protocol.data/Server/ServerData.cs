using Bam.Data.Repositories;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Data.Server;

public class ServerData : MachineData
{
    [CompositeKey]
    public string Handle { get; set; }
}