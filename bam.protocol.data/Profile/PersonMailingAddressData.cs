using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class PersonMailingAddressData : RepoData
{
    [CompositeKey]
    public string PersonHandle { get; set; }
    
    [CompositeKey]
    public string MailingAddressHandle { get; set; }
}