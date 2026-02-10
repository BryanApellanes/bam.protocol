using Bam.Console;
using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Test;
using NSubstitute;
using Org.BouncyCastle.X509;
using Bam.Encryption;
using Bam.Protocol;
using Bam.Protocol.Data;
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
    public void GenerateCertificateFromOptions()
    {
        IActor issuer = Substitute.For<IActor>();
        issuer.Name.Returns($"issuer ({6.RandomLetters()})");
        IActor subject = Substitute.For<IActor>();
        subject.Name.Returns($"subject ({6.RandomLetters()})");
        RsaKeyPair issuerKeyPair = new RsaKeyPair();
        RsaKeyPair subjectKeyPair = new RsaKeyPair();

        ServiceRegistry testRegistry = Configure(svcRegistry => svcRegistry.For<IActor>().UseSingleton(issuer));
        IX509NameProvider x509NameProvider = testRegistry.Get<IX509NameProvider>();

        When.A<CertificateAuthority>("generates certificate from options",
            () => DependencyProvider.Get<CertificateAuthority>(),
            (ca) =>
            {
                GenerateCertificateOptions generationOptions = GenerateCertificateOptions
                    .Create()
                    .IssuerPrivateKey(issuerKeyPair.PrivateKey)
                    .SubjectPublicKey(subjectKeyPair.PublicKey)
                    .IssuerName(x509NameProvider.GetName(issuer))
                    .SubjectName(x509NameProvider.GetName(subject));
                return generationOptions.Generate(ca);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void GenerateCertificate()
    {
        IActor issuer = Substitute.For<IActor>();
        issuer.Name.Returns($"issuer ({6.RandomLetters()})");
        IActor subject = Substitute.For<IActor>();
        subject.Name.Returns($"subject ({6.RandomLetters()})");
        RsaKeyPair issuerKeyPair = new RsaKeyPair();
        RsaKeyPair subjectKeyPair = new RsaKeyPair();

        ServiceRegistry testRegistry = Configure(svcRegistry => svcRegistry.For<IActor>().UseSingleton(issuer));
        IX509NameProvider x509NameProvider = testRegistry.Get<IX509NameProvider>();

        When.A<CertificateAuthority>("generates a certificate",
            () => DependencyProvider.Get<CertificateAuthority>(),
            (ca) =>
            {
                GenerateCertificateOptions generationOptions = GenerateCertificateOptions
                    .Create()
                    .IssuerPrivateKey(issuerKeyPair.PrivateKey)
                    .SubjectPublicKey(subjectKeyPair.PublicKey)
                    .IssuerName(x509NameProvider.GetName(issuer))
                    .SubjectName(x509NameProvider.GetName(subject));
                return generationOptions.Generate(ca);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
