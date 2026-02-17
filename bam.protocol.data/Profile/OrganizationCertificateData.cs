using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class OrganizationCertificateData : RepoData
{
    [CompositeKey]
    public string OrganizationHandle { get; set; } = null!;

    [CompositeKey]
    public string CertificateHash { get; set; } = null!;

    public string CertificateHashAlgorithm { get; set; } = null!;
}