
using Bam.Protocol.Data;
using Org.BouncyCastle.Asn1.X509;
using ZstdSharp.Unsafe;

namespace Bam.Protocol.Profile;

public class X509NameProvider : IX509NameProvider
{
    private List<string> _rdns = new List<string>();

    public X509NameProvider AddOU(string ou)
    {
        string val = ou.StartsWith("OU=") ? ou.Substring("OU=".Length) : ou;
        _rdns.Add($"OU={val}");
        return this;
    }

    public X509NameProvider AddOu(string ou, int at)
    {
        string val = ou.StartsWith("OU=") ? ou.Substring("OU=".Length) : ou;
        _rdns.Insert(at, $"OU={val}");
        return this; 
    }
    
    public X509NameProvider AddOrg(string org)
    {
        string val = org.StartsWith("O=") ? org.Substring("O=".Length) : org;
        _rdns.Add($"O={val}");
        return this;
    }

    public X509NameProvider AddOrg(string org, int at)
    {
        string val = org.StartsWith("OU=") ? org.Substring("OU=".Length) : org;
        _rdns.Insert(at, $"OU={val}");
        return this;
    }
    
    public X509NameProvider AddCountry(string country)
    {
        string val = country.StartsWith("C=") ? country.Substring("C=".Length) : country;
        _rdns.Add($"C={val}");
        return this;
    }

    public X509NameProvider AddCountry(string country, int at)
    {
        string val = country.StartsWith("C=") ? country.Substring("C=".Length) : country;
        _rdns.Insert(at, $"C={val}");
        return this;
    }
    
    public X509Name GetName(IActor actor)
    {
        return GetName(actor.Name);
    }

    public X509Name GetName(string subjectName)
    {
        List<string> segments = new List<string>();
        segments.Add($"CN={subjectName}");
        segments.AddRange(_rdns);
        return new X509Name(string.Join(",", segments.ToArray()));
    }

    static IX509NameProvider _x509NameProvider = null;
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
}