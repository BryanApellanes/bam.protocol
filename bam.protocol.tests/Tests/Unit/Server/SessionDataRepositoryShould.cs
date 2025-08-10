
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
    public async Task SaveValues()
    {
        string testSessionId = 16.RandomLetters();
        FileInfo dbFile = new FileInfo($"./.bam/tests/{nameof(SaveValues)}.sqlite");
        SQLiteDatabase database = new SQLiteDatabase(dbFile);
        ServerSessionSchemaRepository repository = new ServerSessionSchemaRepository()
        {
            Database = database
        };
        ServerSession session = new ServerSession()
        {
            SessionId = testSessionId
        };
        session.KeyValues.Add(new ServerSessionKeyValuePair()
        {
            Key = "testKey",
            Value = "testValue"
        });
        session = repository.Save(session);
        ServerSession reloaded = repository.Retrieve<ServerSession>(session.Id);
        reloaded.ShouldNotBeNull();
        reloaded.KeyValues.ShouldNotBeNull();
        reloaded.KeyValues.Count.ShouldBeEqualTo(1);
    }
    
    [UnitTest]
    public async Task RetrieveBySessionId()
    {
        string testSessionId = 16.RandomLetters();
        FileInfo dbFile = new FileInfo($"./.bam/tests/{nameof(SaveValues)}.sqlite");
        SQLiteDatabase database = new SQLiteDatabase(dbFile);
        ServerSessionSchemaRepository repository = new ServerSessionSchemaRepository()
        {
            Database = database
        };
        ServerSession session = new ServerSession()
        {
            SessionId = testSessionId
        };
        session.KeyValues.Add(new ServerSessionKeyValuePair()
        {
            Key = "testKey",
            Value = "testValue"
        });
        session = repository.Save(session);
        
        
        ServerSession reloaded = repository.OneServerSessionWhere(c => c.SessionId == testSessionId);

        reloaded.ShouldNotBeNull();
        reloaded.KeyValues.ShouldNotBeNull();
        reloaded.KeyValues.Count.ShouldBeEqualTo(1);
    }
    
    [UnitTest]
    public async Task Query()
    {
        string testSessionId = 16.RandomLetters();
        FileInfo dbFile = new FileInfo($"./.bam/tests/{nameof(SaveValues)}.sqlite");
        SQLiteDatabase database = new SQLiteDatabase(dbFile);
        ServerSessionSchemaRepository repository = new ServerSessionSchemaRepository()
        {
            Database = database
        };
        ServerSession session = new ServerSession()
        {
            SessionId = testSessionId
        };
        session.KeyValues.Add(new ServerSessionKeyValuePair()
        {
            Key = "testKey",
            Value = "testValue"
        });
        session = repository.Save(session);

        ServerSession reloaded = repository.ServerSessionsWhere(c => c.SessionId == testSessionId).First();

        reloaded.ShouldNotBeNull();
        reloaded.KeyValues.ShouldNotBeNull();
        reloaded.KeyValues.Count.ShouldBeEqualTo(1);
    }
}