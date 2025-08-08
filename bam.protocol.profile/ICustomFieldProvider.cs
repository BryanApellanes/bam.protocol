using Org.BouncyCastle.X509;

namespace Bam.Protocol.Profile;

public interface ICustomFieldProvider
{
    void AddCustomField(X509V3CertificateGenerator certificateGenerator, string fieldName, string value);

}