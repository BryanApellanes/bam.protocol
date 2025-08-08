using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;


public class PublicKeySetData : RepoData, IKeySet
{
    public string KeySetHandle { get; set; }
    
    public string PublicRsaKey { get; set; }
    public string PublicEccKey { get; set; }
}