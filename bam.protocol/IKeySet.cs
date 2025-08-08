namespace Bam.Protocol;

public interface IKeySet
{
    string KeySetHandle { get; set; }
    string PublicRsaKey { get; set; }
    string PublicEccKey { get; set; }
}