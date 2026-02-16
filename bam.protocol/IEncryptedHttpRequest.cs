using Bam.Encryption;

namespace Bam.Protocol
{
    /// <summary>
    /// Defines an HTTP request with an encrypted content body represented as a cipher.
    /// </summary>
    public interface IEncryptedHttpRequest : IHttpRequest
    {
        /// <summary>
        /// Gets the cipher containing the encrypted content body.
        /// </summary>
        Cipher ContentCipher { get; }
    }
}