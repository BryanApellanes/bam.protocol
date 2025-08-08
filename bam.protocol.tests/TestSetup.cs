using Bam.Data.SQLite;
using Bam.Protocol.Data.Server;
using Bam.Protocol.Data.Server.Dao.Repository;

namespace Bam.Protocol.Tests;

public class TestSetup
{
    public static ServerSessionDataRepository CreateTestData(string testSessionId, Dictionary<string, string> keyValues,  string fileName)
    {
        FileInfo dbFile = new FileInfo($"./.bam/tests/{fileName}.sqlite");
        SQLiteDatabase database = new SQLiteDatabase(dbFile);
        ServerSessionDataRepository repository = new ServerSessionDataRepository()
        {
            Database = database
        };
        ServerSession session = new ServerSession()
        {
            SessionId = testSessionId
        };
        foreach (string key in keyValues.Keys)
        {
            session.KeyValues.Add(new ServerSessionKeyValuePair()
            {
                Key = key,
                Value = keyValues[key]
            });
        }
        session = repository.Save(session);
        return repository;
    }
}