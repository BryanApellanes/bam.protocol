using System.Net;

namespace Bam.Protocol.Server;

/// <summary>
/// Defines a provider that supplies a UDP-specific IP address for server binding.
/// </summary>
public interface IUdpIPAddressProvider : IIPAddressProvider
{
    /// <summary>
    /// Gets the IP address to use for UDP connections.
    /// </summary>
    /// <returns>The UDP IP address.</returns>
    IPAddress GetUdpIPAddress();
}