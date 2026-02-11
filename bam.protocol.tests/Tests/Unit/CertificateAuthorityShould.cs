using Bam.Data.Objects;
using Bam.Encryption;
using Bam.Test;
using NSubstitute;
using Bam.Protocol;
using Bam.Protocol.Data;
using Bam.Protocol.Profile;

namespace Bam.Application.Unit;

[UnitTestMenu("CertificateAuthority Should", Selector = "cas")]
public class CertificateAuthorityShould : UnitTestMenuContainer
{
    private static CertificateAuthority CreateCertificateAuthority(IActor issuer, IKeyManager keyManager)
    {
        ICertificateManager certManager = Substitute.For<ICertificateManager>();
        return new CertificateAuthority(
            issuer,
            keyManager,
            certManager,
            new BamX509NameProvider(),
            new CompositeKeyCalculator(),
            new CertificateSerialNumberProvider()
        );
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

        IX509NameProvider x509NameProvider = new BamX509NameProvider();

        IKeyManager keyManager = Substitute.For<IKeyManager>();
        keyManager.GetSigningKey(issuer).Returns(issuerKeyPair.PrivateKey);

        When.A<CertificateAuthority>("generates certificate from options",
            () => CreateCertificateAuthority(issuer, keyManager),
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

        IX509NameProvider x509NameProvider = new BamX509NameProvider();

        IKeyManager keyManager = Substitute.For<IKeyManager>();
        keyManager.GetSigningKey(issuer).Returns(issuerKeyPair.PrivateKey);

        When.A<CertificateAuthority>("generates a certificate",
            () => CreateCertificateAuthority(issuer, keyManager),
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
