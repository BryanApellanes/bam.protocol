using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class OrganizationCertificateData : RepoData
{
    [CompositeKey]
    public string OrganizationHandle { get; set; }
    
    [CompositeKey]
    public string CertificateHash { get; set; }
    
    public string CertificateHashAlgorithm { get; set; }
}