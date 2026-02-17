using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Private;

public class EccPrivateKeyData : RepoData
{
    public string Pem { get; set; } = null!;
    public string PublicKeyHash { get; set; } = null!;
    public string PublicKeyHashAlgorithm { get; set; } = null!;
}