using Bam.Encryption;

namespace Bam.Protocol
{
    /// <summary>
    /// Defines a strongly-typed encrypted HTTP request with a typed content cipher.
    /// </summary>
    /// <typeparam name="TContent">The type of the content before encryption.</typeparam>
    public interface IEncryptedHttpRequest<TContent> : IEncryptedHttpRequest, IHttpRequest<TContent>
    {
        /// <summary>
        /// Gets the typed content cipher containing the encrypted content body.
        /// </summary>
        new ContentCipher<TContent> ContentCipher { get; }
    }
}
