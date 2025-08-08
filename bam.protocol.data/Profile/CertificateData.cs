using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class CertificateData : KeyedAuditRepoData
{
    [CompositeKey]
    public string Hash { get; set; }
    
    public string HashAlgorithm { get; set; }
    
    [CompositeKey]
    public string Pem { get; set; }
}