using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class DeviceAdditionalProperties : KeyedAuditRepoData
{
    public string DeviceHandle { get; set; }
    public string AdditionalPropertyHandle { get; set; }
}