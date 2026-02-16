using Bam.Encryption;

namespace Bam.Protocol
{
    /// <summary>
    /// Defines a component that decrypts encrypted HTTP requests.
    /// </summary>
    public interface IHttpRequestDecryptor
    {
        /// <summary>
        /// Gets the decryptor used for the content body.
        /// </summary>
        IDecryptor ContentDecryptor { get; }

        /// <summary>
        /// Gets the header decryptor used for cipher headers.
        /// </summary>
        IHttpRequestHeaderDecryptor HeaderDecryptor { get; }

        /// <summary>
        /// Decrypts the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The uncrypted request.</returns>
        IHttpRequest DecryptRequest(IEncryptedHttpRequest request);
    }
}
