using System;
using System.IO;
using System.Net.Sockets;
using Bam.Logging;

namespace Bam.Protocol.Server;

public class BamContextProvider : Loggable, IBamContextProvider
{
    public BamContextProvider(IBamRequestReader requestReader, IBamResponseProvider responseProvider, IBamActorResolver actorResolver)
    {
        this.RequestReader = requestReader;
        this.ResponseProvider = responseProvider;
        this.ActorResolver = actorResolver;
    }
    protected IBamRequestReader RequestReader { get; private set; }
    protected  IBamResponseProvider ResponseProvider { get; private set; }
    protected  IBamActorResolver ActorResolver { get; private set; }

    [Verbosity(VerbosityLevel.Information, MessageFormat = $"EventName={nameof(ReadRequestStarted)};LoggableIdentifier: {{LoggableIdentifier}}")]
    public event EventHandler ReadRequestStarted;
    
    [Verbosity(VerbosityLevel.Information, MessageFormat = $"EventName={nameof(ResolveUserStarted)};LoggableIdentifier: {{LoggableIdentifier}}")]
    public event EventHandler ResolveUserStarted;

    public IBamServerContext CreateContext(TcpClient client, string requestId)
    {
        IBamRequest request = RequestReader.ReadRequest(client);
        return new BamServerContext
        {
            RequestId = requestId,
            BamRequest = request,
            //BamResponse = new BamResponse(stream)
        };
    }
    
    public IBamServerContext CreateContext(Stream stream, string requestId)
    {
        IBamRequest request = RequestReader.ReadRequest(stream);
        return new BamServerContext
        {
            RequestId = requestId,
            BamRequest = request,
            BamResponse = new BamResponse(stream)
        };
    }
}