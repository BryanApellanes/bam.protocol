using Bam.DependencyInjection;

namespace Bam.Protocol.Server;

public class BamRequestProcessor : IBamRequestProcessor
{
    public BamRequestProcessor(IObjectEncoderDecoder encoderDecoder, ServiceRegistry serviceRegistry)
    {
        this.EncoderDecoder = encoderDecoder;
        this.ServiceRegistry = serviceRegistry;
    }

    protected IObjectEncoderDecoder EncoderDecoder { get; }
    protected ServiceRegistry ServiceRegistry { get; }

    public object ProcessRequestContext(IBamServerContext serverContext)
    {
        string content = serverContext.BamRequest.Content;
        MethodInvocationRequest invocation = Newtonsoft.Json.JsonConvert.DeserializeObject<MethodInvocationRequest>(content);
        invocation.ServerInitialize(ServiceRegistry);
        return invocation.Invoke();
    }
}
