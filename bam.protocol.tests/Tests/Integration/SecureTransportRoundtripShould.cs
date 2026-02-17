using System.Net;
using System.Net.Sockets;
using System.Text;
using Bam.Console;
using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Protocol.Client;
using Bam.Protocol.Data;
using Bam.Protocol.Profile;
using Bam.Protocol.Server;
using Bam.Test;
using Bam.Web;
using Newtonsoft.Json;
using NSubstitute;

namespace Bam.Protocol.Tests;

[UnitTestMenu("SecureTransport roundtrip should", "strt")]
public class SecureTransportRoundtripShould : UnitTestMenuContainer
{
    public SecureTransportRoundtripShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    private async Task<(BamServer server, ClientSessionState sessionState, BamServerInfo info)>
        StartServerWithSession(string actorHandle, TimeSpan? jwtExpiry = null)
    {
        // 1. Create mock profile
        IProfile mockProfile = Substitute.For<IProfile>();
        mockProfile.ProfileHandle.Returns(actorHandle);
        mockProfile.PersonHandle.Returns("test-person");
        mockProfile.Name.Returns("Test Actor");

        // 2. Create mock IProfileManager
        IProfileManager mockProfileManager = Substitute.For<IProfileManager>();
        mockProfileManager.FindProfileByHandle(actorHandle).Returns(mockProfile);
        mockProfileManager.FindProfileByPublicKey(Arg.Any<string>()).Returns(mockProfile);

        // 3. Create mock IAccessLevelProvider — returns Read access
        IAccessLevelProvider mockAccessLevelProvider = Substitute.For<IAccessLevelProvider>();
        mockAccessLevelProvider.GetAccessLevel(Arg.Any<IBamServerContext>()).Returns(BamAccess.Read);

        // 4. Build server with overridden dependencies and unique ports
        BamServerOptions options = new BamServerOptions();
        options.UseNameBasedPort = true;
        options.ComponentRegistry.For<IProfileManager>().UseSingleton(mockProfileManager);
        options.ComponentRegistry.For<IAccessLevelProvider>().UseSingleton(mockAccessLevelProvider);
        BamServer server = new BamServer(options);
        BamServerInfo info = server.GetInfo();
        Message.PrintLine($"Server info: {info.ToJson(true)}", ConsoleColor.Cyan);
        await server.StartAsync();

        // 5. Create client ECC key pair (we control it for JWT signing)
        EccPublicPrivateKeyPair clientKeyPair = new EccPublicPrivateKeyPair();
        EccPublicKey clientPublicKey = clientKeyPair.GetEccPublicKey();

        // 6. Start session via ClientSessionManager
        ClientSessionManager sessionManager = new ClientSessionManager(new HttpClient(), info.HttpHostBinding);
        StartSessionRequest sessionRequest = new StartSessionRequest { ClientPublicKey = clientPublicKey };
        StartSessionResponse sessionResponse = await sessionManager.StartSessionAsync(sessionRequest);

        Message.PrintLine($"Session created: {sessionResponse.SessionId}", ConsoleColor.Green);

        // 7. Construct ClientSessionState with our key pair
        ClientSessionState sessionState = new ClientSessionState(
            sessionResponse.SessionId,
            sessionResponse.Nonce,
            sessionResponse.ServerPublicKey,
            clientKeyPair
        );

        // 8. Create JWT signed with client's private key
        PrivateKeyProvider privateKeyProvider = new PrivateKeyProvider(clientKeyPair);
        BamJwtToken jwtToken = new BamJwtToken(sessionState.SessionId, actorHandle, "bam-integration-test", jwtExpiry);
        string jwt = jwtToken.Encode(privateKeyProvider.GetPrivateKey());
        sessionState.AuthorizationToken = jwt;

        Message.PrintLine("JWT created and set on session state", ConsoleColor.Green);

        return (server, sessionState, info);
    }

