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
            (client) =>
            {
                IBamClientRequest request = client.CreateHttpRequest(httpPath);
                return new object?[] { request is HttpClientRequest, request.Host, request.Path, request.QueryString, request.HttpMethod, request.Protocol, request.ProtocolVersion, request.Content };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object?[] r = (object?[])because.Result;
            because.ItsTrue("is HttpClientRequest", (bool)r[0]!);
            because.ItsTrue("Host equals default HTTP address", BamClient.DefaultHttpBaseAddress.Equals(r[1]));
            because.ItsTrue("Path equals expected", httpPath.Equals(r[2]));
            because.ItsTrue("QueryString is null or empty", string.IsNullOrEmpty((string?)r[3]));
            because.ItsTrue("HttpMethod equals GET", HttpMethods.GET.Equals(r[4]));
            because.ItsTrue("Protocol equals HTTP", "HTTP".Equals(r[5]));
            because.ItsTrue("ProtocolVersion equals 1.1", "1.1".Equals(r[6]));
            because.ItsTrue("Content is null", r[7] == null);
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
            (client) =>
            {
                IBamClientRequest request = client.CreateTcpRequest(tcpPath);
                return new object?[] { request is TcpClientRequest, request.Host, request.Path, request.QueryString, request.HttpMethod, request.Protocol, request.ProtocolVersion, request.Content };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object?[] r = (object?[])because.Result;
            because.ItsTrue("is TcpClientRequest", (bool)r[0]!);
            because.ItsTrue("Host equals default TCP address", BamClient.DefaultTcpBaseAddress.Equals(r[1]));
            because.ItsTrue("Path equals expected", tcpPath.Equals(r[2]));
            because.ItsTrue("QueryString is null or empty", string.IsNullOrEmpty((string?)r[3]));
            because.ItsTrue("HttpMethod equals POST", HttpMethods.POST.Equals(r[4]));
            because.ItsTrue("Protocol equals BAM", "BAM".Equals(r[5]));
            because.ItsTrue("ProtocolVersion equals 2.0", "2.0".Equals(r[6]));
            because.ItsTrue("Content is null", r[7] == null);
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
            (client) =>
            {
                IBamClientRequest request = client.CreateUdpRequest(udpPath, content);
                return new object?[] { request is UdpClientRequest, request.Host, request.Path, request.QueryString, request.HttpMethod, request.Protocol, request.ProtocolVersion, request.Content };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object?[] r = (object?[])because.Result;
            because.ItsTrue("is UdpClientRequest", (bool)r[0]!);
            because.ItsTrue("Host equals default UDP address", BamClient.DefaultUdpBaseAddress.Equals(r[1]));
            because.ItsTrue("Path equals expected", udpPath.Equals(r[2]));
            because.ItsTrue("QueryString is null or empty", string.IsNullOrEmpty((string?)r[3]));
            because.ItsTrue("HttpMethod equals PUT", HttpMethods.PUT.Equals(r[4]));
            because.ItsTrue("Protocol equals BAM", "BAM".Equals(r[5]));
            because.ItsTrue("ProtocolVersion equals 2.0", "2.0".Equals(r[6]));
            because.ItsTrue("Content equals expected", content.Equals(r[7]));
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
