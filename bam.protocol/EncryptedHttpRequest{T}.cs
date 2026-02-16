using Bam.Encryption;

namespace Bam.Protocol
{
    /// <summary>
    /// Represents a strongly-typed encrypted HTTP request with a typed content cipher.
    /// </summary>
    /// <typeparam name="TContent">The type of the content body before encryption.</typeparam>
    public class EncryptedHttpRequest<TContent> : EncryptedHttpRequest
    {
        ContentCipher<TContent> cipher;

        /// <summary>
        /// Gets or sets the strongly-typed content cipher containing the encrypted content body.
        /// </summary>
        public new ContentCipher<TContent> ContentCipher
        {
            get
            {
                return cipher;
            }
            set
            {
                this.cipher = value;
            }
        }

        /// <summary>
        /// Gets the content as the string representation of the cipher. Setting this property directly throws an <see cref="InvalidOperationException"/>.
        /// </summary>
        public override string Content
        {
            get => this.ContentCipher;
            set => throw new InvalidOperationException("EncryptedHttpRequest.Content should not be set directly, use ContentCipher instead");
        }

        /// <summary>
        /// Gets the content type from the cipher, or sets it on the base request.
        /// </summary>
        public override string ContentType
        {
            get => this.ContentCipher?.ContentType; 
            set => base.ContentType = value;
        }
    }
}
