namespace Bam.Protocol.Server;

/// <summary>
/// Defines a provider that generates cryptographic nonce values.
/// </summary>
public interface INonceProvider
{
    /// <summary>
    /// Generates a new cryptographic nonce string.
    /// </summary>
    /// <returns>A random nonce string.</returns>
    string GetNonce();
}