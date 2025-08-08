using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Client;

public class ClientKeySetData : KeyedAuditRepoData
{
    /// <summary>
    /// Gets the pem encoded RSA public key.
    /// </summary>
    public string ClientRsaKey { get; }
    public string ServerRsaKey { get; }
    
    /// <summary>
    /// Gets the pem encoded ECC public key.
    /// </summary>
    public string ServerEccKey { get; }
        
    public string ClientEccKey { get; }
    
    /// <summary>
    /// Gets or sets the name of the machine that instantiated this keyset.
    /// </summary>
    [CompositeKey]
    public string MachineName { get; set; }


    [CompositeKey]
    public string ClientHostName { get; set; }


    [CompositeKey]
    public string ServerHostName { get; set; }
}