using Bam.Server;

namespace Bam.Encryption
{
    /// <summary>
    /// Represents an encrypted HTTP response that uses RSA asymmetric encryption for the content body.
    /// </summary>
    /// <typeparam name="T">The type of data to encrypt in the response body.</typeparam>
    public class RsaAsymmetricEncryptedHttpResponse<T> : EncryptedHttpResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RsaAsymmetricEncryptedHttpResponse{T}"/> class with a 200 status code.
        /// </summary>
        public RsaAsymmetricEncryptedHttpResponse()
        {
            this.StatusCode = 200;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RsaAsymmetricEncryptedHttpResponse{T}"/> class, encrypting the specified data with the given RSA public key source.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="rsaPublicKeySource">The source of the RSA public key for encryption.</param>
        public RsaAsymmetricEncryptedHttpResponse(T data, IRsaPublicKeySource rsaPublicKeySource) :this()
        {
            RsaAsymmetricDataEncryptor<T> encryptor = new RsaAsymmetricDataEncryptor<T>(rsaPublicKeySource);
            this.ContentCipher = new RsaAsymmetricContentCipher(encryptor.Encrypt(data));
        }
    }
}
