using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Private;

public class RsaPrivateKeyData : RepoData
{
    public string Pem { get; set; }
    public string PublicKeyHash { get; set; }
    public string PublicKeyHashAlgorithm { get; set; }
}