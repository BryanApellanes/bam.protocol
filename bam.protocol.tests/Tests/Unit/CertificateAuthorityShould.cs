using Bam.Console;
using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Test;
using NSubstitute;
using Org.BouncyCastle.X509;
using Bam.Encryption;
using Bam.Protocol;
using Bam.Protocol.Profile;
using Org.BouncyCastle.Asn1.X509;
using ThirdParty.BouncyCastle.Math;
using KeyManager = Bam.Protocol.Profile.KeyManager;

namespace Bam.Application.Unit;

[UnitTestMenu("CertificateManager Should", Selector = "cms")]
public class CertificateAuthorityShould : UnitTestMenuContainer
{
    public CertificateAuthorityShould()
    {
        Configure(svcRegistry =>
        {
            svcRegistry.For<IX509NameProvider>().Use<BamX509NameProvider>()
                .For<ICertificateManager>().Use<CertificateManager>()
                .For<IKeyManager>().Use<KeyManager>()
                .For<ICompositeKeyCalculator>().Use<CompositeKeyCalculator>()
                .For<ICertificateSerialNumberProvider>().Use<CertificateSerialNumberProvider>();
        });
    }
    
    [UnitTest]
    public async Task GenerateCertificateFromOptions()
    {
        IActor issuer = Substitute.For<IActor>();
        issuer.Name.Returns($"issuer ({6.RandomLetters()})");
        IActor subject = Substitute.For<IActor>();
        subject.Name.Returns($"subject ({6.RandomLetters()})");
        RsaKeyPair issuerKeyPair = new RsaKeyPair();
        RsaKeyPair subjectKeyPair = new RsaKeyPair();
        
        ServiceRegistry testRegistry = Configure(svcRegistry => svcRegistry.For<IActor>().UseSingleton(issuer));
        IX509NameProvider x509NameProvider = testRegistry.Get<IX509NameProvider>();
        CertificateAuthority ca = DependencyProvider.Get<CertificateAuthority>();
        GenerateCertificateOptions generationOptions = GenerateCertificateOptions
            .Create()
            .IssuerPrivateKey(issuerKeyPair.PrivateKey)
            .SubjectPublicKey(subjectKeyPair.PublicKey)
            .IssuerName(x509NameProvider.GetName(issuer))
            .SubjectName(x509NameProvider.GetName(subject));

        X509Certificate certificate = generationOptions.Generate(ca);

        Message.PrintLine("Serial #: {0}", certificate.SerialNumber.ToString());
        Message.PrintLine("Issuer: {0}", certificate.IssuerDN.ToString());
        Message.PrintLine("Subject: {0}", certificate.SubjectDN.ToString());
        Message.PrintLine("Valid From: {0}", certificate.NotBefore.ToString());
        Message.PrintLine("Valid To: {0}", certificate.NotAfter.ToString());
        Message.PrintLine("Signature Algorithm: {0}", certificate.SigAlgName);
        Message.PrintLine("Signature Hex: {0}", certificate.GetSignature().ToHexString());
        Message.PrintLine("Signature Base64: {0}", certificate.GetSignature().ToBase64());
        Message.PrintLine("SHA1 thumbprint: {0}", certificate.GetEncoded().Sha1());
        Message.PrintLine("Subject Pub Key: {0}", certificate.SubjectPublicKeyInfo);
        Message.PrintLine("PEM");
        Message.PrintLine(certificate.ObjectToPem());

        Message.PrintLine("Signature is issuer pub key valid: {0}", certificate.IsSignatureValid(issuerKeyPair.PublicKey.Value));
        //Message.PrintLine("Signature is subject pub key valid: {0}", certificate.IsSignatureValid(subjectKeyPair.PublicKey.Value));
    }
    
    [UnitTest]
    public async Task GenerateCertificate()
    {
        IActor issuer = Substitute.For<IActor>();
        issuer.Name.Returns($"issuer ({6.RandomLetters()})");
        IActor subject = Substitute.For<IActor>();
        subject.Name.Returns($"subject ({6.RandomLetters()})");
        RsaKeyPair issuerKeyPair = new RsaKeyPair();
        RsaKeyPair subjectKeyPair = new RsaKeyPair();
        
        ServiceRegistry testRegistry = Configure(svcRegistry => svcRegistry.For<IActor>().UseSingleton(issuer));
        IX509NameProvider x509NameProvider = testRegistry.Get<IX509NameProvider>();
        CertificateAuthority ca = DependencyProvider.Get<CertificateAuthority>();
        GenerateCertificateOptions generationOptions = GenerateCertificateOptions
            .Create()
            .IssuerPrivateKey(issuerKeyPair.PrivateKey)
            .SubjectPublicKey(subjectKeyPair.PublicKey)
            .IssuerName(x509NameProvider.GetName(issuer))
            .SubjectName(x509NameProvider.GetName(subject));

        X509Certificate certificate = generationOptions.Generate(ca);

        Message.PrintLine(certificate.IssuerDN.ToString());
        Message.PrintLine(certificate.SubjectDN.ToString());
        Message.PrintLine(certificate.NotBefore.ToString());
        Message.PrintLine(certificate.NotAfter.ToString());
        
        
    }
}