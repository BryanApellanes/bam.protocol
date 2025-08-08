using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class AdditionalProperty : KeyedAuditRepoData
{
    [CompositeKey]
    public string Name { get; set; }
    
    public string Value { get; set; }

    private string _handle;

    public string Handle
    {
        get
        {
            if (string.IsNullOrEmpty(_handle))
            {
                _handle = $"{Name}.{Value}".Sha256();
            }

            return _handle;
        }
        set => _handle = value;
    }
}