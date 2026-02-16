using Bam.Encryption;

namespace Bam.Protocol.Server;

/// <summary>
/// Defines the cryptographic key set associated with a server session.
/// </summary>
public interface IServerSessionKeySet
{
    /// <summary>
    /// Gets or sets the session identifier.
    /// </summary>
    string SessionId { get; set; }

    /// <summary>
    /// Gets or sets the nonce value for this session.
    /// </summary>
    string Nonce { get; set; }

    /// <summary>
    /// Gets or sets the client's public key for this session.
    /// </summary>
    IPublicKey ClientPublicKey { get; set; }
}