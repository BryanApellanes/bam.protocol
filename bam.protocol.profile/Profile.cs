using Bam.Encryption;
using Bam.Protocol.Data;

namespace Bam.Protocol.Profile;

public class Profile : IProfile
{
    public string ProfileHandle { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool ShowEmail { get; set; }
    public bool ShowPhone { get; set; }
    public string MailingAddressHandles { get; set; } = null!;
    public string DeviceHandle { get; set; } = null!;
    public string PersonHandle { get; set; } = null!;
}