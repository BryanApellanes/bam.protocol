using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Server;

public class ServerAccountData : CompositeKeyAuditRepoData
{
    /// <summary>
    /// Gets or sets the issuer of this server account, this is typically
    /// the domain name of the issuing server.
    /// </summary>
    public string Issuer { get; set; }
    
    /// <summary>
    /// Gets or sets the ProfileAccount handle.
    /// </summary>
    public string ProfileHandle { get; set; }
}