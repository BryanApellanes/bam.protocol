using System.Text;
using Bam.Console;
using Bam.Data.Repositories;
using Bam.Data.Schema;
using Bam.Data.SQLite;
using Bam.Encryption;
using Bam.Generators;
using Bam.Protocol.Data.Server;
using Bam.Protocol.Data.Server.Dao.Repository;
using Bam.Protocol.Profile;
using Bam.Protocol.Server;
using Bam.Server;
using Bam.Test;
using Bam.Web;
using NSubstitute;

namespace Bam.Protocol.Tests;

[UnitTestMenu("BamServerSessionStateProvider should")]
public class BamServerSessionProviderShould : UnitTestMenuContainer
{
    [UnitTest]
    [ConsoleCommand("Save Data")]
    public async Task SaveData()
    {
        string testSessionId = 16.RandomLetters();
        string testKey1 = 8.RandomLetters();
        string testValue1 = 10.RandomLetters();
        string testKey2 = 8.RandomLetters();
        string testValue2 = 10.RandomLetters();
                        
        string newKey = 15.RandomLetters();
        string newValue = 8.RandomLetters();
        
        IBamRequest mockRequest = Substitute.For<IBamRequest>();
        Dictionary<string, string> mockHeaders = new Dictionary<string, string>();
        mockHeaders.Add(Headers.SessionId, testSessionId);
        mockRequest.Headers.Returns(mockHeaders);
        
        After.Setup(reg =>
        {
            ServerSessionSchemaRepository repository = TestSetup.CreateTestData
            (
                testSessionId,
                new Dictionary<string, string>()
                {
                    { testKey1, testValue1 },
                    { testKey2, testValue2 }
                },
                nameof(SaveData)
            );

            reg.For<INonceProvider>().Use<NonceProvider>();
            reg.For<IKeyManager>().Use<KeyManager>();
            reg.For<ISignatureProvider>().Use<RsaSignatureProvider>();
            reg.For<ServerSessionSchemaRepository>().Use(repository);
            ServerSessionManager manager = reg.Get<ServerSessionManager>();//new ServerSessionManager(repository);

            reg.For<IServerSessionState>().Use(manager.GetSession(mockRequest));
            reg.For<ServerSessionSchemaRepository>().Use(repository);

        })
        .When<IServerSessionState>("Sets a value", (state, reg) =>
        {
            ServerSessionSchemaRepository repository = reg.Get<ServerSessionSchemaRepository>();
            ServerSession session = repository.OneServerSessionWhere(x=> x.SessionId == testSessionId);
            session.KeyValues.Count.Equals(2).ShouldBeEqualTo(true);

            state[newKey] = newValue;
        })
        .It.ShouldPass(because =>
        {
            ServerSessionSchemaRepository repository = because.TestCaseRegistry.Get<ServerSessionSchemaRepository>();
            ServerSession session = repository.OneServerSessionWhere(x=> x.SessionId == testSessionId);
            
            because.ItsTrue("there were 3 key value pairs", session.KeyValues.Count == 3);

            ServerSessionManager manager = because.TestCaseRegistry.Get<ServerSessionManager>();
            IServerSessionState state = manager.GetSession(mockRequest);
            because.ItsTrue($"The value of session[{newKey}] is {state[newKey]?.ToString()}", state[newKey].Equals(newValue));
            
        })
        .SoBeHappy()
        .UnlessItFailed();
            
    }
    
