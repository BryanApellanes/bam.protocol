using Bam.Server;

namespace Bam.Encryption
{
    public class RsaAsymmetricEncryptedHttpResponse<T> : EncryptedHttpResponse
    {
        public RsaAsymmetricEncryptedHttpResponse()
        {
            this.StatusCode = 200;
        }

        public RsaAsymmetricEncryptedHttpResponse(T data, IRsaPublicKeySource rsaPublicKeySource) :this()
        {
            RsaAsymmetricDataEncryptor<T> encryptor = new RsaAsymmetricDataEncryptor<T>(rsaPublicKeySource);
            this.ContentCipher = new RsaAsymmetricContentCipher(encryptor.Encrypt(data));
        }
    }
}
