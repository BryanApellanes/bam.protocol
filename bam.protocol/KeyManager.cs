using Bam.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bam.Protocol.Profile;

namespace Bam.Protocol
{
    public class KeyManager : IKeyManager
    {
        public RsaPublicPrivateKeyPair GenerateRsaKeyPair()
        {
            throw new NotImplementedException();
        }

        public EccPublicPrivateKeyPair GenerateEccKeyPair()
        {
            throw new NotImplementedException();
        }

        public AesKey GenerateAesKey()
        {
            throw new NotImplementedException();
        }

        public AesKey GenerateSharedAesKey(IActor from, IActor to)
        {
            throw new NotImplementedException();
        }

        public IPrivateKey GetSigningKey(IActor actor)
        {
            throw new NotImplementedException();
        }

        public IPrivateKey GetEncryptionKey(IActor actor)
        {
            throw new NotImplementedException();
        }
    }
}