    [UnitTest]
    [ConsoleCommand("Start Session")]
    public async Task StartSession()
    {
        After.Setup(reg =>
        {
            FileInfo dbFile = new FileInfo($"./.bam/tests/{nameof(StartSession)}.sqlite");
            SQLiteDatabase database = new SQLiteDatabase(dbFile);
            ServerSessionSchemaRepository repository = new ServerSessionSchemaRepository()
            {
                Database = database
            };

            reg.For<INonceProvider>().Use<NonceProvider>();
            IKeyManager mockKeyManager = Substitute.For<IKeyManager>();
            mockKeyManager.GenerateEccKeyPair().Returns(new EccPublicPrivateKeyPair());
            reg.For<IKeyManager>().Use(mockKeyManager);
            reg.For<ISignatureProvider>().Use(Substitute.For<ISignatureProvider>());
            reg.For<ServerSessionSchemaRepository>().Use(repository);
            reg.For<ServerSessionManager>().Use<ServerSessionManager>();
        })
        .When<ServerSessionManager>("starts a session", (manager, reg) =>
        {
            EccPublicKey clientPublicKey = new EccPublicKey();
            StartSessionRequest request = new StartSessionRequest
            {
                ClientPublicKey = clientPublicKey
            };
            MemoryStream outputStream = new MemoryStream();
            return manager.StartSession(request, outputStream);
        })
        .It.ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
            StartSessionResponse response = because.TheResult.As<StartSessionResponse>();

            because.ItsTrue("SessionId is not null", !string.IsNullOrEmpty(response.SessionId), $"SessionId was null or empty");
            because.ItsTrue("Nonce is not null", !string.IsNullOrEmpty(response.Nonce), $"Nonce was null or empty");
            because.ItsTrue("ServerPublicKey is not null", response.ServerPublicKey != null, "ServerPublicKey was null");

            ServerSessionManager manager = because.TestCaseRegistry.Get<ServerSessionManager>();
            IBamRequest mockRequest = Substitute.For<IBamRequest>();
            Dictionary<string, string> mockHeaders = new Dictionary<string, string>();
            mockHeaders.Add(Headers.SessionId, response.SessionId);
            mockRequest.Headers.Returns(mockHeaders);

            IServerSessionState state = manager.GetSession(mockRequest);
            because.ItsTrue("Session was persisted", state != null, "GetSession returned null for the created session");
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    [ConsoleCommand("Create Session Via Http")]
    public async Task CreateSessionViaHttp()
    {
        int testPort = 9787;
        HostBinding hostBinding = new HostBinding("localhost", testPort);

        FileInfo dbFile = new FileInfo($"./.bam/tests/{nameof(CreateSessionViaHttp)}.sqlite");
        SQLiteDatabase database = new SQLiteDatabase(dbFile);
        ServerSessionSchemaRepository repository = new ServerSessionSchemaRepository()
        {
            Database = database
        };

        BamServerOptions options = new BamServerOptions()
        {
            HttpHostBinding = hostBinding
        };
        options.ComponentRegistry.For<ServerSessionSchemaRepository>().Use(repository);
        IKeyManager mockKeyManager = Substitute.For<IKeyManager>();
        mockKeyManager.GenerateEccKeyPair().Returns(new EccPublicPrivateKeyPair());
        ISignatureProvider mockSignatureProvider = Substitute.For<ISignatureProvider>();

        // Manually construct the session manager with mock dependencies because
        // BamServerOptions.Initialize() eagerly resolves the BamServerContextInitializer
        // singleton, caching the entire dependency chain before we can override registrations.
        ServerSessionManager sessionManager = new ServerSessionManager(
            repository, mockSignatureProvider, mockKeyManager, new NonceProvider());
        ServerSessionInitializationHandler sessionHandler = new ServerSessionInitializationHandler(sessionManager);
        BamServerContextInitializer initializer = new BamServerContextInitializer(
            options.ComponentRegistry.Get<ActorResolverInitializationHandler>(),
            options.ComponentRegistry.Get<AuthorizationCalculatorInitializationHandler>(),
            sessionHandler,
            options.ComponentRegistry.Get<CommandInitializationHandler>(),
            options.ComponentRegistry.Get<AuthenticationInitializationHandler>());
        options.ComponentRegistry.For<IBamServerContextInitializer>().UseSingleton(initializer);

        BamServer server = new BamServer(options);

        await server.StartAsync();

        try
        {
            After.Setup(reg =>
            {
                reg.For<HttpClient>().Use(new HttpClient());
            })
            .When<HttpClient>("sends session creation request", async (httpClient, reg) =>
            {
                EccPublicKey clientPublicKey = new EccPublicKey();
                string requestJson = System.Text.Json.JsonSerializer.Serialize(new
                {
                    ClientPublicKey = clientPublicKey.Pem
                });
                StringContent content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                string url = $"http://localhost:{testPort}{BamSessionPaths.Create}";
                HttpResponseMessage httpResponse = await httpClient.PostAsync(url, content);
                string responseJson = await httpResponse.Content.ReadAsStringAsync();

                return new { HttpResponse = httpResponse, ResponseJson = responseJson };
            })
            .It.ShouldPass(because =>
            {
                because.TheResult.IsNotNull();
                dynamic result = because.TheResult.As<dynamic>();
                if (result == null) return;

                HttpResponseMessage httpResponse = result.HttpResponse;
                string responseJson = result.ResponseJson;

                because.ItsTrue("Status code is success", httpResponse.IsSuccessStatusCode, $"Expected success status code, got {httpResponse.StatusCode}");

                var doc = System.Text.Json.JsonDocument.Parse(responseJson);
                string sessionId = doc.RootElement.GetProperty("SessionId").GetString();
                string nonce = doc.RootElement.GetProperty("Nonce").GetString();
                string serverPublicKey = doc.RootElement.GetProperty("ServerPublicKey").GetString();

                because.ItsTrue("SessionId is not null", !string.IsNullOrEmpty(sessionId), "SessionId was null or empty");
                because.ItsTrue("Nonce is not null", !string.IsNullOrEmpty(nonce), "Nonce was null or empty");
                because.ItsTrue("ServerPublicKey is not null", !string.IsNullOrEmpty(serverPublicKey), "ServerPublicKey was null or empty");
            })
            .SoBeHappy()
            .UnlessItFailed();
        }
        finally
        {
            server.TryStop();
        }
    }

    [UnitTest]
    [ConsoleCommand("Load Data")]
    public async Task LoadData()
    {
        string testSessionId = 16.RandomLetters();
        string testKey1 = 8.RandomLetters();
        string testValue1 = 10.RandomLetters();
        string testKey2 = 8.RandomLetters();
        string testValue2 = 10.RandomLetters();
        
        After.Setup(reg=>
        {
            ServerSessionSchemaRepository repository = TestSetup.CreateTestData(
                testSessionId,
                new Dictionary<string, string>()
                {
                    { testKey1, testValue1 },
                    { testKey2, testValue2 }
                },
                nameof(LoadData));
            
            IBamRequest mockRequest = Substitute.For<IBamRequest>();
            Dictionary<string, string> mockHeaders = new Dictionary<string, string>();
            mockHeaders.Add(Headers.SessionId, testSessionId);
            mockRequest.Headers.Returns(mockHeaders);

            reg.For<INonceProvider>().Use<NonceProvider>();
            reg.For<IKeyManager>().Use<KeyManager>();
            reg.For<ISignatureProvider>().Use<RsaSignatureProvider>();
            reg.For<ServerSessionSchemaRepository>().Use(repository);
            reg.For<IServerSessionManager>().Use<ServerSessionManager>();
            reg.For<IBamRequest>().Use(mockRequest);
            
        })
        .When<IServerSessionManager>("gets session data", (provider, reg) =>
        {
            IBamRequest request = reg.Get<IBamRequest>();
            return provider.GetSession(request);
        })
        .It
        .ShouldPass(because =>
        {
            IServerSessionState state = because.TheResult.As<IServerSessionState>();
            because.ItsTrue("there were two keys", state.Keys.Length == 2, $"there were NOT 2 keys instead there were ({state.Keys.Length})");
            because.ItsTrue("there were two values", state.Values.Length == 2, $"there were NOT 2 values instead there were ({state.Values.Length})");

            because.ItsTrue($"Value for {testKey1} equals {state[testKey1]}", state[testKey1].Equals(testValue1));
            because.ItsTrue($"Value for {testKey2} equals {state[testKey2]}", state[testKey2].Equals(testValue2));

        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}