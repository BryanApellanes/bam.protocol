using Bam.Data.Repositories;
using System.Net;
using Bam.Encryption;
using Bam.Encryption.Data;
using Bam.Protocol.Client;
using Bam.Protocol.Data.Client;
using Org.BouncyCastle.Crypto;

namespace Bam.Protocol.Data
{
    /// <summary>
    /// Represents a key set for the current process or host to use
    /// in communication as the client.
    /// </summary>
    public class ClientKeySet : ClientKeySetData, IApplicationKeySet, IClientKeySet, IAesKeySource, ICommunicationKeySet
    {
        public ClientKeySet(IPrivateKeyManager privateKeyManager) 
        {
            this.PrivateKeyManager = privateKeyManager;
            this.MachineName = Environment.MachineName;
            this.ClientHostName = Dns.GetHostName();
        }

        protected IPrivateKeyManager PrivateKeyManager { get; }

        public string ClientKeySetHandle { get; }

        public IPrivateKey GetRsaPrivateKey()
        {
            return PrivateKeyManager.GetPrivateRsaKey(new RsaPublicKey(ServerRsaKey));
        }

        public IPrivateKey GetEccPrivateKey()
        {
            return PrivateKeyManager.GetPrivateEccKey(new EccPublicKey(ClientEccKey));
        }
        
        public string ServerRsaKey { get; }
        

        public string ServerEccKey { get; }
        public string ClientEccKey { get; }
        public string EncryptAsymmetric(string value)
        {
            return value.EncryptWithPublicKey(ServerRsaKey);
        }

        public string EncryptSymmetric(string value)
        {
            return GetAesKey().Encrypt(value);
        }

        public string DecryptSymmetric(string base64EncodedValue)
        {
            return GetAesKey().Decrypt(base64EncodedValue);
        }

        /// <inheritdoc />
        public string ApplicationName { get; set; }

        public string Secret { get; set; }


        public RsaPublicKey GetRsaPublicKey()
        {
            return new RsaPublicKey(ServerRsaKey);
        }

        public AesKey GetAesKey()
        {
            IPrivateKey privateKey = GetEccPrivateKey();
            EccPublicPrivateKeyPair eccKey = new EccPublicPrivateKeyPair(privateKey.Pem);
            return eccKey.GetSharedAesKey(ServerEccKey);
        }
    }
}
