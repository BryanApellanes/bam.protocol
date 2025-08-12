using Bam.Encryption;
using Bam.Storage.Encryption;

namespace Bam.Protocol;

public class PrivateKeyManager : IPrivateKeyManager
{
    public IPrivateKey GetPrivateKey(IPublicKey publicKey)
    {
        throw new NotImplementedException();
    }

    /*public IPrivateKey GetPrivateKey(IPublicKey publicKey, IEncryptedStorage storage)
    {
        throw new NotImplementedException();
    }*/

    public bool SavePrivateKey(IPrivateKey privateKey)
    {
        throw new NotImplementedException();
    }

    /*public bool SavePrivateKey(IPrivateKey privateKey, IEncryptedStorage storage)
    {
        throw new NotImplementedException();
    }*/
}