using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class DeviceAdditionalProperties : KeyedAuditRepoData
{
    public string DeviceHandle { get; set; } = null!;
    public string AdditionalPropertyHandle { get; set; } = null!;
}