//using Bam.Encryption.Data.Dao.Repository;

using Bam.Encryption;
using Bam.Protocol.Client;

namespace Bam.Protocol
{
    public interface IClientKeySetDataManager
    {
        //EncryptionDataRepository EncryptionDataRepository { get; }

        IApplicationNameProvider ApplicationNameProvider { get; }

        Task<IClientKeySet> SaveClientKeySetAsync(IClientKeySet clientKeySet); // client side: save the client key set for future retrieval

        /// <summary>
        /// Create an aes key exchange for the specified client key set.
        /// </summary>
        /// <param name="clientKeySet"></param>
        /// <returns></returns>
        Task<IAesKeyExchange> CreateAesKeyExchangeAsync(IClientKeySet clientKeySet); // client side: set the aes key and send exchange

        Task<IClientKeySet> RetrieveClientKeySetForPublicKeyAsync(string publicKey); // client side

        Task<IClientKeySet> RetrieveClientKeySetAsync(string identifier); // client side

        Task<IClientKeySet> SetSecret(ISecretExchange secretExchange);
    }
}
