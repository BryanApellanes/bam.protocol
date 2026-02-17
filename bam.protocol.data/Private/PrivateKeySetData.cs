using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Data.Private;

public class PrivateKeySetData : PublicKeySetData
{
    public string PublicEccKeyHash { get; set; } = null!;
    public string PublicEccKeyHashAlgorithm { get; set; } = null!;

    public string PublicRsaKeyHash { get; set; } = null!;
    public string PublicRsaKeyHashAlgorithm { get; set; } = null!;
}