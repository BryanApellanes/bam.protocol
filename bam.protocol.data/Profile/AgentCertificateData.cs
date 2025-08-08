using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class AgentCertificateData : RepoData
{
    [CompositeKey]
    public string AgentHandle { get; set; }
    
    [CompositeKey]
    public string CertificateHash { get; set; }
    public string CertificateHashAlgorithm { get; set; }
}