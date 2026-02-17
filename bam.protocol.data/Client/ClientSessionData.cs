using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Client;

public class ClientSessionData : KeyedAuditRepoData
{
    public ClientSessionData()
    {
        KeyValues = new List<ClientSessionKeyValue>();
    }

    [CompositeKey]
    public string SessionId { get; set; } = null!;

    public virtual List<ClientSessionKeyValue> KeyValues { get; set; }
}