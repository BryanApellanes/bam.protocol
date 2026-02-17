using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class AgentCertificateData : RepoData
{
    [CompositeKey]
    public string AgentHandle { get; set; } = null!;

    [CompositeKey]
    public string CertificateHash { get; set; } = null!;
    public string CertificateHashAlgorithm { get; set; } = null!;
}