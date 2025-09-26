using Bam.Server;

namespace Bam.Protocol.Client;

public class BamServiceClient : BamClient, IBamServiceClient
{
    public BamServiceClient(IObjectEncoderDecoder objectEncoderDecoder) : base(objectEncoderDecoder)
    {
    }

    public BamServiceClient(IObjectEncoderDecoder objectEncoderDecoder, HostBinding httpBaseAddress) : base(objectEncoderDecoder, httpBaseAddress)
    {
    }

    public BamServiceClient(IObjectEncoderDecoder objectEncoderDecoder, HostBinding httpBaseAddress, HostBinding tcpBaseAddress) : base(objectEncoderDecoder, httpBaseAddress, tcpBaseAddress)
    {
    }

    public BamServiceClient(IObjectEncoderDecoder objectEncoderDecoder, HostBinding httpBaseAddress, HostBinding tcpBaseAddress, HostBinding udpBaseAddress) : base(objectEncoderDecoder, httpBaseAddress, tcpBaseAddress, udpBaseAddress)
    {
    }

    public StartSessionResponse StartSession(string host, int port)
    {
        throw new NotImplementedException();
    }
}