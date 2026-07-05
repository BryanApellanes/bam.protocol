using Bam.Data.Objects;
using Bam.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Protocol.Client
{
    public class BamClient<T> : BamClient
    {
        public BamClient(HostBinding httpBaseAddress) : base(new JsonObjectDataEncoder(), httpBaseAddress)
        {
        }

        public BamClient(HostBinding httpBaseAddress, HostBinding tcpBaseAddress) : base(new JsonObjectDataEncoder(), httpBaseAddress, tcpBaseAddress)
        {
        }

        public BamClient(HostBinding httpBaseAddress, HostBinding tcpBaseAddress, HostBinding udpBaseAddress) : base(new JsonObjectDataEncoder(), httpBaseAddress, tcpBaseAddress, udpBaseAddress)
        {
        }

        public TR Invoke<TR>(string methodName, params object[] args)
        {
            return Invoke<TR>(BamClientProtocols.Http, methodName, args);
        }

        public TR Invoke<TR>(BamClientProtocols protocol, string methodName, params object[] args)
        {
            return InvokeAsync<TR>(protocol, methodName, args).GetAwaiter().GetResult();
        }

        public Task<TR> InvokeAsync<TR>(string methodName, params object[] args)
        {
            return InvokeAsync<TR>(BamClientProtocols.Http, methodName, args);
        }

        public async Task<TR> InvokeAsync<TR>(BamClientProtocols protocol, string methodName, params object[] args)
        {
            MethodInvocationRequest invocation = MethodInvocationRequest.For(typeof(T), methodName, args);
            // ClientInitialize sets the OperationIdentifier and captures any instance context.
            // The invocation object (not a pre-serialized string) is handed to the builder so the
            // client's ObjectEncoderDecoder produces a body the server can decode back into a
            // MethodInvocationRequest — matching the raw request-building path used by the transport tests.
            invocation.ClientInitialize();

            IBamClientRequest request = CreateRequestBuilder(protocol)
                .Path("/invoke")
                .HttpMethod(HttpMethods.POST)
                .Content(invocation)
                .Build();

            IBamClientResponse response = await ReceiveResponseAsync(request);

            if (response.StatusCode != 200)
            {
                throw new BamInvocationException(typeof(T), methodName, response.StatusCode, response.Content);
            }

            // Body strips any BAM wire framing (status line/headers) so deserialization works across transports.
            return JsonConvert.DeserializeObject<TR>(response.Body)!;
        }
    }
}
