using Bam.Encryption;

namespace Bam.Protocol
{
    /// <summary>
    /// Encrypts strongly-typed HTTP requests, serializing and encrypting the content body.
    /// </summary>
    /// <typeparam name="TContent">The type of the content body before encryption.</typeparam>
    public class HttpRequestEncryptor<TContent> : HttpRequestEncryptor, IHttpRequestEncryptor<TContent>
    {
        /// <summary>
        /// Initializes a new instance using the specified typed content encryptor.
        /// </summary>
        /// <param name="encryptor">The typed content encryptor.</param>
        public HttpRequestEncryptor(IContentEncryptor<TContent> encryptor) : base(encryptor)
        {
            this.ContentEncryptor = encryptor;
        }

        /// <summary>
        /// Initializes a new instance using separate encryptors for typed content and headers.
        /// </summary>
        /// <param name="contentEncryptor">The typed content encryptor.</param>
        /// <param name="headerEncryptor">The header encryptor.</param>
        public HttpRequestEncryptor(IContentEncryptor<TContent> contentEncryptor, IEncryptor headerEncryptor) : base(contentEncryptor, headerEncryptor)
        {
            this.ContentEncryptor = contentEncryptor;
        }

        /// <summary>
        /// Gets the typed content encryptor.
        /// </summary>
        public new IContentEncryptor<TContent> ContentEncryptor { get; private set; }

        /// <summary>
        /// Returns an encrypted copy of the specified request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public EncryptedHttpRequest<TContent> EncryptRequest(IHttpRequest<TContent> request)
        {
            EncryptedHttpRequest<TContent> copy = new EncryptedHttpRequest<TContent>();
            copy.Copy(request);
            ContentCipher<TContent> cipher = ContentEncryptor.GetContentCipher(request.TypedContent);
            HeaderEncryptor.EncryptHeaders(copy);
            copy.ContentCipher = cipher;
            copy.ContentType = cipher.ContentType;
            return copy;
        }
    }
}
