using Bam.Protocol.Data;
using Org.BouncyCastle.Asn1.X509;

namespace Bam.Protocol;

public interface IX509NameProvider
{
    X509Name GetName(IActor actor);
    X509Name GetName(string subjectName);
}