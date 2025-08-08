using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Server;

public class ServerSession : CompositeKeyAuditRepoData
{
    public ServerSession()
    {
        KeyValues = new List<ServerSessionKeyValuePair>();
    }

    [CompositeKey]
    public string SessionId { get; set; }

    public virtual List<ServerSessionKeyValuePair> KeyValues { get; set; }
}
