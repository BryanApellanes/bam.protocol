using Bam.Encryption;

namespace Bam.Protocol
{
    /// <summary>
    /// Decrypts strongly-typed encrypted HTTP requests, deserializing the content body to the specified type.
    /// </summary>
    /// <typeparam name="TContent">The type of the decrypted content body.</typeparam>
    public class HttpRequestDecryptor<TContent> : HttpRequestDecryptor, IHttpRequestDecryptor<TContent>
    {
        /// <summary>
        /// Initializes a new instance using the specified content decryptor.
        /// </summary>
        /// <param name="decryptor">The content decryptor.</param>
        public HttpRequestDecryptor(IContentDecryptor<TContent> decryptor) : base(decryptor)
        {
        }

        /// <summary>
        /// Initializes a new instance using separate decryptors for content and headers.
        /// </summary>
        /// <param name="contentDecrpytor">The typed content decryptor.</param>
        /// <param name="headerDecryptor">The header decryptor.</param>
        public HttpRequestDecryptor(IContentDecryptor<TContent> contentDecrpytor, IDecryptor headerDecryptor) : base(contentDecrpytor, headerDecryptor)
        {
            this.ContentDecryptor = contentDecrpytor;
        }

        /// <summary>
        /// Gets the typed content decryptor.
        /// </summary>
        public new IContentDecryptor<TContent> ContentDecryptor
        {
            get;
            private set;
        }

        /// <summary>
        /// Decrypts the specified typed encrypted request, returning a typed HTTP request with deserialized content.
        /// </summary>
        /// <param name="request">The encrypted request to decrypt.</param>
        /// <returns>A decrypted, typed HTTP request.</returns>
        public IHttpRequest<TContent> DecryptRequest(IEncryptedHttpRequest<TContent> request)
        {
            HttpRequest<TContent> copy = new HttpRequest<TContent>();
            copy.Verb = request.Verb;
            foreach(string key in request.Headers.Keys)
            {
                copy.Headers.Add(key, request.Headers[key]);
            }
            copy.Headers.Add("Content-Type", MediaTypes.Json);
            copy.TypedContent = ContentDecryptor.DecryptContentCipher(request.ContentCipher);
            return copy;
        }
    }
}
