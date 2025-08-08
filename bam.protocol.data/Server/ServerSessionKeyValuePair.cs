using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Server;

public class ServerSessionKeyValuePair : CompositeKeyAuditRepoData
{
    public virtual ulong ServerSessionId { get; set; }
    public virtual ServerSession ServerSession { get; set; }
    
    [CompositeKey]
    public string Key { get; set; }
    
    public string Value { get; set; }
}