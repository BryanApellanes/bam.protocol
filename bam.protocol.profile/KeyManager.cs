using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Profile;

public class KeyManager : IKeyManager
{
    public KeyManager() { }

    public KeyManager(IProfileRepository repository, IPrivateKeyManager privateKeyManager)
    {
        this.Repository = repository;
        this.PrivateKeyManager = privateKeyManager;
    }

    protected IProfileRepository Repository { get; } = null!;
    protected IPrivateKeyManager PrivateKeyManager { get; } = null!;

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

    public IPrivateKey GetSigningKey(IActor actor)
    {
        PublicKeySetData keySet = Repository.FindPublicKeySetByHandle(actor.Handle);
        if (keySet == null)
            throw new InvalidOperationException($"No key set found for actor '{actor.Handle}'.");
        if (string.IsNullOrEmpty(keySet.PublicRsaKey))
            throw new InvalidOperationException($"Actor '{actor.Handle}' has no public RSA key.");

        return PrivateKeyManager.GetPrivateRsaKey(new RsaPublicKey(keySet.PublicRsaKey));
    }

    public IPrivateKey GetEncryptionKey(IActor actor)
    {
        PublicKeySetData keySet = Repository.FindPublicKeySetByHandle(actor.Handle);
        if (keySet == null)
            throw new InvalidOperationException($"No key set found for actor '{actor.Handle}'.");
        if (string.IsNullOrEmpty(keySet.PublicEccKey))
            throw new InvalidOperationException($"Actor '{actor.Handle}' has no public ECC key.");

        return PrivateKeyManager.GetPrivateEccKey(new EccPublicKey(keySet.PublicEccKey));
    }

    public AesKey GenerateSharedAesKey(IActor from, IActor to)
    {
        IPrivateKey fromPrivateKey = GetEncryptionKey(from);

        PublicKeySetData toKeySet = Repository.FindPublicKeySetByHandle(to.Handle);
        if (toKeySet == null)
            throw new InvalidOperationException($"No key set found for actor '{to.Handle}'.");
        if (string.IsNullOrEmpty(toKeySet.PublicEccKey))
            throw new InvalidOperationException($"Actor '{to.Handle}' has no public ECC key.");

        EccPublicPrivateKeyPair eccKey = new EccPublicPrivateKeyPair(fromPrivateKey.Pem);
        return eccKey.GetSharedAesKey(toKeySet.PublicEccKey);
    }
}
