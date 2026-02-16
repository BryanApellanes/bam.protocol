using Bam.Encryption;

namespace Bam.Protocol
{
    /// <summary>
    /// A class used to encrypt the content body of a request.
    /// </summary>
    /// <typeparam name="TContent">The type of the content body before encryption.</typeparam>
    public interface IHttpRequestEncryptor<TContent> : IHttpRequestEncryptor
    {
        /// <summary>
        /// Gets the typed content encryptor.
        /// </summary>
        new IContentEncryptor<TContent> ContentEncryptor { get; }

        /// <summary>
        /// Returns an encrypted copy of the specified request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        EncryptedHttpRequest<TContent> EncryptRequest(IHttpRequest<TContent> request);
    }
}
