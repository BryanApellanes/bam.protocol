using Bam.Encryption;

namespace Bam.Protocol.Data;

public interface IProfile
{
    string ProfileHandle { get; }
    string Name { get; }
    bool ShowEmail { get; }
    bool ShowPhone { get; }
    string MailingAddressHandles { get; }
    
    string DeviceHandle { get; }
    string PersonHandle { get; }
}