using Bam.Encryption;

namespace Bam.Protocol
{
    /// <summary>
    /// Defines a component that encrypts HTTP requests.
    /// </summary>
    public interface IHttpRequestEncryptor
    {
        /// <summary>
        /// Gets the encryptor used for the content body.
        /// </summary>
        IEncryptor ContentEncryptor { get;  }

        /// <summary>
        /// Gets the header encryptor.
        /// </summary>
        IHttpRequestHeaderEncryptor HeaderEncryptor { get; }

        /// <summary>
        /// Encrypts the specified HTTP request, returning an encrypted copy.
        /// </summary>
        /// <param name="request">The request to encrypt.</param>
        /// <returns>An encrypted copy of the request.</returns>
        EncryptedHttpRequest EncryptRequest(IHttpRequest request);
    }
}
