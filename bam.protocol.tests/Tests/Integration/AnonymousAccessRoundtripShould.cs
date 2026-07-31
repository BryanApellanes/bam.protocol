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
using NSubstitute;

namespace Bam.Protocol.Tests;

[UnitTestMenu("AnonymousAccess roundtrip should", "aart")]
public class AnonymousAccessRoundtripShould : UnitTestMenuContainer
{
    public AnonymousAccessRoundtripShould(ServiceRegistry serviceRegistry) : base(serviceRegistry)
    {
    }

    private async Task<(BamServer server, ClientSessionState sessionState, BamServerInfo info)>
        StartServerWithSessionAndNoProfile()
    {
        // No matching profile for any public key — the exact scenario bamtk#15 repros.
        IProfileManager mockProfileManager = Substitute.For<IProfileManager>();
        mockProfileManager.FindProfileByPublicKeyPem(Arg.Any<string>()).Returns((IProfile)null!);

        IAccessLevelProvider mockAccessLevelProvider = Substitute.For<IAccessLevelProvider>();
        mockAccessLevelProvider.GetAccessLevel(Arg.Any<IBamServerContext>()).Returns(BamAccess.Read);

        BamServerOptions options = new BamServerOptions();
        options.UseNameBasedPort = true;
        options.ComponentRegistry.For<IProfileManager>().UseSingleton(mockProfileManager);
        options.ComponentRegistry.For<IAccessLevelProvider>().UseSingleton(mockAccessLevelProvider);
        BamServer server = new BamServer(options);
        BamServerInfo info = server.GetInfo();
        Message.PrintLine($"Server info: {info.ToJson(true)}", ConsoleColor.Cyan);
        await server.StartAsync();

        EccPublicPrivateKeyPair clientKeyPair = new EccPublicPrivateKeyPair();
        EccPublicKey clientPublicKey = clientKeyPair.GetEccPublicKey();

        ClientSessionManager sessionManager = new ClientSessionManager(new HttpClient(), info.HttpHostBinding);
        StartSessionRequest sessionRequest = new StartSessionRequest { ClientPublicKey = clientPublicKey };
        StartSessionResponse sessionResponse = await sessionManager.StartSessionAsync(sessionRequest);

        Message.PrintLine($"Session created: {sessionResponse.SessionId}", ConsoleColor.Green);

        ClientSessionState sessionState = new ClientSessionState(
            sessionResponse.SessionId,
            sessionResponse.Nonce,
            sessionResponse.ServerPublicKey,
            clientKeyPair
        );

        return (server, sessionState, info);
    }

    [UnitTest]
    public async Task AnonymousNoEncryptionTcpRoundtripSucceedsWithUnknownActor()
    {
        var (server, sessionState, info) = await StartServerWithSessionAndNoProfile();

        IBamServerContext capturedContext = null!;
        ManualResetEventSlim contextReceived = new ManualResetEventSlim(false);

        server.CreateContextComplete += (sender, args) =>
        {
            if (args.ServerContext?.RequestType == RequestType.Tcp)
            {
                capturedContext = args.ServerContext;
                contextReceived.Set();
            }
        };

        try
        {
            BamClient client = new BamClient(
                new JsonObjectDataEncoder(),
                info.HttpHostBinding,
                new BamHostBinding("localhost", info.TcpPort),
                new BamHostBinding("localhost", info.UdpPort));
            client.SessionState = sessionState;

            MethodInvocationRequest invocation = MethodInvocationRequest.For(typeof(TestAnonymousEchoService), "Echo", "Hello Anonymous");
            invocation.ClientInitialize();

            IBamClientRequest request = client.CreateRequestBuilder(BamClientProtocols.Tcp)
                .Path("/invoke")
                .HttpMethod(HttpMethods.POST)
                .Content(invocation)
                .Build();

            IBamClientResponse response = await client.ReceiveResponseAsync(request);

            Message.PrintLine($"Response status: {response.StatusCode}", ConsoleColor.Cyan);
            Message.PrintLine($"Response content: {response.Content}", ConsoleColor.Cyan);

            if (response.StatusCode != 200)
            {
                throw new Exception($"Expected status 200 but got {response.StatusCode}. Content: {response.Content}");
            }

            if (!response.Content.Contains("Echo: Hello Anonymous"))
            {
                throw new Exception($"Expected response to contain 'Echo: Hello Anonymous' but got: {response.Content}");
            }

            if (!contextReceived.Wait(TimeSpan.FromSeconds(10)))
            {
                throw new Exception("Timed out waiting for server to process TCP request");
            }

            if (capturedContext == null)
            {
                throw new Exception("Server context was not captured");
            }

            if (capturedContext.Actor == null)
            {
                throw new Exception("Expected the well-known anonymous actor but Actor was null");
            }

            if (capturedContext.Actor.Handle != AnonymousActorProvider.AnonymousHandle)
            {
                throw new Exception($"Expected actor handle '{AnonymousActorProvider.AnonymousHandle}' but got '{capturedContext.Actor.Handle}'");
            }

            if (capturedContext.Actor.Name != AnonymousActorProvider.AnonymousName)
            {
                throw new Exception($"Expected actor name '{AnonymousActorProvider.AnonymousName}' but got '{capturedContext.Actor.Name}'");
            }

            Message.PrintLine("PASSED: Anonymous no-encryption TCP roundtrip succeeded with UNKNOWN actor", ConsoleColor.Green);
        }
        finally
        {
            server.Stop();
        }
    }

