using Bam.Data.Objects;
using Bam.Data.SQLite;
using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Data.Profile.Dao.Repository;
using Bam.Protocol.Profile;
using Bam.Test;
using NSubstitute;
using Org.BouncyCastle.X509;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("CertificateManager Should", Selector = "certms")]
public class CertificateManagerShould : UnitTestMenuContainer
{
    private static ProfileSchemaRepository CreateRepository(string testName)
    {
        ProfileSchemaRepository repo = new ProfileSchemaRepository()
        {
            Database = new SQLiteDatabase(new FileInfo($"./.bam/tests/{testName}.sqlite"))
        };
        repo.Initialize();
        return repo;
    }

    private static CertificateAuthority CreateCertificateAuthority(
        IActor issuer,
        IKeyManager keyManager,
        ICertificateManager certificateManager)
    {
        return new CertificateAuthority(
            issuer,
            keyManager,
            certificateManager,
            new BamX509NameProvider(),
            new CompositeKeyCalculator(),
            new CertificateSerialNumberProvider()
        );
    }

    [UnitTest]
    public void CreateRootCACertificate()
    {
        RsaKeyPair issuerKeyPair = new RsaKeyPair();

        IActor issuer = Substitute.For<IActor>();
        issuer.Handle.Returns("rootCaActor");
        issuer.Name.Returns("Root CA Actor");

        IKeyManager keyManager = Substitute.For<IKeyManager>();
        keyManager.GetSigningKey(issuer).Returns(issuerKeyPair.PrivateKey);

        When.A<CertificateManager>("creates a root CA certificate",
            () =>
            {
                ProfileSchemaRepository repo = CreateRepository(nameof(CreateRootCACertificate));
                repo.Save(new PublicKeySetData
                {
                    KeySetHandle = "rootCaActor",
                    PublicRsaKey = issuerKeyPair.PublicPem,
                });

                CertificateManager certManager = new CertificateManager(repo, null!);
                CertificateAuthority ca = CreateCertificateAuthority(issuer, keyManager, certManager);
                return new CertificateManager(repo, ca);
            },
            (certManager) =>
            {
                X509Certificate cert = certManager.CreateRootCACertificate(issuer);
                return cert;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
            X509Certificate cert = (X509Certificate)because.Result;
            because.ItsTrue("subject contains actor name",
                cert.SubjectDN.ToString().Contains("Root CA Actor"));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void CreateAndLoadRootCACertificate()
    {
        RsaKeyPair issuerKeyPair = new RsaKeyPair();

        IActor issuer = Substitute.For<IActor>();
        issuer.Handle.Returns("loadCaActor");
        issuer.Name.Returns("Load CA Actor");

        IKeyManager keyManager = Substitute.For<IKeyManager>();
        keyManager.GetSigningKey(issuer).Returns(issuerKeyPair.PrivateKey);

        When.A<CertificateManager>("creates then loads a root CA certificate",
            () =>
            {
                ProfileSchemaRepository repo = CreateRepository(nameof(CreateAndLoadRootCACertificate));
                repo.Save(new PublicKeySetData
                {
                    KeySetHandle = "loadCaActor",
                    PublicRsaKey = issuerKeyPair.PublicPem,
                });

                CertificateManager certManager = new CertificateManager(repo, null!);
                CertificateAuthority ca = CreateCertificateAuthority(issuer, keyManager, certManager);
                return new CertificateManager(repo, ca);
            },
            (certManager) =>
            {
                X509Certificate created = certManager.CreateRootCACertificate(issuer);
                X509Certificate loaded = certManager.LoadRootCACertificate(issuer);
                return new object[] { created, loaded };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            X509Certificate created = (X509Certificate)results[0];
            X509Certificate loaded = (X509Certificate)results[1];
            because.ItsTrue("loaded is not null", loaded != null);
            because.ItsTrue("loaded subject matches created subject",
                loaded.SubjectDN.ToString() == created.SubjectDN.ToString());
            because.ItsTrue("loaded serial matches created serial",
                loaded.SerialNumber.Equals(created.SerialNumber));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void CreateSignedCertificate()
    {
        RsaKeyPair issuerKeyPair = new RsaKeyPair();

        IActor issuer = Substitute.For<IActor>();
        issuer.Handle.Returns("signerActor");
        issuer.Name.Returns("Signer Actor");

        IActor subject = Substitute.For<IActor>();
        subject.Handle.Returns("signedActor");
        subject.Name.Returns("Signed Actor");

        RsaKeyPair subjectKeyPair = new RsaKeyPair();

        IKeyManager keyManager = Substitute.For<IKeyManager>();
        keyManager.GetSigningKey(issuer).Returns(issuerKeyPair.PrivateKey);

        When.A<CertificateManager>("creates a signed certificate",
            () =>
            {
                ProfileSchemaRepository repo = CreateRepository(nameof(CreateSignedCertificate));
                repo.Save(new PublicKeySetData
                {
                    KeySetHandle = "signedActor",
                    PublicRsaKey = subjectKeyPair.PublicPem,
                });

                CertificateManager certManager = new CertificateManager(repo, null!);
                CertificateAuthority ca = CreateCertificateAuthority(issuer, keyManager, certManager);
                return new CertificateManager(repo, ca);
            },
            (certManager) =>
            {
                X509Certificate cert = certManager.CreateSignedCertificate(subject);
                return cert;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
            X509Certificate cert = (X509Certificate)because.Result;
            because.ItsTrue("subject contains signed actor name",
                cert.SubjectDN.ToString().Contains("Signed Actor"));
            because.ItsTrue("issuer contains signer actor name",
                cert.IssuerDN.ToString().Contains("Signer Actor"));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void LoadReturnsNullWhenNoCertificateExists()
    {
        When.A<CertificateManager>("returns null for unknown actor",
            () =>
            {
                ProfileSchemaRepository repo = CreateRepository(nameof(LoadReturnsNullWhenNoCertificateExists));
                return new CertificateManager(repo, null!);
            },
            (certManager) =>
            {
                IActor actor = Substitute.For<IActor>();
                actor.Handle.Returns("unknownActor");
                X509Certificate cert = certManager.LoadRootCACertificate(actor);
                return cert;
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("result is null", because.Result == null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ThrowsWhenActorHasNoPublicKey()
    {
        When.A<CertificateManager>("throws when actor has no RSA key",
            () =>
            {
                ProfileSchemaRepository repo = CreateRepository(nameof(ThrowsWhenActorHasNoPublicKey));

                IActor issuer = Substitute.For<IActor>();
                issuer.Handle.Returns("noKeyIssuer");
                issuer.Name.Returns("No Key Issuer");

                IKeyManager keyManager = Substitute.For<IKeyManager>();
                CertificateManager certManager = new CertificateManager(repo, null!);
                CertificateAuthority ca = CreateCertificateAuthority(issuer, keyManager, certManager);
                return new CertificateManager(repo, ca);
            },
            (certManager) =>
            {
                IActor actor = Substitute.For<IActor>();
                actor.Handle.Returns("noKeyActor");
                try
                {
                    certManager.CreateRootCACertificate(actor);
                    return false;
                }
                catch (InvalidOperationException)
                {
                    return true;
                }
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("threw InvalidOperationException", (bool)because.Result);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
