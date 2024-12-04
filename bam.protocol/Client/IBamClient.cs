using Bam.Server;

namespace Bam.Protocol.Client
{
    public interface IBamClient
    {
         HostBinding BaseAddress { get; set; }
         IBamClientRequestBuilder CreateRequestBuilder(BamClientProtocols protocol);
         Task<IBamClientResponse> ReceiveResponseAsync(IBamClientRequest request);
    }
}
