using System.Reflection;
using Bam.Console;
using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Protocol.Client;
using Bam.Protocol.Server;
using Bam.Services;
using bam.testing;
using Bam.Test;

namespace Bam.Protocol.Tests;

[UnitTestMenu("BamClient should", "bcls")]
public class BamClientShould : UnitTestMenuContainer
{
    public BamClientShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public void CreateHttpRequestBuilder()
    {
        When.A<BamClient>("creates an HTTP request builder",
            () => new BamClient(new JsonObjectDataEncoder()),
            (client) => client.CreateRequestBuilder(BamClientProtocols.Http))
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("is HttpBamClientRequestBuilder", because.Result is HttpBamClientRequestBuilder);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void CreateTcpRequestBuilder()
    {
        When.A<BamClient>("creates a TCP request builder",
            () => new BamClient(new JsonObjectDataEncoder()),
            (client) => client.CreateRequestBuilder(BamClientProtocols.Tcp))
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("is TcpBamClientRequestBuilder", because.Result is TcpBamClientRequestBuilder);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void CreateUdpRequestBuilder()
    {
        When.A<BamClient>("creates a UDP request builder",
            () => new BamClient(new JsonObjectDataEncoder()),
            (client) => client.CreateRequestBuilder(BamClientProtocols.Udp))
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("is UdpBamClientRequestBuilder", because.Result is UdpBamClientRequestBuilder);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void CreateHttpRequest()
    {
        string httpPath = "/test/http/path/";

        When.A<BamClient>("creates an HTTP request",
            () => new BamClient(new JsonObjectDataEncoder()),
            (client) => client.CreateHttpRequest(httpPath))
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .Is<HttpClientRequest>()
                .As<IBamClientRequest>("Host equals default HTTP address", r => BamClient.DefaultHttpBaseAddress.Equals(r.Host))
                .As<IBamClientRequest>("Path equals expected", r => httpPath.Equals(r.Path))
                .As<IBamClientRequest>("QueryString is null or empty", r => string.IsNullOrEmpty(r.QueryString))
                .As<IBamClientRequest>("HttpMethod equals GET", r => HttpMethods.GET.Equals(r.HttpMethod))
                .As<IBamClientRequest>("Protocol equals HTTP", r => "HTTP".Equals(r.Protocol))
                .As<IBamClientRequest>("ProtocolVersion equals 1.1", r => "1.1".Equals(r.ProtocolVersion))
                .As<IBamClientRequest>("Content is null", r => r.Content == null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void CreateTcpRequest()
    {
        string tcpPath = "/test/tcp/path";

        When.A<BamClient>("creates a TCP request",
            () => new BamClient(new JsonObjectDataEncoder()),
            (client) => client.CreateTcpRequest(tcpPath))
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .Is<TcpClientRequest>()
                .As<IBamClientRequest>("Host equals default TCP address", r => BamClient.DefaultTcpBaseAddress.Equals(r.Host))
                .As<IBamClientRequest>("Path equals expected", r => tcpPath.Equals(r.Path))
                .As<IBamClientRequest>("QueryString is null or empty", r => string.IsNullOrEmpty(r.QueryString))
                .As<IBamClientRequest>("HttpMethod equals POST", r => HttpMethods.POST.Equals(r.HttpMethod))
                .As<IBamClientRequest>("Protocol equals BAM", r => "BAM".Equals(r.Protocol))
                .As<IBamClientRequest>("ProtocolVersion equals 2.0", r => "2.0".Equals(r.ProtocolVersion))
                .As<IBamClientRequest>("Content is null", r => r.Content == null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void CreateUdpRequest()
    {
        string udpPath = "/test/udp/path";
        object content = "The content";

        When.A<BamClient>("creates a UDP request",
            () => new BamClient(new JsonObjectDataEncoder()),
            (client) => client.CreateUdpRequest(udpPath, content))
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .Is<UdpClientRequest>()
                .As<IBamClientRequest>("Host equals default UDP address", r => BamClient.DefaultUdpBaseAddress.Equals(r.Host))
                .As<IBamClientRequest>("Path equals expected", r => udpPath.Equals(r.Path))
                .As<IBamClientRequest>("QueryString is null or empty", r => string.IsNullOrEmpty(r.QueryString))
                .As<IBamClientRequest>("HttpMethod equals PUT", r => HttpMethods.PUT.Equals(r.HttpMethod))
                .As<IBamClientRequest>("Protocol equals BAM", r => "BAM".Equals(r.Protocol))
                .As<IBamClientRequest>("ProtocolVersion equals 2.0", r => "2.0".Equals(r.ProtocolVersion))
                .As<IBamClientRequest>("Content equals expected", r => content.Equals(r.Content));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task Receive400HttpResponseSessionRequired()
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
