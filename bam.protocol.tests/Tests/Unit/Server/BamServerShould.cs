using System.Text;
using Bam;
using Bam.Console;
using Bam.CoreServices;
using Bam.Protocol.Server;
using Bam.Server;
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
        //server.DefaultHostBinding.ShouldNotBeNull();
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
    public void FireServerRequestEventsForTcpRequest()
    {
        
    }
}