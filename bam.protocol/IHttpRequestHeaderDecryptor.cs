using Bam.Encryption;

namespace Bam.Protocol
{
    public interface IHttpRequestHeaderDecryptor
    {
        IDecryptor Decryptor { get; }

        void DecryptHeaders(IHttpRequest request);
    }
}
