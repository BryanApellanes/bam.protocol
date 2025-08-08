using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class MailingAddressData : KeyedAuditRepoData
{
    public string Address { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }

    [CompositeKey]
    public string Handle { get; set; }
}