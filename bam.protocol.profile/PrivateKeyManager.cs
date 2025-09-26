using System.Text;
using Bam.Encryption;
using Bam.Storage;
using Bam.Storage.Encryption;

namespace Bam.Protocol.Profile;

public class PrivateKeyManager : IPrivateKeyManager
{
    public PrivateKeyManager(OpaqueFsKeyValuePairStorage opaqueStorage)
    {
        this.OpaqueStorage = opaqueStorage;
    }
    
    protected OpaqueFsKeyValuePairStorage OpaqueStorage { get; set; }
    public IPublicKey GeneratePrivateRsaKey()
    {
        RsaKeyPair keyPair = new RsaKeyPair();
        string publicKeyPemHash = keyPair.PublicPem.Sha256();
        OpaqueStorage.Save(publicKeyPemHash, keyPair.PrivateKey.Pem);
        return keyPair.PublicKey;
    }
    
    public IPrivateKey GetPrivateRsaKey(IPublicKey publicKey)
    {
        IKeyValuePair kvp = OpaqueStorage.Get(publicKey.Pem);
        string pem = Encoding.UTF8.GetString(kvp.Value);
        return new RsaPrivateKey(pem.PemToKey());
    }

    public IPublicKey GeneratePrivateEccKey()
    {
        EccKeyPair keyPair = new EccKeyPair();
        string  publicKeyPemHash = keyPair.PublicPem.Sha256();
        OpaqueStorage.Save(publicKeyPemHash, keyPair.PrivateKey.Pem);
        return keyPair.PublicKey;
    }

    public IPrivateKey GetPrivateEccKey(IPublicKey publicKey)
    {
        IKeyValuePair kvp = OpaqueStorage.Get(publicKey.Pem);
        string pem = Encoding.UTF8.GetString(kvp.Value);
        return new EccPrivateKey(pem.PemToKey());
    }
}