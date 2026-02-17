using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;


public class PublicKeySetData : RepoData, IKeySet
{
    public string KeySetHandle { get; set; } = null!;

    public string PublicRsaKey { get; set; } = null!;
    public string PublicEccKey { get; set; } = null!;
}