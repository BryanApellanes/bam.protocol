using Bam.Encryption;
using Bam.Protocol.Data;

namespace Bam.Protocol.Profile;

public class Profile : IProfile
{
    public string ProfileHandle { get; set; }
    public string Name { get; set; }
    public bool ShowEmail { get; set; }
    public bool ShowPhone { get; set; }
    public string MailingAddressHandles { get; set; }
    public string DeviceHandle { get; set; }
    public string PersonHandle { get; set; }
}