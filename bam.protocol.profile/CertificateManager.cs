using Bam.Protocol;
using Org.BouncyCastle.X509;

namespace Bam.Protocol;

public class CertificateManager : ICertificateManager
{
    public X509Certificate LoadRootCACertificate(IActor actor)
    {
        throw new NotImplementedException();
    }

    public X509Certificate CreateRootCACertificate(IActor actor)
    {
        throw new NotImplementedException();
    }

    public X509Certificate CreateSignedCertificate(IActor actor)
    {
        throw new NotImplementedException();
    }
}