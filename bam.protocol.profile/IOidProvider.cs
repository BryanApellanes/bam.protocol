using Org.BouncyCastle.Asn1;

namespace Bam.Protocol.Profile;

public interface IOidProvider
{
    DerObjectIdentifier GetObjectIdentifier();
}