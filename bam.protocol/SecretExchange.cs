namespace Bam.Protocol
{
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
