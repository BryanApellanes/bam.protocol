using Bam.Encryption;

namespace Bam.Protocol
{
    public class EncryptedHttpRequest<TContent> : EncryptedHttpRequest
    {
        ContentCipher<TContent> cipher;

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

        public override string Content 
        {
            get => this.ContentCipher;
            set => throw new InvalidOperationException("EncryptedHttpRequest.Content should not be set directly, use ContentCipher instead");
        }

        public override string ContentType 
        {
            get => this.ContentCipher?.ContentType; 
            set => base.ContentType = value;
        }
    }
}
