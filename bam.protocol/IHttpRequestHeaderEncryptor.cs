using Bam.Encryption;

namespace Bam.Protocol
{
    public interface IHttpRequestHeaderEncryptor
    {
        IEncryptor Encryptor { get; }

        void EncryptHeaders(IHttpRequest request);
    }
}