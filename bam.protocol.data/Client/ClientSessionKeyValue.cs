using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Client;

public class ClientSessionKeyValue : KeyedAuditRepoData
{
    public virtual ulong ClientSessionDataId { get; set; }
    public virtual ClientSessionData ClientSessionData { get; set; }
    
    [CompositeKey]
    public string Key { get; set; }
    
    public string Value { get; set; }
}