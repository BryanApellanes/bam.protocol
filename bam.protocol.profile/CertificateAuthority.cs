using Bam.Data.Objects;
using Bam.Encryption;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;

namespace Bam.Protocol.Profile;

public class CertificateAuthority : CertificateIssuer
{
    public CertificateAuthority(IActor issuer, IKeyManager keyManager, ICertificateManager certificateManager,
        IX509NameProvider x509NameProvider, ICompositeKeyCalculator compositeKeyCalculator,
        ICertificateSerialNumberProvider serialNumberProvider) : base(serialNumberProvider)
    {
        this.Issuer = issuer;
        this.CertificateManager = certificateManager;
        this.KeyManager = keyManager;
        this.X509NameProvider = x509NameProvider;
        this.CompositeKeyCalculator = compositeKeyCalculator;
    }
    
    protected IActor Issuer { get; }
    protected ICertificateManager CertificateManager { get; }
    protected IKeyManager KeyManager { get; }
    protected IX509NameProvider X509NameProvider { get; }
    protected ICompositeKeyCalculator CompositeKeyCalculator { get; }

    public X509Certificate CreateCertificate(GenerateCertificateOptions options)
    {
        return CreateCertificate(options.IssuerName(), options.SubjectName(), options.IssuerPrivateKey(),
            options.SubjectPublicKey());
    }
    
    public X509Certificate CreateCertificate(string subject, IPublicKey subjectPublic)
    {
        X509Name issuerName = X509NameProvider.GetName(Issuer);
        X509Name subjectName = X509NameProvider.GetName(subject);
        return base.CreateCertificate(issuerName, subjectName, KeyManager.GetSigningKey(Issuer).Value, subjectPublic.Value);
    }
}