    [UnitTest]
    public async Task AuthenticatedTcpRoundtripStillResolvesRealActor()
    {
        // Regression guard: a genuinely authenticated call, over the same reordered pipeline,
        // must still resolve the real profile-backed actor (not the anonymous sentinel).
        // ActorResolver.ResolveActor builds the resolved actor from PersonHandle, not
        // ProfileHandle — the JWT/profile lookup key and the resolved actor identity are
        // distinct fields, so both are set explicitly here to keep the assertion meaningful.
        string actorHandle = "test-tcp-real-actor";
        string personHandle = "test-tcp-real-actor-person";

        IProfile mockProfile = Substitute.For<IProfile>();
        mockProfile.ProfileHandle.Returns(actorHandle);
        mockProfile.PersonHandle.Returns(personHandle);
        mockProfile.Name.Returns("Test Actor");

        IProfileManager mockProfileManager = Substitute.For<IProfileManager>();
        mockProfileManager.FindProfileByHandle(actorHandle).Returns(mockProfile);
        mockProfileManager.FindProfileByPublicKeyPem(Arg.Any<string>()).Returns(mockProfile);

        IAccessLevelProvider mockAccessLevelProvider = Substitute.For<IAccessLevelProvider>();
        mockAccessLevelProvider.GetAccessLevel(Arg.Any<IBamServerContext>()).Returns(BamAccess.Read);

        BamServerOptions options = new BamServerOptions();
        options.UseNameBasedPort = true;
        options.ComponentRegistry.For<IProfileManager>().UseSingleton(mockProfileManager);
        options.ComponentRegistry.For<IAccessLevelProvider>().UseSingleton(mockAccessLevelProvider);
        BamServer server = new BamServer(options);
        BamServerInfo info = server.GetInfo();
        await server.StartAsync();

        EccPublicPrivateKeyPair clientKeyPair = new EccPublicPrivateKeyPair();
        EccPublicKey clientPublicKey = clientKeyPair.GetEccPublicKey();

        ClientSessionManager sessionManager = new ClientSessionManager(new HttpClient(), info.HttpHostBinding);
        StartSessionRequest sessionRequest = new StartSessionRequest { ClientPublicKey = clientPublicKey };
        StartSessionResponse sessionResponse = await sessionManager.StartSessionAsync(sessionRequest);

        ClientSessionState sessionState = new ClientSessionState(
            sessionResponse.SessionId,
            sessionResponse.Nonce,
            sessionResponse.ServerPublicKey,
            clientKeyPair
        );

        PrivateKeyProvider privateKeyProvider = new PrivateKeyProvider(clientKeyPair);
        BamJwtToken jwtToken = new BamJwtToken(sessionState.SessionId, actorHandle, "bam-integration-test");
        string jwt = jwtToken.Encode(privateKeyProvider.GetPrivateKey());
        sessionState.AuthorizationToken = jwt;

        IBamServerContext capturedContext = null!;
        ManualResetEventSlim contextReceived = new ManualResetEventSlim(false);

        server.CreateContextComplete += (sender, args) =>
        {
            if (args.ServerContext?.RequestType == RequestType.Tcp)
            {
                capturedContext = args.ServerContext;
                contextReceived.Set();
            }
        };

        try
        {
            BamClient client = new BamClient(
                new JsonObjectDataEncoder(),
                info.HttpHostBinding,
                new BamHostBinding("localhost", info.TcpPort),
                new BamHostBinding("localhost", info.UdpPort));
            client.SessionState = sessionState;

            MethodInvocationRequest invocation = MethodInvocationRequest.For(typeof(TestEchoService), "Echo", "Hello Real Actor");
            invocation.ClientInitialize();

            IBamClientRequest request = client.CreateRequestBuilder(BamClientProtocols.Tcp)
                .Path("/invoke")
                .HttpMethod(HttpMethods.POST)
                .Content(invocation)
                .Build();

            IBamClientResponse response = await client.ReceiveResponseAsync(request);

            if (response.StatusCode != 200)
            {
                throw new Exception($"Expected status 200 but got {response.StatusCode}. Content: {response.Content}");
            }

            if (!contextReceived.Wait(TimeSpan.FromSeconds(10)))
            {
                throw new Exception("Timed out waiting for server to process TCP request");
            }

            if (capturedContext?.Actor == null)
            {
                throw new Exception("Expected a resolved actor but Actor was null");
            }

            if (capturedContext.Actor.Handle != personHandle)
            {
                throw new Exception($"Expected actor handle '{personHandle}' but got '{capturedContext.Actor.Handle}'");
            }

            Message.PrintLine("PASSED: Authenticated TCP roundtrip still resolves the real actor", ConsoleColor.Green);
        }
        finally
        {
            server.Stop();
        }
    }
}
