using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Server;

public class InboxData : KeyedAuditRepoData
{
    public ulong AccountDataId { get; init; }
    public virtual AccountData AccountData { get; set; }

}