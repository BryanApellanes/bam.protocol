using Bam.Protocol.Profile;
using Bam.Test;
using Org.BouncyCastle.Asn1.X509;

namespace Bam.Protocol.Tests.Unit.Profile;

[UnitTestMenu("X509NameProvider Should", Selector = "x509np")]
public class X509NameProviderShould : UnitTestMenuContainer
{
    [UnitTest]
    public void NotAllowSubjectNameToInjectAdditionalRdns()
    {
        // A subject name carrying DN metacharacters must be encoded as a single CN value, not
        // parsed into extra RDNs. With the previous string-concatenation form, "svc,OU=Admins"
        // injected a second OU RDN; the structural construction keeps it inside one CN.
        string hostileSubject = "svc,OU=Admins";

        When.A<BamX509NameProvider>("builds a distinguished name from a hostile subject",
            () => new BamX509NameProvider(),
            (provider) =>
            {
                X509Name name = provider.GetName(hostileSubject);
                IList<string> commonNames = name.GetValueList(X509Name.CN);
                IList<string> organizationalUnits = name.GetValueList(X509Name.OU);
                return new DnInjectionOutcome(
                    commonNames.Count,
                    commonNames.Count == 1 ? commonNames[0] : string.Empty,
                    organizationalUnits.Count);
            })
        .TheTest
        .ShouldPass<DnInjectionOutcome>((because, outcome) =>
        {
            because.ItsTrue("exactly one CN RDN is present", outcome.CommonNameCount == 1);
            because.ItsTrue("the CN value is the literal subject name, commas and all", outcome.CommonNameValue == hostileSubject);
            because.ItsTrue("no additional OU RDN was injected (only the provider's configured OU remains)", outcome.OrganizationalUnitCount == 1);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private sealed record DnInjectionOutcome(int CommonNameCount, string CommonNameValue, int OrganizationalUnitCount);
}
