using Bam.Server;
using Bam.Protocol.Server;

namespace Bam.Protocol;

public class BamHostBinding : HostBinding
{
    public BamHostBinding()
    {
        Protocol = "bam";
        _port = BamServer.DefaultTcpPort;
    }

    public BamHostBinding(string host, int port = BamServer.DefaultTcpPort) : base(host, port)
    {
        Protocol = "bam";
        _port = port;
    }

    public BamHostBinding(BamServerBuilder builder, HostBinding hostBinding)
    {
        Protocol = "bam";
        _port = builder.TcpPort();
        HostName = hostBinding.HostName;
    }
    
    public string Protocol { get; private set; }

    public override string ToString()
    {
        return $"{Protocol}://{HostName}:{Port}/";
    }

    public override bool Equals(object obj)
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

    public override int GetHashCode()
    {
        return this.ToString().GetHashCode();
    }
}