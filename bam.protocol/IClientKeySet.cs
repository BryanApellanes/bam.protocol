using Bam.Encryption;

namespace Bam.Protocol.Client
{
    /// <summary>
    /// Defines the cryptographic key set for a client, including RSA, ECC, and AES keys for encrypted communication.
    /// </summary>
    public interface IClientKeySet : ICommunicationKeySet, IRsaPublicKeySource, IAesKeySource
    {
        /// <summary>
        /// Gets a value that uniquely identifies this client key set by the hash of the public key.
        /// </summary>
        string ClientKeySetHandle { get; }

        /// <summary>
        /// Gets the pem encoded RSA private key.
        /// </summary>
        IPrivateKey GetRsaPrivateKey();
        
        /// <summary>
        /// Gets the PEM encoded RSA key for the client.
        /// </summary>
        string ClientRsaKey { get; }
        /// <summary>
        /// Gets the pem encoded RSA public key.
        /// </summary>
        string ServerRsaKey { get; }
        
        /// <summary>
        /// Gets the pem encoded ECC public key.
        /// </summary>
        string ServerEccKey { get; }
        
        /// <summary>
        /// Gets the PEM encoded ECC key for the client.
        /// </summary>
        string ClientEccKey { get; }

        /// <summary>
        /// Gets or sets the machine name associated with this key set.
        /// </summary>
        string MachineName { get; set; }

        /// <summary>
        /// Gets or sets the client hostname.
        /// </summary>
        new string ClientHostName { get; set; }

        /// <summary>
        /// Gets or sets the server hostname.
        /// </summary>
        new string ServerHostName { get; set; }

        /// <summary>
        /// Encrypts the specified value using asymmetric (RSA) encryption.
        /// </summary>
        /// <param name="value">The plain-text value to encrypt.</param>
        /// <returns>The encrypted value.</returns>
        string EncryptAsymmetric(string value);

        /// <summary>
        /// Encrypts the specified value using symmetric (AES) encryption.
        /// </summary>
        /// <param name="value">The plain-text value to encrypt.</param>
        /// <returns>The encrypted value.</returns>
        string EncryptSymmetric(string value);

        /// <summary>
        /// Decrypts the specified base64-encoded value using symmetric (AES) decryption.
        /// </summary>
        /// <param name="base64EncodedValue">The base64-encoded cipher text.</param>
        /// <returns>The decrypted plain-text value.</returns>
        string DecryptSymmetric(string base64EncodedValue);
    }
}
