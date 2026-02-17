using Bam.Encryption;

namespace Bam.Server
{
    /// <summary>
    /// Abstract base class for encrypted HTTP responses that wrap the response body in a cipher.
    /// </summary>
    public abstract class EncryptedHttpResponse : HttpResponse
    {

        ContentCipher _conentCipher = null!;
        /// <summary>
        /// Gets or sets the content cipher. Setting this also updates the Content and ContentType properties.
        /// </summary>
        public ContentCipher ContentCipher
        {
            get {  return _conentCipher; }
            set
            {
                _conentCipher = value;
                Content = _conentCipher;
                ContentType = _conentCipher.ContentType;
            }
        }

        /// <summary>
        /// Creates an encrypted HTTP response for the specified data using the given encryption scheme.
        /// </summary>
        /// <typeparam name="T">The type of data to encrypt.</typeparam>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="clientSessionInfo">The client key source for encryption.</param>
        /// <param name="encryptionScheme">The encryption scheme to use.</param>
        /// <returns>An encrypted HTTP response containing the encrypted data.</returns>
        public static EncryptedHttpResponse ForData<T>(T data, IClientKeySource clientSessionInfo, EncryptionSchemes encryptionScheme)
        {
            switch (encryptionScheme)
            {
                case EncryptionSchemes.Invalid:
                case EncryptionSchemes.Symmetric:
                default:
                    return CreateSymmetricResponseForData(data, clientSessionInfo);
                case EncryptionSchemes.Asymmetric:
                    return CreateAsymmetricResponseForData(data, clientSessionInfo);
            }
        }

        /// <summary>
        /// Creates an RSA asymmetric encrypted HTTP response for the specified data.
        /// </summary>
        /// <typeparam name="T">The type of data to encrypt.</typeparam>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="clientKeySource">The client key source providing the RSA public key.</param>
        /// <returns>An encrypted HTTP response using RSA asymmetric encryption.</returns>
        public static EncryptedHttpResponse CreateAsymmetricResponseForData<T>(T data, IClientKeySource clientKeySource)
        {
            return new RsaAsymmetricEncryptedHttpResponse<T>(data, clientKeySource);
        }

        /// <summary>
        /// Creates an AES symmetric encrypted HTTP response for the specified data.
        /// </summary>
        /// <typeparam name="T">The type of data to encrypt.</typeparam>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="clientKeySource">The client key source providing the AES key.</param>
        /// <returns>An encrypted HTTP response using AES symmetric encryption.</returns>
        public static EncryptedHttpResponse CreateSymmetricResponseForData<T>(T data, IClientKeySource clientKeySource)
        {
            return new SymmetricEncryptedHttpResponse<T>(data, clientKeySource);
        }
    }
}
