using System.Net;

namespace Bam.Protocol.Server;

/// <summary>
/// Defines a provider that supplies an IP address for server binding.
/// </summary>
public interface IIPAddressProvider
{
    /// <summary>
    /// Gets the IP address to bind to.
    /// </summary>
    /// <returns>The IP address.</returns>
    IPAddress GetIPAddress();
}