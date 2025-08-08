using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;

namespace Bam.Protocol.Profile;

public class CustomFieldProvider : ICustomFieldProvider
{
    public CustomFieldProvider(IOidProvider oidProvider)
    {
        this.OidProvider = oidProvider;
    }
    
    protected IOidProvider OidProvider { get; }
    
    public void AddCustomField(X509V3CertificateGenerator certificateGenerator, string fieldName, string value)
    {
        DerObjectIdentifier customOid = OidProvider.GetObjectIdentifier();
        Asn1Encodable customValue = new DerUtf8String(value);
        GeneralName otherName =GeneralName.GetOptional(new OtherName(customOid, customValue));
        GeneralNames subjectAltNames = new GeneralNames(new GeneralName[] { otherName });
        certificateGenerator.AddExtension(X509Extensions.SubjectAlternativeName, false, subjectAltNames);
    }
    
    /*

// Define a custom OID (Object Identifier)
// In a real-world scenario, ensure this OID is properly registered or within a private OID arc.
DerObjectIdentifier customOid = new DerObjectIdentifier("1.2.3.4.5.6.7.8.9");

// Create a custom ASN.1 structure for your data (e.g., a String)
Asn1Encodable customValue = new DerUtf8String("MyCustomValue");

// Create an OtherName object
GeneralName otherName = new GeneralName(new OtherName(customOid, customValue));

// Add it to Subject Alternative Name extension
GeneralNames subjectAltNames = new GeneralNames(new GeneralName[] { otherName });
certGenerator.AddExtension(X509Extensions.SubjectAlternativeName, false, subjectAltNames);
 *
 *
 */
}