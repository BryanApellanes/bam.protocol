namespace Bam.Protocol;

/// <summary>
/// Defines a set of public cryptographic keys identified by a handle.
/// </summary>
public interface IKeySet
{
    /// <summary>
    /// Gets or sets the unique handle for this key set.
    /// </summary>
    string KeySetHandle { get; set; }

    /// <summary>
    /// Gets or sets the PEM-encoded public RSA key.
    /// </summary>
    string PublicRsaKey { get; set; }

    /// <summary>
    /// Gets or sets the PEM-encoded public ECC key.
    /// </summary>
    string PublicEccKey { get; set; }
}