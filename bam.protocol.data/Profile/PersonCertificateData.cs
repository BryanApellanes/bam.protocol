using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class PersonCertificateData : RepoData
{
    [CompositeKey]
    public string PersonHandle { get; set; } = null!;

    [CompositeKey]
    public string CertificateHash { get; set; } = null!;
    public string CertificateHashAlgorithm { get; set; } = null!;
}