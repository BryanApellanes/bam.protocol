//using Bam.Encryption.Data.Dao.Repository;

using Bam.Encryption;
using Bam.Protocol.Client;

namespace Bam.Protocol
{
    /// <summary>
    /// Manages persistence and key exchange operations for client key sets.
    /// </summary>
    public interface IClientKeySetDataManager
    {
        //EncryptionDataRepository EncryptionDataRepository { get; }

        /// <summary>
        /// Gets the application name provider.
        /// </summary>
        IApplicationNameProvider ApplicationNameProvider { get; }

        /// <summary>
        /// Saves the specified client key set for future retrieval.
        /// </summary>
        /// <param name="clientKeySet">The client key set to save.</param>
        /// <returns>The saved client key set.</returns>
        Task<IClientKeySet> SaveClientKeySetAsync(IClientKeySet clientKeySet);

        /// <summary>
        /// Create an aes key exchange for the specified client key set.
        /// </summary>
        /// <param name="clientKeySet"></param>
        /// <returns></returns>
        Task<IAesKeyExchange> CreateAesKeyExchangeAsync(IClientKeySet clientKeySet); // client side: set the aes key and send exchange

        /// <summary>
        /// Retrieves a client key set by its public key.
        /// </summary>
        /// <param name="publicKey">The public key to search for.</param>
        /// <returns>The matching client key set.</returns>
        Task<IClientKeySet> RetrieveClientKeySetForPublicKeyAsync(string publicKey);

        /// <summary>
        /// Retrieves a client key set by its identifier.
        /// </summary>
        /// <param name="identifier">The identifier of the client key set.</param>
        /// <returns>The matching client key set.</returns>
        Task<IClientKeySet> RetrieveClientKeySetAsync(string identifier);

        /// <summary>
        /// Sets the secret on the client key set identified by the secret exchange.
        /// </summary>
        /// <param name="secretExchange">The secret exchange containing the encrypted secret and identifier.</param>
        /// <returns>The client key set with the secret applied.</returns>
        Task<IClientKeySet> SetSecret(ISecretExchange secretExchange);
    }
}
