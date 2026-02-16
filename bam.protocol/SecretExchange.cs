namespace Bam.Protocol
{
    /// <summary>
    /// Implements a secret exchange for securely sharing secrets between server and client.
    /// </summary>
    public class SecretExchange : ISecretExchange
    {
        /// <inheritdoc />
        public string Identifier { get; set; }

        /// <inheritdoc />
        public string ServerHostName { get; set; }
        
        /// <inheritdoc />
        public string ClientHostName { get; set; }

        /// <inheritdoc />
        public string SecretCipher { get; set; }
    }
}
