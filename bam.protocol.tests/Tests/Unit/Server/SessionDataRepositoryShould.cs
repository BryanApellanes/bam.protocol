
using Bam;
using Bam.Data;
using Bam.Data.Repositories;
using Bam.Data.Schema;
using Bam.Data.SQLite;
using Bam.Generators;
using Bam.Protocol.Data.Server;
using Bam.Protocol.Data.Server.Dao.Repository;
using Bam.Protocol.Server;
using Bam.Test;
using Bam.Web;
using NSubstitute;

[UnitTestMenu("SessionDataRepository Should")]
public class SessionDataRepositoryShould : UnitTestMenuContainer
{
    [UnitTest]
    public void SaveValues()
    {
        string testSessionId = 16.RandomLetters();

        When.A<ServerSessionSchemaRepository>("saves session values",
            () => new ServerSessionSchemaRepository()
            {
                Database = new SQLiteDatabase(new FileInfo($"./.bam/tests/{nameof(SaveValues)}.sqlite"))
            },
            (repository) =>
            {
                ServerSession session = new ServerSession() { SessionId = testSessionId };
                session.KeyValues.Add(new ServerSessionKeyValuePair() { Key = "testKey", Value = "testValue" });
                session = repository.Save(session);
                return repository.Retrieve<ServerSession>(session.Id);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
            ServerSession reloaded = because.TheResult.As<ServerSession>();
            because.ItsTrue("KeyValues is not null", reloaded?.KeyValues != null);
            if (reloaded?.KeyValues is List<ServerSessionKeyValuePair> kvps)
            {
                because.ItsTrue("KeyValues count equals 1", kvps.Count == 1);
            }
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void RetrieveBySessionId()
    {
        string testSessionId = 16.RandomLetters();

        When.A<ServerSessionSchemaRepository>("retrieves session by session id",
            () => new ServerSessionSchemaRepository()
            {
                Database = new SQLiteDatabase(new FileInfo($"./.bam/tests/{nameof(SaveValues)}.sqlite"))
            },
            (repository) =>
            {
                ServerSession session = new ServerSession() { SessionId = testSessionId };
                session.KeyValues.Add(new ServerSessionKeyValuePair() { Key = "testKey", Value = "testValue" });
                repository.Save(session);
                return repository.OneServerSessionWhere(c => c.SessionId == testSessionId);
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
            ServerSession reloaded = because.TheResult.As<ServerSession>();
            because.ItsTrue("KeyValues is not null", reloaded?.KeyValues != null);
            if (reloaded?.KeyValues is List<ServerSessionKeyValuePair> kvps)
            {
                because.ItsTrue("KeyValues count equals 1", kvps.Count == 1);
            }
        })
        .SoBeHappy()
        .UnlessItFailed();
    }

    [UnitTest]
    public void Query()
    {
        string testSessionId = 16.RandomLetters();

        When.A<ServerSessionSchemaRepository>("queries sessions",
            () => new ServerSessionSchemaRepository()
            {
                Database = new SQLiteDatabase(new FileInfo($"./.bam/tests/{nameof(SaveValues)}.sqlite"))
            },
            (repository) =>
            {
                ServerSession session = new ServerSession() { SessionId = testSessionId };
                session.KeyValues.Add(new ServerSessionKeyValuePair() { Key = "testKey", Value = "testValue" });
                repository.Save(session);
                return repository.ServerSessionsWhere(c => c.SessionId == testSessionId).First();
            })
        .TheTest
        .ShouldPass(because =>
        {
            because.TheResult.IsNotNull();
            ServerSession reloaded = because.TheResult.As<ServerSession>();
            because.ItsTrue("KeyValues is not null", reloaded?.KeyValues != null);
            if (reloaded?.KeyValues is List<ServerSessionKeyValuePair> kvps)
            {
                because.ItsTrue("KeyValues count equals 1", kvps.Count == 1);
            }
        })
        .SoBeHappy()
        .UnlessItFailed();
    }
}
