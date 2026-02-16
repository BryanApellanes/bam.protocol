namespace Bam.Encryption
{
    /// <summary>
    /// Defines a source that provides both AES and RSA public keys for client communication.
    /// </summary>
    public interface IClientKeySource : IAesKeySource, IRsaPublicKeySource
    {
    }
}
