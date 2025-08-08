using Bam.Encryption;

namespace Bam.Protocol
{
    public interface IHttpRequestDecryptor<TContent> : IHttpRequestDecryptor
    {
        new IContentDecryptor<TContent> ContentDecryptor { get; }

        IHttpRequest<TContent> DecryptRequest(IEncryptedHttpRequest<TContent> request);
    }
}
