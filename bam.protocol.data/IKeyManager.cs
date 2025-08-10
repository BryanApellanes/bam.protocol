using Bam.Encryption;
using Bam.Protocol.Data;

namespace Bam.Protocol.Profile;

public interface IKeyManager
{
    RsaPublicPrivateKeyPair GenerateRsaKeyPair();
    EccPublicPrivateKeyPair GenerateEccKeyPair();
    AesKey GenerateAesKey();
    AesKey GenerateSharedAesKey(IActor from, IActor to);
    
    IPrivateKey GetSigningKey(IActor actor);
    IPrivateKey GetEncryptionKey(IActor actor);
}