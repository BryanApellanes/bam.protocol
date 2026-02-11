using Bam.Console;
using Bam.Data.Objects;
using Bam.DependencyInjection;
using Bam.Encryption;
using Bam.Protocol.Client;
using Bam.Protocol.Data;
using Bam.Protocol.Profile;
using Bam.Protocol.Server;
using Bam.Test;
using Newtonsoft.Json;
using NSubstitute;

namespace Bam.Protocol.Tests;

[UnitTestMenu("AuthenticatedRoundtrip should", "arts")]
public class AuthenticatedRoundtripShould : UnitTestMenuContainer
{
    public AuthenticatedRoundtripShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    [UnitTest]
    public async Task CompleteAuthenticatedHttpRoundtrip()
    {
        string actorHandle = "test-roundtrip-actor";

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

        // 4. Build server with overridden dependencies
        BamServerOptions options = new BamServerOptions();
        options.ComponentRegistry.For<IProfileManager>().UseSingleton(mockProfileManager);
        options.ComponentRegistry.For<IAccessLevelProvider>().UseSingleton(mockAccessLevelProvider);
        BamServer server = new BamServer(options);
        BamServerInfo info = server.GetInfo();
        Message.PrintLine($"Server info: {info.ToJson(true)}", ConsoleColor.Cyan);
        await server.StartAsync();

        try
        {
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
            BamJwtToken jwtToken = new BamJwtToken(sessionState.SessionId, actorHandle, "bam-integration-test");
            string jwt = jwtToken.Encode(privateKeyProvider.GetPrivateKey());
            sessionState.AuthorizationToken = jwt;

            Message.PrintLine("JWT created and set on session state", ConsoleColor.Green);

            // 9. Create BamClient with session state
            BamClient client = new BamClient(new JsonObjectDataEncoder(), info.HttpHostBinding);
            client.SessionState = sessionState;

            // 10. Build MethodInvocationRequest for TestEchoService.Echo("Hello")
            MethodInvocationRequest invocation = MethodInvocationRequest.For(typeof(TestEchoService), "Echo", "Hello");
            invocation.ClientInitialize();
            string body = JsonConvert.SerializeObject(invocation);

            Message.PrintLine($"Request body: {body}", ConsoleColor.Yellow);

            // 11. Create HTTP request
            IBamClientRequest request = client.CreateRequestBuilder(BamClientProtocols.Http)
                .Path("/invoke")
                .HttpMethod(HttpMethods.POST)
                .Content(body)
                .Build();

            // 12. Send request (triggers encryption, signing, nonce hash, JWT header)
            IBamClientResponse response = await client.ReceiveResponseAsync(request);

            Message.PrintLine($"Response status: {response.StatusCode}", ConsoleColor.Cyan);
            Message.PrintLine($"Response content: {response.Content}", ConsoleColor.Cyan);

            // 13. Assert
            if (response.StatusCode != 200)
            {
                throw new Exception($"Expected status 200 but got {response.StatusCode}. Content: {response.Content}");
            }

            if (!response.Content.Contains("Echo: Hello"))
            {
                throw new Exception($"Expected response to contain 'Echo: Hello' but got: {response.Content}");
            }

            Message.PrintLine("Integration test PASSED: Full authenticated HTTP roundtrip succeeded", ConsoleColor.Green);
        }
        finally
        {
            server.Stop();
        }
    }
}
