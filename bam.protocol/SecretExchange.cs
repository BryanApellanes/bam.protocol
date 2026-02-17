namespace Bam.Protocol
{
    /// <summary>
    /// Implements a secret exchange for securely sharing secrets between server and client.
    /// </summary>
#pragma warning disable CS0618
    public class SecretExchange : ISecretExchange
#pragma warning restore CS0618
    {
        /// <inheritdoc />
        public string Identifier { get; set; } = null!;

        /// <inheritdoc />
        public string ServerHostName { get; set; } = null!;

        /// <inheritdoc />
        public string ClientHostName { get; set; } = null!;

        /// <inheritdoc />
        public string SecretCipher { get; set; } = null!;
    }
}
