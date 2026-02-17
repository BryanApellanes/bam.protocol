using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class MailingAddressData : KeyedAuditRepoData
{
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string Country { get; set; } = null!;

    [CompositeKey]
    public string Handle { get; set; } = null!;
}