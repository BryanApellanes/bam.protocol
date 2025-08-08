using Bam.Encryption;

namespace Bam.Protocol.Client
{
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
        
        string ClientRsaKey { get; }
        /// <summary>
        /// Gets the pem encoded RSA public key.
        /// </summary>
        string ServerRsaKey { get; }
        
        /// <summary>
        /// Gets the pem encoded ECC public key.
        /// </summary>
        string ServerEccKey { get; }
        
        string ClientEccKey { get; }
        
        string MachineName { get; set; }


        string ClientHostName { get; set; }


        string ServerHostName { get; set; }
        string EncryptAsymmetric(string value);
        string EncryptSymmetric(string value);

        string DecryptSymmetric(string base64EncodedValue);
    }
}
