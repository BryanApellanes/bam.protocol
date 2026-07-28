using Bam.Protocol.Data;

namespace Bam.Protocol.Profile;

/// <summary>
/// An <see cref="IAdminPublicKeySource"/> that holds a fixed, configured break-glass admin
/// public RSA key (the PEM exported from the offline YubiKey / PIV applet).  Construct with the
/// per-deployment admin public key, or with null to leave revocation unconfigured (fail-closed).
/// </summary>
public class StaticAdminPublicKeySource : IAdminPublicKeySource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StaticAdminPublicKeySource"/> class.
    /// </summary>
    /// <param name="adminPublicRsaKey">The PEM-encoded break-glass admin public RSA key, or null to leave revocation unconfigured (fail-closed).</param>
    public StaticAdminPublicKeySource(string? adminPublicRsaKey)
    {
        this.AdminPublicRsaKey = adminPublicRsaKey;
    }

    /// <inheritdoc />
    public string? AdminPublicRsaKey { get; }
}
