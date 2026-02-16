using Bam.Encryption;

namespace Bam.Protocol
{
    /// <summary>
    /// Decrypts encrypted HTTP requests by decrypting both the content body and cipher headers.
    /// </summary>
    public class HttpRequestDecryptor : IHttpRequestDecryptor
    {
        /// <summary>
        /// Initializes a new instance using the specified decryptor for both content and headers.
        /// </summary>
        /// <param name="decryptor">The decryptor to use for both content and headers.</param>
        public HttpRequestDecryptor(IDecryptor decryptor)
        {
            this.ContentDecryptor = decryptor;
            this.HeaderDecryptor = new HttpRequestHeaderDecryptor(decryptor);
        }

        /// <summary>
        /// Initializes a new instance using separate decryptors for content and headers.
        /// </summary>
        /// <param name="contentDecryptor">The decryptor for the content body.</param>
        /// <param name="headerDecryptor">The decryptor for cipher headers.</param>
        public HttpRequestDecryptor(IDecryptor contentDecryptor, IDecryptor headerDecryptor)
        {
            this.ContentDecryptor = contentDecryptor;
            this.HeaderDecryptor = new HttpRequestHeaderDecryptor(headerDecryptor);
        }

        /// <summary>
        /// Gets the decryptor used for the content body.
        /// </summary>
        public IDecryptor ContentDecryptor
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the header decryptor used for cipher headers.
        /// </summary>
        public IHttpRequestHeaderDecryptor HeaderDecryptor
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public IHttpRequest DecryptRequest(IEncryptedHttpRequest request)
        {
            HttpRequest copy = new HttpRequest();
            copy.Copy(request);
            copy.Content = ContentDecryptor.DecryptCipher(request.ContentCipher);
            HeaderDecryptor.DecryptHeaders(copy);
            return copy;
        }
    }
}
