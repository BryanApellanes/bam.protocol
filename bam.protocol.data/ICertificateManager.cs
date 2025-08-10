using Bam.Protocol.Data;
using Org.BouncyCastle.X509;

namespace Bam.Protocol;

public interface ICertificateManager
{
    X509Certificate LoadRootCACertificate(IActor actor);
    X509Certificate CreateRootCACertificate(IActor actor);
    X509Certificate CreateSignedCertificate(IActor actor);
}