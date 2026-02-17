using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class DeviceCertificateData : RepoData
{
    [CompositeKey]
    public string DeviceHandle { get; set; } = null!;

    [CompositeKey]
    public string CertificateHash { get; set; } = null!;

    public string CertificateHashAlgorithm { get; set; } = null!;
}