using System.Net;

namespace Bam.Protocol.Server;

/// <summary>
/// Provides UDP-specific IP address configuration for BAM server bindings.
/// </summary>
public class BamUdpIPAddressProvider : BamIPAddressProvider, IUdpIPAddressProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamUdpIPAddressProvider"/> class bound to <see cref="IPAddress.Any"/>.
    /// </summary>
    public BamUdpIPAddressProvider()
    {
        this._ipAddress = IPAddress.Any;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BamUdpIPAddressProvider"/> class with the specified IP address string.
    /// </summary>
    /// <param name="ipAddress">The IP address string to parse.</param>
    public BamUdpIPAddressProvider(string ipAddress) : this(IPAddress.Parse(ipAddress))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BamUdpIPAddressProvider"/> class with the specified IP address.
    /// </summary>
    /// <param name="ipAddress">The IP address to bind to.</param>
    public BamUdpIPAddressProvider(IPAddress ipAddress)
    {
        this._ipAddress = ipAddress;
    }

    private readonly IPAddress _ipAddress;
    /// <summary>
    /// Gets the UDP IP address to bind to.
    /// </summary>
    /// <returns>The configured UDP IP address.</returns>
    public virtual IPAddress GetUdpIPAddress()
    {
        return _ipAddress;
    }
}