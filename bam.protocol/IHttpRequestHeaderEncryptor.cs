using Bam.Encryption;

namespace Bam.Protocol
{
    /// <summary>
    /// Defines a component that encrypts plain-text headers on HTTP requests.
    /// </summary>
    public interface IHttpRequestHeaderEncryptor
    {
        /// <summary>
        /// Gets the encryptor used for header values.
        /// </summary>
        IEncryptor Encryptor { get; }

        /// <summary>
        /// Encrypts plain-text headers on the specified request.
        /// </summary>
        /// <param name="request">The request whose headers should be encrypted.</param>
        void EncryptHeaders(IHttpRequest request);
    }
}