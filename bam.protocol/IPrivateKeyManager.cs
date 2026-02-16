using Bam.Encryption;
using Bam.Storage.Encryption;

namespace Bam.Protocol;

/// <summary>
/// Manages generation and retrieval of private RSA and ECC keys.
/// </summary>
public interface IPrivateKeyManager
{
    /// <summary>
    /// Generates a new RSA private key and returns the corresponding public key.
    /// </summary>
    /// <returns>The public key corresponding to the generated private key.</returns>
    IPublicKey GeneratePrivateRsaKey();

    /// <summary>
    /// Retrieves the RSA private key associated with the specified public key.
    /// </summary>
    /// <param name="publicKey">The public key whose private key to retrieve.</param>
    /// <returns>The corresponding RSA private key.</returns>
    IPrivateKey GetPrivateRsaKey(IPublicKey publicKey);

    /// <summary>
    /// Generates a new ECC private key and returns the corresponding public key.
    /// </summary>
    /// <returns>The public key corresponding to the generated private key.</returns>
    IPublicKey GeneratePrivateEccKey();

    /// <summary>
    /// Retrieves the ECC private key associated with the specified public key.
    /// </summary>
    /// <param name="publicKey">The public key whose private key to retrieve.</param>
    /// <returns>The corresponding ECC private key.</returns>
    IPrivateKey GetPrivateEccKey(IPublicKey publicKey);

}