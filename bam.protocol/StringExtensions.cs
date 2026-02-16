namespace Bam.Protocol;

/// <summary>
/// Provides extension methods for string operations in the Bam protocol.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Derives a deterministic, unprivileged port number (1024-65535) from a server name using SHA256 hashing.
    /// </summary>
    /// <param name="serverName">The server name to derive a port number from.</param>
    /// <returns>A port number between 1024 and 65535.</returns>
    public static int GetUnprivilegedPortForName(this string serverName)
    {
        return serverName.ToHashIntBetween(HashAlgorithms.SHA256, 1024, 65535);
    }
}