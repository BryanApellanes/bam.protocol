using Bam.Server;
using Bam.Protocol.Server;

namespace Bam.Protocol;

/// <summary>
/// Represents a BAM protocol host binding with protocol, hostname, and port.
/// </summary>
public class BamHostBinding : HostBinding
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamHostBinding"/> class with default settings.
    /// </summary>
    public BamHostBinding()
    {
        Protocol = "bam";
        _port = BamServer.DefaultTcpPort;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BamHostBinding"/> class with the specified host and port.
    /// </summary>
    /// <param name="host">The hostname for this binding.</param>
    /// <param name="port">The port number. Defaults to the default TCP port.</param>
    public BamHostBinding(string host, int port = BamServer.DefaultTcpPort) : base(host, port)
    {
        Protocol = "bam";
        _port = port;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BamHostBinding"/> class from a server builder and existing host binding.
    /// </summary>
    /// <param name="builder">The server builder providing port configuration.</param>
    /// <param name="hostBinding">The host binding to copy the hostname from.</param>
    public BamHostBinding(BamServerBuilder builder, HostBinding hostBinding)
    {
        Protocol = "bam";
        _port = builder.TcpPort();
        HostName = hostBinding.HostName;
    }
    
    /// <summary>
    /// Gets the protocol identifier for this binding.
    /// </summary>
    public string Protocol { get; private set; }

    /// <summary>
    /// Returns the string representation of this host binding in the format "protocol://hostname:port".
    /// </summary>
    /// <returns>The formatted host binding string.</returns>
    public override string ToString()
    {
        return $"{Protocol}://{HostName}:{Port}";
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj is string stringValue)
        {
            return stringValue.Equals(ToString());
        }

        if (obj is HostBinding hostBinding)
        {
            return hostBinding.ToString().Equals(ToString());
        }

        return false;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return this.ToString().GetHashCode();
    }
}