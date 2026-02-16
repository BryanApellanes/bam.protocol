using Bam.Encryption;

namespace Bam.Protocol
{
    /// <summary>
    /// Defines a component that decrypts cipher headers on HTTP requests.
    /// </summary>
    public interface IHttpRequestHeaderDecryptor
    {
        /// <summary>
        /// Gets the decryptor used for header values.
        /// </summary>
        IDecryptor Decryptor { get; }

        /// <summary>
        /// Decrypts cipher headers on the specified request.
        /// </summary>
        /// <param name="request">The request whose cipher headers should be decrypted.</param>
        void DecryptHeaders(IHttpRequest request);
    }
}
