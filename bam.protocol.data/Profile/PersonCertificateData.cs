using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class PersonCertificateData : RepoData
{
    [CompositeKey]
    public string PersonHandle { get; set; }
    
    [CompositeKey]
    public string CertificateHash { get; set; }
    public string CertificateHashAlgorithm { get; set; }
}