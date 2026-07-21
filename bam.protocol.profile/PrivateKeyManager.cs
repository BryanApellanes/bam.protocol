using Bam.Encryption;
using Bam.Storage;
using Bam.Storage.Encryption;
using Org.BouncyCastle.Crypto;

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
        OpaqueStorage.Save(StorageKey(keyPair.PublicPem), keyPair.PrivateKey.Pem);
        return keyPair.PublicKey;
    }

    public IPrivateKey GetPrivateRsaKey(IPublicKey publicKey)
    {
        return new RsaPrivateKey(DecodePrivateKey(GetPrivateKeyPem(publicKey)));
    }

    public IPublicKey GeneratePrivateEccKey()
    {
        EccKeyPair keyPair = new EccKeyPair();
        OpaqueStorage.Save(StorageKey(keyPair.PublicPem), keyPair.PrivateKey.Pem);
        return keyPair.PublicKey;
    }

    public IPrivateKey GetPrivateEccKey(IPublicKey publicKey)
    {
        return new EccPrivateKey(DecodePrivateKey(GetPrivateKeyPem(publicKey)));
    }

    /// <summary>
    /// Reads the stored private key PEM bytes for the specified public key.
    /// </summary>
    /// <param name="publicKey">The public key whose stored private key PEM to read.</param>
    /// <returns>The PEM-encoded private key bytes.</returns>
    /// <exception cref="InvalidOperationException">No private key is stored for the specified public key; the underlying storage failure, if any, is the inner exception.</exception>
    private byte[] GetPrivateKeyPem(IPublicKey publicKey)
    {
        IKeyValuePair keyValuePair;
        try
        {
            keyValuePair = OpaqueStorage.Get(StorageKey(publicKey.Pem));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("No private key stored for the specified public key.", ex);
        }
        if (keyValuePair == null || keyValuePair.Value == null || keyValuePair.Value.Length == 0)
        {
            throw new InvalidOperationException("No private key stored for the specified public key.");
        }
        return keyValuePair.Value;
    }

    /// <summary>
    /// Computes the storage key for the specified public key PEM. Both save and get must address
    /// storage through this method so that a generated private key is retrievable by its public key.
    /// </summary>
    /// <param name="publicKeyPem">The PEM-encoded public key string.</param>
    /// <returns>The storage key.</returns>
    private static string StorageKey(string publicKeyPem)
    {
        return publicKeyPem.Sha256();
    }

    /// <summary>
    /// Decodes PEM-encoded private key bytes into the private asymmetric key parameter.
    /// Private-key PEMs parse as a key pair, so the pair is read and its private half returned.
    /// </summary>
    /// <param name="privateKeyPem">The PEM-encoded private key bytes.</param>
    /// <returns>The private asymmetric key parameter.</returns>
    private static AsymmetricKeyParameter DecodePrivateKey(byte[] privateKeyPem)
    {
        return privateKeyPem.PemToKeyPair().Private;
    }
}
