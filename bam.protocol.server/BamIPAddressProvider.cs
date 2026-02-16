using System.Net;

namespace Bam.Protocol.Server;

/// <summary>
/// Provides a default IP address (IPAddress.Any) for BAM server bindings.
/// </summary>
public class BamIPAddressProvider: IIPAddressProvider
{
    /// <summary>
    /// Gets the IP address to bind to. Returns <see cref="IPAddress.Any"/>.
    /// </summary>
    /// <returns>The IP address to bind to.</returns>
    public IPAddress GetIPAddress()
    {
        return IPAddress.Any;
    }
}