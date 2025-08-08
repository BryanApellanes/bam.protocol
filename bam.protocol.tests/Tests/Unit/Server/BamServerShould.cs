using System.Text;
using Bam.Console;
using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Protocol.Client;
using Bam.Protocol.Server;
using Bam.Server;
using Bam.Services;
using Bam.Test;

namespace Bam.Protocol.Tests;

[UnitTestMenu("BamServer should")]
public class BamServerShould : UnitTestMenuContainer
{
    public BamServerShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public async Task TestHttpServer()
    {
        HttpServer server = new HttpServer(context =>
        {
            System.Console.WriteLine(context.Request.Url);
            byte[] output = Encoding.UTF8.GetBytes(context.Request.Url.ToString());
            context.Response.OutputStream.Write(output, 0, output.Length);
            context.Response.Close();
        });
        server.Start(new HostBinding("127.0.0.1", 8080){Ssl = false});
        HttpClient client = new HttpClient();
        HttpRequestMessage requestMessage = new HttpRequestMessage();
        requestMessage.RequestUri = new Uri("http://127.0.0.1:8080");
        await client.SendAsync(requestMessage);
    }
    
    [UnitTest]
    public async Task HaveDefaultHostBinding()
    {
        BamServer server = new BamServerBuilder().Build();
        server.HttpHostBinding.ShouldNotBeNull();
        Message.PrintLine(server.HttpHostBinding.ToString());
    }
    
    [UnitTest]
    public void FireServerStartAndStopEvents()
    {
        bool? startingEventRaised = false;
        bool? startedEventRaised = false;
        bool? stoppingEventRaised = false;
        bool? stoppedEventRaised = false;
        BamServer server = new BamServerBuilder()
            .OnStarting((sender, args) => startingEventRaised = true)
            .OnStarted((sender, args) => startedEventRaised = true)
            .OnStopping((sender, args) => stoppingEventRaised = true)
            .OnStopped((sender, args) => stoppedEventRaised = true)
            .Build();
        
        server.Start();
        server.Stop();
        
        startingEventRaised.ShouldBeTrue("`Starting` event was not raised");
        startedEventRaised.ShouldBeTrue("`Started` event was not raised");
        stoppingEventRaised.ShouldBeTrue("`Stopping` event was not raised");
        stoppedEventRaised.ShouldBeTrue("`Stopped` event was not raised");
    }
    
    [UnitTest]
    public async Task FireServerStartAndStopEventsAsync()
    {
        bool? startingEventRaised = false;
        bool? startedEventRaised = false;
        bool? stoppingEventRaised = false;
        bool? stoppedEventRaised = false;
        
        After.Setup(registry =>
        {
            registry.For<BamServer>()
                .Use<BamServer>(() =>
                {
                    return new BamServerBuilder()
                        .OnStarting((sender, args) => startingEventRaised = true)
                        .OnStarted((sender, args) => startedEventRaised = true)
                        .OnStopping((sender, args) => stoppingEventRaised = true)
                        .OnStopped((sender, args) => stoppedEventRaised = true)
                        .Build();
                });
        })
        .When<BamServer>("Starts and stops asynchronously", async (server, registry) =>
        {
            BamServerInfo info = server.GetInfo();
            Message.PrintLine(info.ToJson(true), ConsoleColor.Cyan);
            await server.StartAsync();
            await server.TryStopAsync();
        })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("Starting event was raised", startingEventRaised.Value, "Starting event was not raised");
            because.ItsTrue("Started event was raised", startedEventRaised.Value, "Started event was not raised");
            because.ItsTrue("Stopping event was raised", stoppingEventRaised.Value, "Stopping event was not raised");
            because.ItsTrue("Stopped event was raised", stoppedEventRaised.Value, "Stopped event was not raised");
            
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
    
    [UnitTest]
    public async Task StartSession()
    {
        BamServer server = new BamServer();
        BamServerInfo info = server.GetInfo();
        Message.PrintLine(info.ToJson(true), ConsoleColor.Cyan);
        await server.StartAsync();
        
        After.Setup((reg) =>
        {
            reg.For<BamClient>().Use(new BamClient(new JsonObjectDataEncoder(), info.HttpHostBinding));
        
        })
        .When<BamClient>("BamClient calls ReceiveResponseAsync", async (client) =>
        {
            string httpPath = "/test/http/path?q=unit";
            IBamClientRequest request = client.CreateHttpRequest(httpPath);
            IBamClientResponse response = await client.ReceiveResponseAsync(request);
            return response;
        })
        .It
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
            because.TheResult.Is<IBamClientResponse>();
            IBamClientResponse response = because.TheResult.As<IBamClientResponse>();
            because.ItsTrue("response is not null", response != null, "response is null");
            because.ItsTrue("status code was 400", response.StatusCode == 400, $"status code was NOT 400 but was {response.StatusCode}");
            because.IllLookAtIt(response.Content);
        })
        .SoBeHappy((reg) =>
        {
            server.Stop();
        })
        .UnlessItFailed();
    }
}