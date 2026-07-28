namespace Bam.Protocol.Data;

/// <summary>
/// Supplies the break-glass admin public RSA key that authorizes key-set revocation.  Only the
/// public key is ever held by the framework; the private key lives offline (a YubiKey / PIV
/// applet) and signs revocation payloads out of band.  A consumer provisions this per deployment.
/// When the key is absent or empty, revocation cannot be authorized and fails closed.
/// </summary>
public interface IAdminPublicKeySource
{
    /// <summary>
    /// Gets the PEM-encoded break-glass admin public RSA key, or null/empty when none is
    /// configured (in which case no revocation can be authorized).
    /// </summary>
    string? AdminPublicRsaKey { get; }
}
