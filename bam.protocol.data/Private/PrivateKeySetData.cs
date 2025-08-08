using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Data.Private;

public class PrivateKeySetData : PublicKeySetData
{
    public string PublicEccKeyHash { get; set; }
    public string PublicEccKeyHashAlgorithm { get; set; }
    
    public string PublicRsaKeyHash { get; set; }
    public string PublicRsaKeyHashAlgorithm { get; set; }
}