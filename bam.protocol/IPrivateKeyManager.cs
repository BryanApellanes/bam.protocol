using Bam.Encryption;
using Bam.Storage.Encryption;

namespace Bam.Protocol;

public interface IPrivateKeyManager
{
    IPrivateKey GetPrivateKey(IPublicKey publicKey);
    //IPrivateKey GetPrivateKey(IPublicKey publicKey, IEncryptedStorage storage);
    bool SavePrivateKey(IPrivateKey privateKey);
   // bool SavePrivateKey(IPrivateKey privateKey, IEncryptedStorage storage);
}