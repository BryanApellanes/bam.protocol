using Org.BouncyCastle.Asn1;

namespace Bam.Protocol.Profile;

public abstract class OidProvider : IOidProvider
{
    public const string IANA_ASSIGNED_PATH = "1.3.6.1.4.1";

    protected string GetIdentifier(params ulong[] subPathIdentifiers)
    {
        string suffix = string.Empty;
        if (subPathIdentifiers.Length > 0)
        {
            suffix = $".{string.Join(".", subPathIdentifiers)}";
        }

        return $"{IANA_ASSIGNED_PATH}.{GetPen()}" + suffix;
    }
    
    public abstract string GetPen();
    
    public DerObjectIdentifier GetObjectIdentifier()
    {
        return new DerObjectIdentifier(GetIdentifier());
    }
}