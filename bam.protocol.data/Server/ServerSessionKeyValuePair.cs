using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Server;

public class ServerSessionKeyValuePair : CompositeKeyAuditRepoData
{
    public virtual ulong ServerSessionId { get; set; }
    public virtual ServerSession ServerSession { get; set; } = null!;

    [CompositeKey]
    public new string Key { get; set; } = null!;

    public string Value { get; set; } = null!;
}