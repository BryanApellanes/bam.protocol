using Bam.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Protocol.Server
{
    /// <summary>
    /// Generates cryptographic nonce values of a configurable length.
    /// </summary>
    public class NonceProvider : INonceProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonceProvider"/> class.
        /// </summary>
        public NonceProvider() { }

        /// <summary>
        /// Gets or sets the length of the generated nonce string. Defaults to 32.
        /// </summary>
        public int Length { get; set; } = 32;

        /// <summary>
        /// Generates a new secure alphanumeric nonce string.
        /// </summary>
        /// <returns>A random nonce string of the configured length.</returns>
        public string GetNonce()
        {
            return Length.SecureAlphaNumericCharacters();
        }
    }
}
