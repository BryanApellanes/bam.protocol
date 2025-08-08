using Bam.Encryption;

namespace Bam.Protocol
{
    public interface IHttpRequestEncryptor
    {
        IEncryptor ContentEncryptor { get;  }
        IHttpRequestHeaderEncryptor HeaderEncryptor { get; }

        EncryptedHttpRequest EncryptRequest(IHttpRequest request);
    }
}
