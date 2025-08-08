using Bam.Protocol.Profile;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.X509;

namespace Bam.Encryption;

public class GenerateCertificateOptions
{
    private X509Name _issuerX509Name = null;
    private X509Name _subjectX509Name = null;
    private Func<AsymmetricKeyParameter> _getPrivateKey;
    private Func<AsymmetricKeyParameter> _getPublicKey;
    
    public GenerateCertificateOptions()
    {
    }

    public static GenerateCertificateOptions Create()
    {
        return new GenerateCertificateOptions();
    }
    
    public X509Certificate Generate(CertificateAuthority ca)
    {
        return ca.CreateCertificate(this);
    }
    
    
    public X509Name IssuerName()
    {
        return _issuerX509Name;
    }
    
    public GenerateCertificateOptions IssuerName(string issuer)
    {
        _issuerX509Name = new X509Name(issuer);
        return this;
    }

    public GenerateCertificateOptions IssuerName(X509Name issuer)
    {
        _issuerX509Name = issuer;
        return this;
    }
    
    public X509Name SubjectName()
    {
        return _subjectX509Name;
    }
    
    public GenerateCertificateOptions SubjectName(string subject)
    {
        _subjectX509Name = new X509Name(subject);
        return this;
    }

    public GenerateCertificateOptions SubjectName(X509Name subject)
    {
        _subjectX509Name = subject;
        return this;
    }
    public AsymmetricKeyParameter IssuerPrivateKey()
    {
        return _getPrivateKey();
    }

    public GenerateCertificateOptions IssuerPrivateKey(IPrivateKey privateKey)
    {
        return IssuerPrivateKey(() => privateKey.Value);
    }
    
    public GenerateCertificateOptions IssuerPrivateKey(Func<AsymmetricKeyParameter> getPrivateKey)
    {
        _getPrivateKey = getPrivateKey;
        return this;
    }

    public AsymmetricKeyParameter SubjectPublicKey()
    {
        return _getPublicKey();
    }

    public GenerateCertificateOptions SubjectPublicKey(IPublicKey publicKey)
    {
        return PublicKey(() => publicKey.Value);
    }
    
    public GenerateCertificateOptions PublicKey(Func<AsymmetricKeyParameter> getPublicKey)
    {
        _getPublicKey = getPublicKey;
        return this;
    }

    public GenerateCertificateOptions PrivateKey(IPrivateKey privateKey)
    {
        _getPrivateKey = () => privateKey.Value;
        return this;
    }

    public GenerateCertificateOptions PublicKey(IPublicKey publicKeyd)
    {
        _getPublicKey = () => publicKeyd.Value;
        return this;
    }
    
    public GenerateCertificateOptions PrivateKey(IPrivateKeyProvider privateKeyProvider)
    {
        _getPrivateKey = privateKeyProvider.GetPrivateKey;
        return this;
    }

    public GenerateCertificateOptions PublicKey(IPublicKeyProvider publicKeyProvider)
    {
        _getPublicKey = publicKeyProvider.GetPublicKey;
        return this;
    }

    public GenerateCertificateOptions PrivateKey(AsymmetricKeyParameter privateKey)
    {
        _getPrivateKey = () => privateKey;
        return this;
    }

    public GenerateCertificateOptions PublicKey(AsymmetricKeyParameter publicKey)
    {
        _getPublicKey = () => publicKey;
        return this;
    }
}