/*
	This file was generated and should not be modified directly
*/
// model is SchemaDefinition
using System;
using System.Data;
using System.Data.Common;
using Bam;
using Bam.Data;
using Bam.Data.Qi;

namespace Bam.Protocol.Data.Server.Dao
{
	// schema = ServerSessionSchema
    public static class ServerSessionSchemaContext
    {
		public static string ConnectionName
		{
			get
			{
				return "ServerSessionSchema";
			}
		}

		public static IDatabase Db
		{
			get
			{
				return Bam.Data.Db.For(ConnectionName);
			}
		}


	public class ServerAccountDataQueryContext
	{
			public ServerAccountDataCollection Where(WhereDelegate<ServerAccountDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerAccountData.Where(where, db);
			}
		   
			public ServerAccountDataCollection Where(WhereDelegate<ServerAccountDataColumns> where, OrderBy<ServerAccountDataColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerAccountData.Where(where, orderBy, db);
			}

			public ServerAccountData OneWhere(WhereDelegate<ServerAccountDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerAccountData.OneWhere(where, db);
			}

			public static ServerAccountData GetOneWhere(WhereDelegate<ServerAccountDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerAccountData.GetOneWhere(where, db);
			}
		
			public ServerAccountData FirstOneWhere(WhereDelegate<ServerAccountDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerAccountData.FirstOneWhere(where, db);
			}

			public ServerAccountDataCollection Top(int count, WhereDelegate<ServerAccountDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerAccountData.Top(count, where, db);
			}

			public ServerAccountDataCollection Top(int count, WhereDelegate<ServerAccountDataColumns> where, OrderBy<ServerAccountDataColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerAccountData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<ServerAccountDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerAccountData.Count(where, db);
			}
	}

	static ServerAccountDataQueryContext _serverAccountDatas;
	static object _serverAccountDatasLock = new object();
	public static ServerAccountDataQueryContext ServerAccountDatas
	{
		get
		{
			return _serverAccountDatasLock.DoubleCheckLock<ServerAccountDataQueryContext>(ref _serverAccountDatas, () => new ServerAccountDataQueryContext());
		}
	}
	public class ServerSessionQueryContext
	{
			public ServerSessionCollection Where(WhereDelegate<ServerSessionColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSession.Where(where, db);
			}
		   
			public ServerSessionCollection Where(WhereDelegate<ServerSessionColumns> where, OrderBy<ServerSessionColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSession.Where(where, orderBy, db);
			}

			public ServerSession OneWhere(WhereDelegate<ServerSessionColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSession.OneWhere(where, db);
			}

			public static ServerSession GetOneWhere(WhereDelegate<ServerSessionColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSession.GetOneWhere(where, db);
			}
		
			public ServerSession FirstOneWhere(WhereDelegate<ServerSessionColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSession.FirstOneWhere(where, db);
			}

			public ServerSessionCollection Top(int count, WhereDelegate<ServerSessionColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSession.Top(count, where, db);
			}

			public ServerSessionCollection Top(int count, WhereDelegate<ServerSessionColumns> where, OrderBy<ServerSessionColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSession.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<ServerSessionColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSession.Count(where, db);
			}
	}

	static ServerSessionQueryContext _serverSessions;
	static object _serverSessionsLock = new object();
	public static ServerSessionQueryContext ServerSessions
	{
		get
		{
			return _serverSessionsLock.DoubleCheckLock<ServerSessionQueryContext>(ref _serverSessions, () => new ServerSessionQueryContext());
		}
	}
	public class ServerSessionKeyValuePairQueryContext
	{
			public ServerSessionKeyValuePairCollection Where(WhereDelegate<ServerSessionKeyValuePairColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.Where(where, db);
			}
		   
			public ServerSessionKeyValuePairCollection Where(WhereDelegate<ServerSessionKeyValuePairColumns> where, OrderBy<ServerSessionKeyValuePairColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.Where(where, orderBy, db);
			}

			public ServerSessionKeyValuePair OneWhere(WhereDelegate<ServerSessionKeyValuePairColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.OneWhere(where, db);
			}

			public static ServerSessionKeyValuePair GetOneWhere(WhereDelegate<ServerSessionKeyValuePairColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.GetOneWhere(where, db);
			}
		
			public ServerSessionKeyValuePair FirstOneWhere(WhereDelegate<ServerSessionKeyValuePairColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.FirstOneWhere(where, db);
			}

			public ServerSessionKeyValuePairCollection Top(int count, WhereDelegate<ServerSessionKeyValuePairColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.Top(count, where, db);
			}

			public ServerSessionKeyValuePairCollection Top(int count, WhereDelegate<ServerSessionKeyValuePairColumns> where, OrderBy<ServerSessionKeyValuePairColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<ServerSessionKeyValuePairColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.Count(where, db);
			}
	}

	static ServerSessionKeyValuePairQueryContext _serverSessionKeyValuePairs;
	static object _serverSessionKeyValuePairsLock = new object();
	public static ServerSessionKeyValuePairQueryContext ServerSessionKeyValuePairs
	{
		get
		{
			return _serverSessionKeyValuePairsLock.DoubleCheckLock<ServerSessionKeyValuePairQueryContext>(ref _serverSessionKeyValuePairs, () => new ServerSessionKeyValuePairQueryContext());
		}
	}
    }
}																								
