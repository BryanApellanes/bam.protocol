using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Client;

public class ClientKeySetData : KeyedAuditRepoData
{
    /// <summary>
    /// Gets the pem encoded RSA public key of the client.
    /// </summary>
    public string ClientRsaKey { get; }
    
    /// <summary>
    /// Gets the pem encoded RSA public key of the server.
    /// </summary>
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


    /// <summary>
    /// Gets or sets the hostname of the client as resolved by DNS at the time this keyset was instantiated.
    /// </summary>
    [CompositeKey]
    public string ClientHostName { get; set; }


    /// <summary>
    /// Gets or sets the server hostname.
    /// </summary>
    [CompositeKey]
    public string ServerHostName { get; set; }
}