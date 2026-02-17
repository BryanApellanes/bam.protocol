using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Server;

public class AccountData : KeyedAuditRepoData
{
    public string PersonHandle { get; set; } = null!;
}