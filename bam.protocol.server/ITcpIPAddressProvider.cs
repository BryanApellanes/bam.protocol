using System.Net;

namespace Bam.Protocol.Server;

/// <summary>
/// Defines a provider that supplies a TCP-specific IP address for server binding.
/// </summary>
public interface ITcpIPAddressProvider : IIPAddressProvider
{
    /// <summary>
    /// Gets the IP address to use for TCP connections.
    /// </summary>
    /// <returns>The TCP IP address.</returns>
    IPAddress GetTcpIPAddress();
}