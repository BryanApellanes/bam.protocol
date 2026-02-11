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

        public TR Invoke<TR>(string methodName, params object[] args)
        {
            return InvokeAsync<TR>(methodName, args).GetAwaiter().GetResult();
        }

        public async Task<TR> InvokeAsync<TR>(string methodName, params object[] args)
        {
            MethodInvocationRequest invocation = MethodInvocationRequest.For(typeof(T), methodName, args);
            invocation.OperationIdentifier = OperationIdentifier.For(typeof(T), methodName);
            string body = JsonConvert.SerializeObject(invocation);

            IBamClientRequest request = CreateRequestBuilder(BamClientProtocols.Http)
                .Path("/invoke")
                .HttpMethod(HttpMethods.POST)
                .Content(body)
                .Build();

            IBamClientResponse response = await ReceiveResponseAsync(request);

            if (response.StatusCode != 200)
            {
                throw new BamInvocationException(typeof(T), methodName, response.StatusCode, response.Content);
            }

            return JsonConvert.DeserializeObject<TR>(response.Content);
        }
    }
}