    [UnitTest]
    public async Task CompleteAuthenticatedTcpRoundtrip()
    {
        string actorHandle = "test-tcp-roundtrip-actor";
        var (server, sessionState, info) = await StartServerWithSession(actorHandle);

        try
        {
            // Create BamClient with session state, targeting TCP
            BamClient client = new BamClient(
                new JsonObjectDataEncoder(),
                info.HttpHostBinding,
                new BamHostBinding("localhost", info.TcpPort),
                new BamHostBinding("localhost", info.UdpPort));
            client.SessionState = sessionState;

            // Build MethodInvocationRequest for TestEchoService.Echo("Hello TCP")
            MethodInvocationRequest invocation = MethodInvocationRequest.For(typeof(TestEchoService), "Echo", "Hello TCP");
            invocation.ClientInitialize();

            Message.PrintLine($"OperationIdentifier: {invocation.OperationIdentifier}", ConsoleColor.Yellow);

            // Create TCP request with invocation object as content
            IBamClientRequest request = client.CreateRequestBuilder(BamClientProtocols.Tcp)
                .Path("/invoke")
                .HttpMethod(HttpMethods.POST)
                .Content(invocation)
                .Build();

            // Send request (triggers encryption, signing, nonce hash, JWT header)
            IBamClientResponse response = await client.ReceiveResponseAsync(request);

            Message.PrintLine($"Response status: {response.StatusCode}", ConsoleColor.Cyan);
            Message.PrintLine($"Response content: {response.Content}", ConsoleColor.Cyan);

            // Assert
            if (response.StatusCode != 200)
            {
                throw new Exception($"Expected status 200 but got {response.StatusCode}. Content: {response.Content}");
            }

            if (!response.Content.Contains("Echo: Hello TCP"))
            {
                throw new Exception($"Expected response to contain 'Echo: Hello TCP' but got: {response.Content}");
            }

            Message.PrintLine("PASSED: Full authenticated TCP roundtrip succeeded", ConsoleColor.Green);
        }
        finally
        {
            server.Stop();
        }
    }

    [UnitTest]
    public async Task TcpRoundtripWithBadSignatureIsRejected()
    {
        string actorHandle = "test-tcp-badsig-actor";
        var (server, sessionState, info) = await StartServerWithSession(actorHandle);

        try
        {
            // Build the invocation request
            MethodInvocationRequest invocation = MethodInvocationRequest.For(typeof(TestEchoService), "Echo", "Hello Bad Sig");
            invocation.ClientInitialize();

            // Encode the invocation to get the body string (same as BamClient.CreateRequestData)
            JsonObjectDataEncoder encoder = new JsonObjectDataEncoder();
            IObjectEncoding encoding = encoder.Encode(invocation);
            string body = encoding.Encoding.GetString(encoding.Value);

            // Manually construct TCP request
            TcpClientRequest request = new TcpClientRequest();
            request.Host = new BamHostBinding("localhost", info.TcpPort);
            request.Path = "/invoke";
            request.HttpMethod = HttpMethods.POST;

            // Use SecurityProvider to encrypt and sign (sets headers on the request)
            ClientRequestSecurityProvider securityProvider = new ClientRequestSecurityProvider();
            string encryptedBody = securityProvider.PrepareRequest(request, body, sessionState);

            // Tamper with the body signature
            request.Headers[Headers.BodySignature] = "bogus-signature-value";

            // Build raw TCP data (replicating BamClient.CreateRequestData format)
            StringBuilder rawData = new StringBuilder();
            rawData.AppendLine(request.GetRequestLine().ToString());
            foreach (KeyValuePair<string, string> header in request.Headers)
            {
                rawData.AppendLine($"{header.Key}: {header.Value}");
            }
            rawData.AppendLine();
            rawData.AppendLine(encryptedBody);

            Message.PrintLine($"Sending tampered TCP request to port {info.TcpPort}", ConsoleColor.Yellow);

            // Send raw over TCP
            byte[] data = Encoding.UTF8.GetBytes(rawData.ToString());
            TcpClient tcpClient = new TcpClient("localhost", info.TcpPort);
            NetworkStream stream = tcpClient.GetStream();
            await stream.WriteAsync(data, 0, data.Length);
            tcpClient.Client.Shutdown(SocketShutdown.Send);

            // Read response
            using MemoryStream responseBuffer = new MemoryStream();
            byte[] readBuffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(readBuffer, 0, readBuffer.Length)) > 0)
            {
                responseBuffer.Write(readBuffer, 0, bytesRead);
            }
            tcpClient.Close();

            string responseText = Encoding.UTF8.GetString(responseBuffer.ToArray());
            BamClientResponse response = new BamClientResponse(responseText);

            Message.PrintLine($"Response status: {response.StatusCode}", ConsoleColor.Cyan);
            Message.PrintLine($"Response content: {response.Content}", ConsoleColor.Cyan);

            // Assert: should NOT be 200
            if (response.StatusCode == 200)
            {
                throw new Exception("Expected non-200 status but got 200. Server accepted a tampered signature!");
            }

