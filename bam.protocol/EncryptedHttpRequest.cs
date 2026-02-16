using Bam.Encryption;

namespace Bam.Protocol
{
    /// <summary>
    /// Represents an HTTP request whose content body is encrypted as a cipher.
    /// </summary>
    public class EncryptedHttpRequest : HttpRequest, IEncryptedHttpRequest
    {
        /// <summary>
        /// Gets or sets the cipher containing the encrypted content body.
        /// </summary>
        public Cipher ContentCipher { get; internal set; }

        /// <summary>
        /// Gets the content as the string representation of the cipher. Setting this property directly throws an <see cref="InvalidOperationException"/>.
        /// </summary>
        public override string Content
        {
            get => this.ContentCipher;
            set => throw new InvalidOperationException("EncryptedHttpRequest.Content should not be set directly, use ContentCipher instead");
        }

        /// <summary>
        /// Copies the URI, verb, and headers from the specified request without copying the content.
        /// </summary>
        /// <param name="request">The request to copy from.</param>
        public override void Copy(IHttpRequest request)
        {
            this.Uri = request.Uri;
            this.Verb = request.Verb;
            foreach (string key in request.Headers.Keys)
            {
                this.Headers.Add(key, request.Headers[key]);
            }
        }
    }
}
