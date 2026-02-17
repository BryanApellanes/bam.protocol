using Bam.DependencyInjection;

namespace Bam.Protocol.Server;

/// <summary>
/// Default request processor that deserializes method invocation requests and executes them.
/// </summary>
public class BamRequestProcessor : IBamRequestProcessor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BamRequestProcessor"/> class.
    /// </summary>
    /// <param name="encoderDecoder">The object encoder/decoder for deserialization.</param>
    /// <param name="serviceRegistry">The service registry for resolving invocation dependencies.</param>
    public BamRequestProcessor(IObjectEncoderDecoder encoderDecoder, ServiceRegistry serviceRegistry)
    {
        this.EncoderDecoder = encoderDecoder;
        this.ServiceRegistry = serviceRegistry;
    }

    protected IObjectEncoderDecoder EncoderDecoder { get; }
    protected ServiceRegistry ServiceRegistry { get; }

    /// <summary>
    /// Processes the request by deserializing the content as a method invocation and executing it.
    /// </summary>
    /// <param name="serverContext">The server context containing the request to process.</param>
    /// <returns>The result of the method invocation.</returns>
    public object ProcessRequestContext(IBamServerContext serverContext)
    {
        string content = serverContext.BamRequest.Content;
        MethodInvocationRequest invocation = Newtonsoft.Json.JsonConvert.DeserializeObject<MethodInvocationRequest>(content)!;
        invocation!.ServerInitialize(ServiceRegistry);
        return invocation.Invoke();
    }
}
