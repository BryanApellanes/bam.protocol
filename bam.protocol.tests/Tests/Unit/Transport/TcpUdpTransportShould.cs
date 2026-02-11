using System.Net;
using System.Net.Sockets;
using System.Text;
using Bam.Console;
using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Protocol.Client;
using Bam.Protocol.Server;
using Bam.Server;
using Bam.Services;
using Bam.Web;
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

        After.Setup(reg =>
        {
            reg.For<BamRequestReader>()
                .Use(new BamRequestReader(new BamRequestReaderOptions(new BamServerOptions())));
        })
        .When<BamRequestReader>("parses BAM/2.0 request from stream", (reader) =>
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
    public void ReadAndParseTcpClientRequest()
    {
        string bamRequest = $@"GET bam://test.com/tcp/path BAM/2.0
Content-Type: application/json; charset=utf-8

test body
";
        int port = GetAvailablePort();

        After.Setup(reg =>
        {
            reg.For<BamRequestReader>()
                .Use(new BamRequestReader(new BamRequestReaderOptions(new BamServerOptions())));
        })
        .When<BamRequestReader>("parses BAM/2.0 request from TcpClient with headers and request line", (reader) =>
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
                sender.Client.Shutdown(SocketShutdown.Send);

                TcpClient serverSide = listener.AcceptTcpClient();

                IBamRequest result = reader.ReadRequest(serverSide);

                sender.Close();
                serverSide.Close();

                BamRequest concrete = (BamRequest)result;
                return new object?[]
                {
                    result.Content,
                    result.Headers,
                    concrete.Line,
                    concrete.Line?.Method.ToString(),
                    concrete.Line?.RequestUri,
                    concrete.Line?.ProtocolVersion,
                    result.Headers?.ContainsKey("content-type")
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
            because.ItsTrue("Content is 'test body'", "test body".Equals(results[0]));
            because.ItsTrue("Headers is not null (parsed)", results[1] != null);
            because.ItsTrue("Line is not null (parsed)", results[2] != null);
            because.ItsTrue("Method is GET", "GET".Equals(results[3]));
            because.ItsTrue("RequestUri is bam://test.com/tcp/path", "bam://test.com/tcp/path".Equals(results[4]));
            because.ItsTrue("ProtocolVersion is BAM/2.0", "BAM/2.0".Equals(results[5]));
            because.ItsTrue("Headers contain content-type", results[6] is true);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task AcceptTcpConnectionAndCreateContext()
    {
        bool tcpClientConnected = false;

        After.Setup(reg =>
        {
            reg.For<BamServer>()
                .Use(() => new BamServerBuilder()
                    .OnTcpClientConnected((sender, args) => tcpClientConnected = true)
                    .Build());
        })
        .When<BamServer>("accepts a TCP connection and fires TcpClientConnected", async (server) =>
        {
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
                client.Client.Shutdown(SocketShutdown.Send);

                await Task.Delay(500);
                client.Close();
            }
            finally
            {
                server.Stop();
            }

            return (object)tcpClientConnected;
        })
        .TheTest
        .ShouldPass(because =>
        {
            because.ItsTrue("TcpClientConnected event was fired", (bool)because.Result);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task TcpServerSendsResponse()
    {
        string responseText = string.Empty;

        After.Setup(reg =>
        {
            reg.For<BamServer>().Use(() => new BamServerBuilder().Build());
        })
        .When<BamServer>("sends a response back over TCP", async (server) =>
        {
            BamServerInfo info = server.GetInfo();
            Message.PrintLine($"TCP response test: TcpPort={info.TcpPort}", ConsoleColor.Cyan);

            await server.StartAsync();
            await Task.Delay(200);

            try
            {
                TcpClient client = new TcpClient();
                client.Connect("127.0.0.1", info.TcpPort);
                NetworkStream stream = client.GetStream();

                string request = "GET bam://test.com/path BAM/2.0\nContent-Type: application/json\n\ntest body\n";
                byte[] data = Encoding.UTF8.GetBytes(request);
                stream.Write(data, 0, data.Length);
                client.Client.Shutdown(SocketShutdown.Send);

                using MemoryStream responseBuffer = new MemoryStream();
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    responseBuffer.Write(buffer, 0, bytesRead);
                }

                responseText = Encoding.UTF8.GetString(responseBuffer.ToArray());
                client.Close();
            }
            finally
            {
                server.Stop();
            }

            return (object)responseText;
        })
        .TheTest
        .ShouldPass(because =>
        {
            string text = (string)because.Result;
            because.ItsTrue("Response is not empty", !string.IsNullOrEmpty(text));
            because.ItsTrue("Response starts with BAM/2.0", text.StartsWith("BAM/2.0"));
            Message.PrintLine($"[TCP Response] {text}", ConsoleColor.Green);
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

        After.Setup(reg =>
        {
            reg.For<BamServer>()
                .Use(() => new BamServerBuilder()
                    .UdpPort(udpPort)
                    .OnUdpDataReceived((sender, args) =>
                    {
                        udpDataReceived = true;
                        receivedData = args.UdpData;
                    })
                    .Build());
        })
        .When<BamServer>("receives UDP data and fires UdpDataReceived event", async (server) =>
        {
            BamServerInfo info = server.GetInfo();
            Message.PrintLine($"UDP test server: UdpPort={info.UdpPort}", ConsoleColor.Cyan);

            await server.StartAsync();
            await Task.Delay(1000);

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

            return (object)new object?[] { udpDataReceived, receivedData != null, receivedData?.Length > 0 };
        })
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
        After.Setup(reg =>
        {
            reg.For<BamClient>().Use(new BamClient(new JsonObjectDataEncoder()));
        })
        .When<BamClient>("throws UnsupportedRequestTypeException for UDP response handling", (client) =>
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
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void BamResponseWorksWithoutHttpContext()
    {
        After.Setup(reg =>
        {
            TcpBamServerInitializationContext context = new TcpBamServerInitializationContext();
            context.ServerContext = new BamServerContext
            {
                RequestType = RequestType.Tcp,
                OutputStream = new MemoryStream()
            };
            reg.For<BamServerInitializationContext>().Use(context);
        })
        .When<BamServerInitializationContext>("constructs BamResponse<T> without HttpContext", (context) =>
        {
            try
            {
                BamResponse<string> response = new BamResponse<string>(context, 200);
                return new object[] { true, response.StatusCode };
            }
            catch (Exception ex)
            {
                return new object[] { false, ex.GetType().Name };
            }
        })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            because.ItsTrue("BamResponse<T> constructed without NullReferenceException", (bool)results[0]);
            because.ItsTrue("StatusCode is 200", 200.Equals(results[1]));
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public async Task TcpRoundtrip()
    {
        int tcpPort = GetAvailablePort();

        After.Setup(reg =>
        {
            reg.For<BamServer>()
                .Use(() => new BamServerBuilder()
                    .TcpPort(tcpPort)
                    .Build());
            reg.For<BamClient>()
                .Use(new BamClient(
                    new JsonObjectDataEncoder(),
                    BamClient.DefaultHttpBaseAddress,
                    new BamHostBinding("localhost", tcpPort)));
        })
        .When<BamServer>("handles a full TCP roundtrip", async (server, reg) =>
        {
            await server.StartAsync();
            await Task.Delay(200);

            try
            {
                BamClient client = reg.Get<BamClient>();
                IBamClientRequest request = client.CreateTcpRequest("/test/roundtrip");
                IBamClientResponse response = await client.ReceiveResponseAsync(request);
                return (object)new object[] { response.Content, response.StatusCode };
            }
            finally
            {
                server.Stop();
            }
        })
        .TheTest
        .ShouldPass(because =>
        {
            object[] results = (object[])because.Result;
            string text = (string)results[0];
            int code = (int)results[1];
            because.ItsTrue("Response content is not empty", !string.IsNullOrEmpty(text));
            because.ItsTrue("Response contains BAM/2.0", text.Contains("BAM/2.0"));
            because.ItsTrue("Status code is non-zero", code != 0);
            Message.PrintLine($"[TCP Roundtrip] StatusCode={code}, Content={text}", ConsoleColor.Green);
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void TcpRequestIncludesSecurityHeaders()
    {
        int port = GetAvailablePort();
        EccPublicPrivateKeyPair clientKeyPair = new EccPublicPrivateKeyPair();
        EccPublicPrivateKeyPair serverKeyPair = new EccPublicPrivateKeyPair();

        After.Setup(reg =>
        {
            BamClient client = new BamClient(
                new JsonObjectDataEncoder(),
                BamClient.DefaultHttpBaseAddress,
                new BamHostBinding("localhost", port));
            client.SessionState = new ClientSessionState(
                "tcp-session-id",
                "tcp-nonce-value",
                serverKeyPair.GetEccPublicKey(),
                clientKeyPair);
            reg.For<BamClient>().Use(client);
        })
        .When<BamClient>("sends TCP request with security headers when SessionState is set", (client) =>
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            try
            {
                IBamClientRequest request = client.CreateTcpRequest("/secure/path");
                request.Content = "secure payload";

                Task<IBamClientResponse> sendTask = client.ReceiveResponseAsync(request);

                TcpClient serverSide = listener.AcceptTcpClient();
                using MemoryStream buffer = new MemoryStream();
                NetworkStream stream = serverSide.GetStream();
                byte[] readBuf = new byte[4096];
                int bytesRead;
                while ((bytesRead = stream.Read(readBuf, 0, readBuf.Length)) > 0)
                {
                    buffer.Write(readBuf, 0, bytesRead);
                }

                string rawRequest = Encoding.UTF8.GetString(buffer.ToArray());
                serverSide.Close();

                return rawRequest;
            }
            finally
            {
                listener.Stop();
            }
        })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult
                .IsNotNull()
                .As<string>("has session ID header", raw => raw!.Contains($"{Headers.SessionId}:"))
                .As<string>("has body signature header", raw => raw!.Contains($"{Headers.BodySignature}:"))
                .As<string>("has nonce header", raw => raw!.Contains($"{Headers.Nonce}:"))
                .As<string>("has nonce hash header", raw => raw!.Contains($"{Headers.NonceHash}:"))
                .As<string>("plaintext body is not present (encrypted)", raw => !raw!.Contains("secure payload"));
            Message.PrintLine($"[TCP Security Headers] Raw request:\n{because.Result}", ConsoleColor.Green);
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