            Message.PrintLine($"PASSED: Tampered signature rejected with status {response.StatusCode}", ConsoleColor.Green);
        }
        finally
        {
            server.Stop();
        }
    }

    [UnitTest]
    public async Task AuthenticatedUdpProcessedByServer()
    {
        string actorHandle = "test-udp-roundtrip-actor";
        var (server, sessionState, info) = await StartServerWithSession(actorHandle);

        IBamServerContext capturedContext = null!;
        ManualResetEventSlim contextReceived = new ManualResetEventSlim(false);

        // Hook CreateContextComplete to capture the server context after pipeline runs
        server.CreateContextComplete += (sender, args) =>
        {
            if (args.ServerContext?.RequestType == RequestType.Udp)
            {
                capturedContext = args.ServerContext;
                contextReceived.Set();
            }
        };

        try
        {
            // Allow UDP listener to start
            await Task.Delay(500);

            // Create BamClient targeting UDP
            BamClient client = new BamClient(
                new JsonObjectDataEncoder(),
                info.HttpHostBinding,
                new BamHostBinding("localhost", info.TcpPort),
                new BamHostBinding("localhost", info.UdpPort));
            client.SessionState = sessionState;

            // Build request
            MethodInvocationRequest invocation = MethodInvocationRequest.For(typeof(TestEchoService), "Echo", "Hello UDP");
            invocation.ClientInitialize();

            Message.PrintLine($"Sending authenticated UDP request to port {info.UdpPort}", ConsoleColor.Yellow);

            // Create and send UDP request
            IBamClientRequest request = client.CreateRequestBuilder(BamClientProtocols.Udp)
                .Path("/invoke")
                .Content(invocation)
                .Build();

            await client.ReceiveResponseAsync(request);

            // Wait for server to process the request
            if (!contextReceived.Wait(TimeSpan.FromSeconds(10)))
            {
                throw new Exception("Timed out waiting for server to process UDP request");
            }

            // Assert on captured server context
            if (capturedContext == null)
            {
                throw new Exception("Server context was not captured");
            }

            if (capturedContext.ServerSessionState == null)
            {
                throw new Exception("Session state was not resolved on server");
            }

            if (capturedContext.Authentication == null)
            {
                throw new Exception("Authentication was not set on server context");
            }

            if (!capturedContext.Authentication.Success)
            {
                string messages = string.Join(", ", capturedContext.Authentication.Messages);
                throw new Exception($"Authentication failed on server: {messages}");
            }

            Message.PrintLine("PASSED: Authenticated UDP request processed by server", ConsoleColor.Green);
        }
        finally
        {
            server.Stop();
        }
    }

    [UnitTest]
    public async Task UdpWithExpiredJwtIsRejected()
    {
        string actorHandle = "test-udp-expired-jwt-actor";

        // Create session with expired JWT (60 seconds in the past)
        var (server, sessionState, info) = await StartServerWithSession(actorHandle, TimeSpan.FromSeconds(-60));

        IBamServerContext capturedContext = null!;
        ManualResetEventSlim contextReceived = new ManualResetEventSlim(false);

        // Hook CreateContextComplete to capture the server context
        server.CreateContextComplete += (sender, args) =>
        {
            if (args.ServerContext?.RequestType == RequestType.Udp)
            {
                capturedContext = args.ServerContext;
                contextReceived.Set();
            }
        };

        try
        {
            // Allow UDP listener to start (longer delay to avoid port reuse issues)
            await Task.Delay(1000);

            // Create BamClient targeting UDP
            BamClient client = new BamClient(
                new JsonObjectDataEncoder(),
                info.HttpHostBinding,
                new BamHostBinding("localhost", info.TcpPort),
                new BamHostBinding("localhost", info.UdpPort));
            client.SessionState = sessionState;

            // Build and send request
            MethodInvocationRequest invocation = MethodInvocationRequest.For(typeof(TestEchoService), "Echo", "Hello Expired");
            invocation.ClientInitialize();

            Message.PrintLine($"Sending UDP request with expired JWT to port {info.UdpPort}", ConsoleColor.Yellow);

            IBamClientRequest request = client.CreateRequestBuilder(BamClientProtocols.Udp)
                .Path("/invoke")
                .Content(invocation)
                .Build();

            await client.ReceiveResponseAsync(request);

            // Wait for server to process the request
            if (!contextReceived.Wait(TimeSpan.FromSeconds(10)))
            {
                throw new Exception("Timed out waiting for server to process UDP request");
            }

            // Assert: authentication should have failed
            if (capturedContext == null)
            {
                throw new Exception("Server context was not captured");
            }

            if (capturedContext.Authentication == null)
            {
                throw new Exception("Authentication was not set on server context");
            }

            if (capturedContext.Authentication.Success)
            {
                throw new Exception("Expected authentication to fail for expired JWT but it succeeded");
            }

            Message.PrintLine($"PASSED: Expired JWT rejected. Messages: {string.Join(", ", capturedContext.Authentication.Messages)}", ConsoleColor.Green);
        }
        finally
        {
            server.Stop();
        }
    }
}
