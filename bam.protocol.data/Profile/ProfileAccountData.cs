using System.Text.Json.Serialization;
using Bam.Data.Repositories;
using Bam.Encryption;

namespace Bam.Protocol.Data.Profile;

public class ProfileAccountData : KeyedAuditRepoData
{
    public ProfileAccountData()
    {
        ProfileAccountHandle = 6.SecureAlphaNumericCharacters();
    }
    
    public virtual ulong PersonId { get; set; }
    
    public virtual IPerson Person { get; set; }
    
    public string PersonHandle { get; set; }
    
    /// <summary>
    /// Gets or sets a unique identifier for this profile.
    /// </summary>
    public string ProfileAccountHandle { get; set; }
    
    public string DisplayName { get; set; }
}