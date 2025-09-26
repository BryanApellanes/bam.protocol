using Bam.Encryption;
using Bam.Storage.Encryption;

namespace Bam.Protocol;

public interface IPrivateKeyManager
{
    IPublicKey GeneratePrivateRsaKey();
    IPrivateKey GetPrivateRsaKey(IPublicKey publicKey);
    IPublicKey GeneratePrivateEccKey();
    IPrivateKey GetPrivateEccKey(IPublicKey publicKey);
    
}