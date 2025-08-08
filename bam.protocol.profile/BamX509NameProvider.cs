namespace Bam.Protocol.Profile;

public class BamX509NameProvider : X509NameProvider
{
    public BamX509NameProvider()
    {
        this.AddCountry("United States");
        this.AddOU("Public");
        this.AddOrg("Three Headz");   
    }
}