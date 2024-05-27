using Bam;

namespace Bam.Protocol;

public static class StringExtensions
{
    public static int GetUnprivilegedPortForName(this string serverName)
    {
        return serverName.ToHashIntBetween(HashAlgorithms.SHA256, 1024, 65535);
    }
}