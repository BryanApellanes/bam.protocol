namespace Bam.Protocol.Profile;

public struct ThreeHeadzPen
{
    public string Decimal { get; set; }
    public string Organization { get; set; }
    public string Contact { get; set; }
    public string Email { get; set; }

    public static ThreeHeadzPen Info => new ThreeHeadzPen()
    {
        Decimal = "64015",
        Organization = "Three Headz",
        Contact = "Bryan Apellanes",
        Email = "bryan@threeheadz.com"
    };
}