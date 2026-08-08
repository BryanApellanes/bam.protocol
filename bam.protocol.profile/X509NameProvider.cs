using Bam.Protocol.Data;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;

namespace Bam.Protocol.Profile;

public class X509NameProvider : IX509NameProvider
{
    private readonly List<Rdn> _rdns = new List<Rdn>();

    public X509NameProvider AddOU(string ou)
    {
        _rdns.Add(new Rdn(X509Name.OU, StripPrefix(ou, "OU=")));
        return this;
    }

    public X509NameProvider AddOu(string ou, int at)
    {
        _rdns.Insert(at, new Rdn(X509Name.OU, StripPrefix(ou, "OU=")));
        return this;
    }

    public X509NameProvider AddOrg(string org)
    {
        _rdns.Add(new Rdn(X509Name.O, StripPrefix(org, "O=")));
        return this;
    }

    public X509NameProvider AddOrg(string org, int at)
    {
        // This overload historically emits an OU RDN; the O-vs-OU discrepancy is a separate
        // pre-existing defect tracked in bam.protocol#4 and is intentionally not changed here.
        _rdns.Insert(at, new Rdn(X509Name.OU, StripPrefix(org, "OU=")));
        return this;
    }

    public X509NameProvider AddCountry(string country)
    {
        _rdns.Add(new Rdn(X509Name.C, StripPrefix(country, "C=")));
        return this;
    }

    public X509NameProvider AddCountry(string country, int at)
    {
        _rdns.Insert(at, new Rdn(X509Name.C, StripPrefix(country, "C=")));
        return this;
    }

    public X509Name GetName(IActor actor)
    {
        return GetName(actor.Name);
    }

    public X509Name GetName(string subjectName)
    {
        // Build the distinguished name structurally from an ordered OID/value pair list so
        // BouncyCastle DER-encodes each value in its own RDN. A subject name containing DN
        // metacharacters (e.g. "svc,OU=Admins") is carried as a single CN value rather than
        // injecting additional RDNs, which a comma-joined string form would allow.
        List<DerObjectIdentifier> oids = new List<DerObjectIdentifier>();
        List<string> values = new List<string>();

        oids.Add(X509Name.CN);
        values.Add(subjectName);
        foreach (Rdn rdn in _rdns)
        {
            oids.Add(rdn.Oid);
            values.Add(rdn.Value);
        }

        return new X509Name(oids, values);
    }

    private static string StripPrefix(string value, string prefix)
    {
        return value.StartsWith(prefix) ? value.Substring(prefix.Length) : value;
    }

    static IX509NameProvider _x509NameProvider = null!;
    private static object _currentLock = new object();
    public static IX509NameProvider Current
    {
        get
        {
            return _currentLock.DoubleCheckLock(ref _x509NameProvider, () => new X509NameProvider());
        }
        set
        {
            _x509NameProvider = value;
        }
    }

    private sealed record Rdn(DerObjectIdentifier Oid, string Value);
}
