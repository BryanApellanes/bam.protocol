using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class DeviceCertificateData : RepoData
{
    [CompositeKey]
    public string DeviceHandle { get; set; }
    
    [CompositeKey]
    public string CertificateHash { get; set; }
    
    public string CertificateHashAlgorithm { get; set; }
}