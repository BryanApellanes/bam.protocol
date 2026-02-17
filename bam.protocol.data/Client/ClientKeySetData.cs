using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Client;

public class ClientKeySetData : KeyedAuditRepoData
{
    /// <summary>
    /// Gets the pem encoded RSA public key of the client.
    /// </summary>
    public string ClientRsaKey { get; } = null!;
    
    /// <summary>
    /// Gets the pem encoded RSA public key of the server.
    /// </summary>
    public string ServerRsaKey { get; } = null!;
    
    /// <summary>
    /// Gets the pem encoded ECC public key.
    /// </summary>
    public string ServerEccKey { get; } = null!;
        
    public string ClientEccKey { get; } = null!;
    
    /// <summary>
    /// Gets or sets the name of the machine that instantiated this keyset.
    /// </summary>
    [CompositeKey]
    public string MachineName { get; set; } = null!;


    /// <summary>
    /// Gets or sets the hostname of the client as resolved by DNS at the time this keyset was instantiated.
    /// </summary>
    [CompositeKey]
    public string ClientHostName { get; set; } = null!;


    /// <summary>
    /// Gets or sets the server hostname.
    /// </summary>
    [CompositeKey]
    public string ServerHostName { get; set; } = null!;
}