using System.Net;

namespace Bam.Protocol.Server;

/// <summary>
/// Provides TCP-specific IP address configuration for BAM server bindings.
/// </summary>
public class BamTcpIPAddressProvider : BamIPAddressProvider, ITcpIPAddressProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamTcpIPAddressProvider"/> class bound to <see cref="IPAddress.Any"/>.
    /// </summary>
    public BamTcpIPAddressProvider()
    {
        this._ipAddress = IPAddress.Any;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BamTcpIPAddressProvider"/> class with the specified IP address string.
    /// </summary>
    /// <param name="ipAddress">The IP address string to parse.</param>
    public BamTcpIPAddressProvider(string ipAddress) : this(IPAddress.Parse(ipAddress))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BamTcpIPAddressProvider"/> class with the specified IP address.
    /// </summary>
    /// <param name="ipAddress">The IP address to bind to.</param>
    public BamTcpIPAddressProvider(IPAddress ipAddress)
    {
        this._ipAddress = ipAddress;
    }

    private readonly IPAddress _ipAddress;
    /// <summary>
    /// Gets the TCP IP address to bind to.
    /// </summary>
    /// <returns>The configured TCP IP address.</returns>
    public virtual IPAddress GetTcpIPAddress()
    {
        return _ipAddress;
    }
}