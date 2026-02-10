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
            reg.For<IKeyManager>().Use<KeyManager>();
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
            because.TheResult.Is<StartSessionResponse>();

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
            because.TheResult.Is<IServerSessionState>();
            
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