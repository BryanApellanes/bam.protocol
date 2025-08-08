using Bam.Encryption;

namespace Bam.Protocol
{
    public interface IEncryptedHttpRequest<TContent> : IEncryptedHttpRequest, IHttpRequest<TContent>
    {
        new ContentCipher<TContent> ContentCipher { get; }
    }
}
