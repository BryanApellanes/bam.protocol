using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class CertificateData : KeyedAuditRepoData
{
    [CompositeKey]
    public string Hash { get; set; } = null!;

    public string HashAlgorithm { get; set; } = null!;

    [CompositeKey]
    public string Pem { get; set; } = null!;
}