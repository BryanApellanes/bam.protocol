using Bam.Encryption;

namespace Bam.Protocol
{
    public interface IEncryptedHttpRequest : IHttpRequest
    {
        Cipher ContentCipher { get; }
    }
}