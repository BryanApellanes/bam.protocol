using Bam.Encryption;
using Bam.Protocol.Server;

namespace Bam.Protocol;

/// <summary>
/// Represents a request to start a new session, containing the client's ECC public key.
/// </summary>
public class StartSessionRequest : BamRequest
{
    /// <summary>
    /// Gets or sets the client's ECC public key used for key exchange.
    /// </summary>
    public EccPublicKey ClientPublicKey { get; set; } = null!;
}