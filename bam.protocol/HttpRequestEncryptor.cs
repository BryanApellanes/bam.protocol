using Bam.Encryption;

namespace Bam.Protocol
{
    /// <summary>
    /// Encrypts HTTP requests by encrypting both the content body and specified headers.
    /// </summary>
    public class HttpRequestEncryptor : IHttpRequestEncryptor
    {
        /// <summary>
        /// Initializes a new instance using the specified encryptor for both content and headers.
        /// </summary>
        /// <param name="encryptor">The encryptor to use for both content and headers.</param>
        public HttpRequestEncryptor(IEncryptor encryptor)
        {
            this.ContentEncryptor = encryptor;
            this.HeaderEncryptor = new HttpRequestHeaderEncryptor(encryptor);
        }

        /// <summary>
        /// Initializes a new instance using separate encryptors for content and headers.
        /// </summary>
        /// <param name="contentEncryptor">The encryptor for the content body.</param>
        /// <param name="headerEncryptor">The encryptor for headers.</param>
        public HttpRequestEncryptor(IEncryptor contentEncryptor, IEncryptor headerEncryptor)
        {
            this.ContentEncryptor = contentEncryptor;
            this.HeaderEncryptor = new HttpRequestHeaderEncryptor(headerEncryptor);
        }

        /// <summary>
        /// Gets the encryptor used for the content body.
        /// </summary>
        public IEncryptor ContentEncryptor
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the header encryptor.
        /// </summary>
        public IHttpRequestHeaderEncryptor HeaderEncryptor
        {
            get;
            private set;
        }

        /// <summary>
        /// Encrypts the specified HTTP request, returning an encrypted copy.
        /// </summary>
        /// <param name="request">The request to encrypt.</param>
        /// <returns>An encrypted copy of the request.</returns>
        public EncryptedHttpRequest EncryptRequest(IHttpRequest request)
        {
            EncryptedHttpRequest copy = new EncryptedHttpRequest();
            copy.Copy(request);
            copy.ContentCipher = ContentEncryptor.Encrypt(request.Content);
            HeaderEncryptor.EncryptHeaders(copy);
            return copy;
        }
    }
}
