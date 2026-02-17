using Bam.Data.Repositories;

namespace Bam.Protocol.Data.Profile;

public class OrganizationMailingAddress : KeyedAuditRepoData
{
    [CompositeKey]
    public string OrganizationHandle { get; set; } = null!;

    [CompositeKey]
    public string MailingAddressHandle { get; set; } = null!;
}