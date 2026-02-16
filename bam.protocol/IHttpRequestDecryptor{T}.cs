using Bam.Encryption;

namespace Bam.Protocol
{
    /// <summary>
    /// Defines a component that decrypts strongly-typed encrypted HTTP requests.
    /// </summary>
    /// <typeparam name="TContent">The type of the decrypted content.</typeparam>
    public interface IHttpRequestDecryptor<TContent> : IHttpRequestDecryptor
    {
        /// <summary>
        /// Gets the typed content decryptor.
        /// </summary>
        new IContentDecryptor<TContent> ContentDecryptor { get; }

        /// <summary>
        /// Decrypts the specified typed encrypted request.
        /// </summary>
        /// <param name="request">The encrypted request to decrypt.</param>
        /// <returns>A decrypted, typed HTTP request.</returns>
        IHttpRequest<TContent> DecryptRequest(IEncryptedHttpRequest<TContent> request);
    }
}
