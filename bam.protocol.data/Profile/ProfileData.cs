using System.Text.Json.Serialization;
using Bam.Data.Repositories;
using Bam.Encryption;

namespace Bam.Protocol.Data.Profile;

public class ProfileData : KeyedAuditRepoData, IProfile
{
    public ProfileData()
    {
        ProfileHandle = 6.SecureAlphaNumericCharacters();
    }
    
    public virtual ulong PersonId { get; set; }
    
    public virtual IPerson Person { get; set; } = null!;

    public string PersonHandle { get; set; } = null!;
    
    /// <summary>
    /// Gets or sets a unique identifier for this profile.
    /// </summary>
    public string ProfileHandle { get; set; }
    
    /// <summary>
    /// Gets or sets the display name for this profile.
    /// For example, "personal profile" or "business profile". 
    /// </summary>
    public string Name { get; set; } = null!;

    public bool ShowFirstName { get; set; }
    public bool ShowLastName { get; set; }
    
    public bool ShowEmail { get; set; }
    public bool ShowPhone { get; set; }
    public string MailingAddressHandles { get; set; } = null!;
    public string DeviceHandle { get; set; } = null!;
}