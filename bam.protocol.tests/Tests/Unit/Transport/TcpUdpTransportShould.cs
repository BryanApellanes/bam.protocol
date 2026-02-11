using System.Net;
using System.Net.Sockets;
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

[UnitTestMenu("TCP/UDP transport should", "tcpudp")]
public class TcpUdpTransportShould : UnitTestMenuContainer
{
    public TcpUdpTransportShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public void ParseBamProtocolRequestFromStream()
    {
        string requestBody = "This is the TCP/UDP request body";
        string requestStream = $@"GET bam://test.com/tcp/path BAM/2.0
Content-Type: application/json; charset=utf-8
X-Bam-Custom: custom-value

{requestBody}
";

        When.A<BamRequestReader>("parses BAM/2.0 request from stream",
            () => new BamRequestReader(new BamRequestReaderOptions(new BamServerOptions())),
            (reader) =>
            {
                MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(requestStream));
                IBamRequest bamRequest = reader.ReadRequest(stream);
                BamRequest concrete = (BamRequest)bamRequest;
                return new object[]
                {
                    bamRequest.Content,
                    bamRequest.Headers.ContainsKey("content-type"),
                    bamRequest.Headers.ContainsKey("x-bam-custom"),
                    concrete.Line?.Method.ToString(),
                    concrete.Line?.RequestUri,
                    concrete.Line?.ProtocolVersion
                };
            })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            because.ItsTrue("Content equals request body", requestBody.Equals(results[0]));
            because.ItsTrue("Headers contain content-type", (bool)results[1]);
            because.ItsTrue("Headers contain x-bam-custom", (bool)results[2]);
            because.ItsTrue("Method is GET", "GET".Equals(results[3]));
            because.ItsTrue("RequestUri is bam://test.com/tcp/path", "bam://test.com/tcp/path".Equals(results[4]));
            because.ItsTrue("ProtocolVersion is BAM/2.0", "BAM/2.0".Equals(results[5]));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void ReadRawTcpClientRequest()
    {
        string bamRequest = $@"GET bam://test.com/tcp/path BAM/2.0
Content-Type: application/json; charset=utf-8

test body
";
        int port = GetAvailablePort();

        When.A<BamRequestReader>("reads raw bytes from TcpClient without parsing headers",
            () => new BamRequestReader(new BamRequestReaderOptions(new BamServerOptions())),
            (reader) =>
            {
                TcpListener listener = new TcpListener(IPAddress.Loopback, port);
                listener.Start();
                try
                {
                    TcpClient sender = new TcpClient();
                    sender.Connect(IPAddress.Loopback, port);
                    NetworkStream senderStream = sender.GetStream();
                    byte[] data = Encoding.UTF8.GetBytes(bamRequest);
                    senderStream.Write(data, 0, data.Length);
                    senderStream.Flush();

                    TcpClient serverSide = listener.AcceptTcpClient();
                    Thread.Sleep(100); // allow data to arrive

                    IBamRequest result = reader.ReadRequest(serverSide);

                    sender.Close();
                    serverSide.Close();

                    BamRequest concrete = (BamRequest)result;
                    return new object?[]
                    {
                        result.Content,
                        result.Headers,
                        concrete.Line,
                        !string.IsNullOrEmpty(result.Content)
                    };
                }
                finally
                {
                    listener.Stop();
                }
            })
        .TheTest
        .ShouldPass(because =>
        {
            object?[] results = (object?[])because.Result;
            string content = (string)results[0]!;
            because.ItsTrue("Content is not empty", (bool)results[3]!);
            because.ItsTrue("Content contains raw request text (not parsed)", content.Contains("BAM/2.0"));
            because.ItsTrue("Headers is null (not parsed)", results[1] == null);
            because.ItsTrue("Line is null (not parsed)", results[2] == null);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task AcceptTcpConnectionAndCreateContext()
    {
        bool tcpClientConnected = false;

        BamServer server = new BamServerBuilder()
            .OnTcpClientConnected((sender, args) => tcpClientConnected = true)
            .Build();

        BamServerInfo info = server.GetInfo();
        Message.PrintLine($"TCP test server: TcpPort={info.TcpPort}", ConsoleColor.Cyan);

        await server.StartAsync();
        await Task.Delay(200);

        try
        {
            TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", info.TcpPort);
            NetworkStream stream = client.GetStream();
            string request = "GET bam://test.com/path BAM/2.0\n\n";
            byte[] data = Encoding.UTF8.GetBytes(request);
            stream.Write(data, 0, data.Length);
            stream.Flush();

            await Task.Delay(500);
            client.Close();
        }
        finally
        {
            server.Stop();
        }

        When.A<bool>("server fires TcpClientConnected event",
            () => tcpClientConnected,
            (result) => result)
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("TcpClientConnected event was fired", (bool)because.Result);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task TcpServerDoesNotSendResponse()
    {
        BamServer server = new BamServerBuilder().Build();
        BamServerInfo info = server.GetInfo();
        Message.PrintLine($"TCP no-response test: TcpPort={info.TcpPort}", ConsoleColor.Cyan);

        await server.StartAsync();
        await Task.Delay(200);

        int bytesAvailable = -1;
        bool readTimedOut = false;

        try
        {
            TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", info.TcpPort);
            NetworkStream stream = client.GetStream();

            string request = "GET bam://test.com/path BAM/2.0\nContent-Type: application/json\n\ntest body\n";
            byte[] data = Encoding.UTF8.GetBytes(request);
            stream.Write(data, 0, data.Length);
            stream.Flush();

            await Task.Delay(1000);

            stream.ReadTimeout = 2000;
            byte[] buffer = new byte[1024];
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                bytesAvailable = bytesRead;
            }
            catch (IOException)
            {
                readTimedOut = true;
            }

            client.Close();
        }
        finally
        {
            server.Stop();
        }

        When.A<bool>("TCP server does not send a response back (gap: no response.Send())",
            () => true,
            (_) => readTimedOut || bytesAvailable == 0)
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("No data received from TCP server (read timed out or zero bytes)", (bool)because.Result);
            Message.PrintLine("[GAP] BamServer.HandleTcpRequest runs the initialization pipeline but never writes a response to the NetworkStream.", ConsoleColor.Yellow);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task ReceiveUdpDataOnServer()
    {
        bool udpDataReceived = false;
        byte[]? receivedData = null;

        int udpPort = GetAvailablePort();
        BamServer server = new BamServerBuilder()
            .UdpPort(udpPort)
            .OnUdpDataReceived((sender, args) =>
            {
                udpDataReceived = true;
                receivedData = args.UdpData;
            })
            .Build();

        BamServerInfo info = server.GetInfo();
        Message.PrintLine($"UDP test server: UdpPort={info.UdpPort}", ConsoleColor.Cyan);

        await server.StartAsync();
        await Task.Delay(1000); // give UDP listener time to bind

        try
        {
            UdpClient udpClient = new UdpClient();
            string request = "PUT bam://test.com/udp/path BAM/2.0\nContent-Type: application/json\n\nudp test body\n";
            byte[] data = Encoding.UTF8.GetBytes(request);
            udpClient.Send(data, data.Length, "127.0.0.1", info.UdpPort);
            udpClient.Close();

            await Task.Delay(1000);
        }
        finally
        {
            server.Stop();
        }

        When.A<bool>("server fires UdpDataReceived event with the datagram data",
            () => udpDataReceived,
            (result) => new object?[] { result, receivedData != null, receivedData?.Length > 0 })
        .TheTest
        .ShouldPass(because =>
        {
            object?[] results = (object?[])because.Result;
            because.ItsTrue("UdpDataReceived event was fired", (bool)results[0]!);
            because.ItsTrue("Received data is not null", (bool)results[1]!);
            because.ItsTrue("Received data has content", (bool)results[2]!);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void UdpClientRequestThrowsUnsupportedRequestType()
    {
        When.A<BamClient>("throws UnsupportedRequestTypeException for UDP response handling",
            () => new BamClient(new JsonObjectDataEncoder()),
            (client) =>
            {
                IBamClientRequest request = client.CreateUdpRequest("/udp/path", "test content");
                try
                {
                    client.ReceiveResponseAsync(request).GetAwaiter().GetResult();
                    return false;
                }
                catch (UnsupportedRequestTypeException)
                {
                    return true;
                }
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("UnsupportedRequestTypeException was thrown for UdpClientRequest", (bool)because.Result);
            Message.PrintLine("[GAP] BamClient.ResponseHandlers has no mapping for UdpClientRequest — no ReceiveUdpResponseAsync method exists.", ConsoleColor.Yellow);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void BamResponseCrashesWithoutHttpContext()
    {
        When.A<BamServerInitializationContext>("constructing BamResponse<T> without HttpContext throws NullReferenceException",
            () =>
            {
                TcpBamServerInitializationContext context = new TcpBamServerInitializationContext();
                context.ServerContext = new BamServerContext { RequestType = RequestType.Tcp };
                return context;
            },
            (context) =>
            {
                try
                {
                    BamResponse<string> response = new BamResponse<string>(context, 200);
                    return false;
                }
                catch (NullReferenceException)
                {
                    return true;
                }
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("NullReferenceException thrown because BamResponse<T> accesses HttpContext.Response.OutputStream", (bool)because.Result);
            Message.PrintLine("[GAP] BamResponse<T> constructor accesses initializationContext.ServerContext.HttpContext.Response.OutputStream — crashes for TCP/UDP where HttpContext is null.", ConsoleColor.Yellow);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    private static int GetAvailablePort()
    {
        TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
