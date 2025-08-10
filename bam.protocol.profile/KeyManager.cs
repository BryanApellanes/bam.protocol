using Bam.Encryption;
using Bam.Protocol.Data;

namespace Bam.Protocol.Profile;

public class KeyManager : IKeyManager
{
    public RsaPublicPrivateKeyPair GenerateRsaKeyPair()
    {
        return new RsaPublicPrivateKeyPair();
    }

    public EccPublicPrivateKeyPair GenerateEccKeyPair()
    {
        return new EccPublicPrivateKeyPair();
    }

    public AesKey GenerateAesKey()
    {
        return new AesKey();
    }

    public AesKey GenerateSharedAesKey(IActor from, IActor to)
    {
        throw new NotImplementedException();
    }

    public IPrivateKey GetSigningKey(IActor actor)
    {
        throw new NotImplementedException();
    }

    public IPrivateKey GetEncryptionKey(IActor actor)
    {
        throw new NotImplementedException();
    }
}