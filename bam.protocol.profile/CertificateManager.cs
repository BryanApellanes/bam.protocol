using Bam.Encryption;
using Bam.Protocol.Data;
using Bam.Protocol.Data.Profile;
using Bam.Protocol.Profile;
using Org.BouncyCastle.X509;

namespace Bam.Protocol;

public class CertificateManager : ICertificateManager
{
    public CertificateManager(IProfileRepository repository, CertificateAuthority certificateAuthority)
    {
        this.Repository = repository;
        this.CertificateAuthority = certificateAuthority;
    }

    protected IProfileRepository Repository { get; }
    protected CertificateAuthority CertificateAuthority { get; }

    public X509Certificate LoadRootCACertificate(IActor actor)
    {
        AgentCertificateData agentCert = Repository.FindAgentCertificateByHandle(actor.Handle);
        if (agentCert == null)
        {
            return null;
        }

        CertificateData certData = Repository.FindCertificateByHash(agentCert.CertificateHash);
        if (certData == null)
        {
            return null;
        }

        return ParseCertificate(certData.Pem);
    }

    public X509Certificate CreateRootCACertificate(IActor actor)
    {
        X509Certificate certificate = CertificateAuthority.CreateCertificate(actor.Name, new RsaPublicKey(GetOrCreatePublicKey(actor)));

        return SaveCertificate(actor, certificate);
    }

    public X509Certificate CreateSignedCertificate(IActor actor)
    {
        X509Certificate certificate = CertificateAuthority.CreateCertificate(actor.Name, new RsaPublicKey(GetOrCreatePublicKey(actor)));

        return SaveCertificate(actor, certificate);
    }

    private string GetOrCreatePublicKey(IActor actor)
    {
        PublicKeySetData keySet = Repository.FindPublicKeySetByHandle(actor.Handle);
        if (keySet != null && !string.IsNullOrEmpty(keySet.PublicRsaKey))
        {
            return keySet.PublicRsaKey;
        }

        throw new InvalidOperationException($"Actor '{actor.Handle}' has no public RSA key. Register a key set before creating certificates.");
    }

    private X509Certificate SaveCertificate(IActor actor, X509Certificate certificate)
    {
        string pem = certificate.ObjectToPem();
        string hash = pem.Sha256();

        CertificateData certData = new CertificateData
        {
            Pem = pem,
            Hash = hash,
            HashAlgorithm = "SHA256",
        };
        Repository.SaveCertificate(certData);

        AgentCertificateData agentCert = new AgentCertificateData
        {
            AgentHandle = actor.Handle,
            CertificateHash = hash,
            CertificateHashAlgorithm = "SHA256",
        };
        Repository.SaveAgentCertificate(agentCert);

        return certificate;
    }

    private static X509Certificate ParseCertificate(string pem)
    {
        X509CertificateParser parser = new X509CertificateParser();
        return parser.ReadCertificate(System.Text.Encoding.UTF8.GetBytes(pem));
    }
}
