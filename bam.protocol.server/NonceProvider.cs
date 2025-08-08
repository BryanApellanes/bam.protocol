using Bam.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Protocol.Server
{
    public class NonceProvider : INonceProvider
    {
        public NonceProvider() { }
        public int Length { get; set; } = 32;
        public string GetNonce()
        {
            return Length.SecureAlphaNumericCharacters();
        }
    }
}
