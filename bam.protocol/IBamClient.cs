using Bam.Server;

namespace Bam.Protocol.Client
{
    /// <summary>
    /// Defines a client that communicates with a Bam server over HTTP, TCP, or UDP.
    /// </summary>
    public interface IBamClient
    {
         /// <summary>
         /// Gets or sets the default (TCP) base address.
         /// </summary>
         HostBinding BaseAddress { get; set; }

         /// <summary>
         /// Gets or sets the HTTP base address.
         /// </summary>
         HostBinding HttpBaseAddress { get; set; }

         /// <summary>
         /// Gets or sets the UDP base address.
         /// </summary>
         HostBinding UdpBaseAddress { get; set; }

         /// <summary>
         /// Creates a request builder for the specified transport protocol.
         /// </summary>
         /// <param name="protocol">The transport protocol to use.</param>
         /// <returns>A request builder configured for the specified protocol.</returns>
         IBamClientRequestBuilder CreateRequestBuilder(BamClientProtocols protocol);

         /// <summary>
         /// Sends the specified request and asynchronously receives the response.
         /// </summary>
         /// <param name="request">The client request to send.</param>
         /// <returns>The response from the server.</returns>
         Task<IBamClientResponse> ReceiveResponseAsync(IBamClientRequest request);
    }
}
