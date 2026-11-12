using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Client;

public class ClientSessionKeyValue : KeyedAuditRepoData
{
    public virtual ulong ClientSessionDataId { get; set; }
    public virtual ClientSessionData? ClientSessionData { get; set; }

    [CompositeKey]
    public new string Key { get; set; } = null!;

    public string Value { get; set; } = null!;
}