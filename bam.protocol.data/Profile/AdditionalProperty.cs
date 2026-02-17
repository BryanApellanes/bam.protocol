using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class AdditionalProperty : KeyedAuditRepoData
{
    [CompositeKey]
    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;

    private string _handle = null!;

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