using Bam.Server;

namespace Bam.Encryption
{
    /// <summary>
    /// Represents an encrypted HTTP response that uses AES symmetric encryption for the content body.
    /// </summary>
    /// <typeparam name="T">The type of data to encrypt in the response body.</typeparam>
    public class SymmetricEncryptedHttpResponse<T> : EncryptedHttpResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SymmetricEncryptedHttpResponse{T}"/> class with a 200 status code.
        /// </summary>
        public SymmetricEncryptedHttpResponse()
        {
            this.StatusCode = 200;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymmetricEncryptedHttpResponse{T}"/> class, encrypting the specified data with the given AES key source.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="aesKeySource">The source of the AES key for encryption.</param>
        public SymmetricEncryptedHttpResponse(T data, IAesKeySource aesKeySource) : this()
        {
            SymmetricDataEncryptor<T> encryptor = new SymmetricDataEncryptor<T>(aesKeySource);
            this.ContentCipher = new SymmetricContentCipher(encryptor.Encrypt(data));
        }
    }
}
