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
	// schema = ServerSessionData
    public static class ServerSessionDataContext
    {
		public static string ConnectionName
		{
			get
			{
				return "ServerSessionData";
			}
		}

		public static IDatabase Db
		{
			get
			{
				return Bam.Data.Db.For(ConnectionName);
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
	public class ServerSessionKeyValueQueryContext
	{
			public ServerSessionKeyValueCollection Where(WhereDelegate<ServerSessionKeyValueColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.Where(where, db);
			}
		   
			public ServerSessionKeyValueCollection Where(WhereDelegate<ServerSessionKeyValueColumns> where, OrderBy<ServerSessionKeyValueColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.Where(where, orderBy, db);
			}

			public ServerSessionKeyValue OneWhere(WhereDelegate<ServerSessionKeyValueColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.OneWhere(where, db);
			}

			public static ServerSessionKeyValue GetOneWhere(WhereDelegate<ServerSessionKeyValueColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.GetOneWhere(where, db);
			}
		
			public ServerSessionKeyValue FirstOneWhere(WhereDelegate<ServerSessionKeyValueColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.FirstOneWhere(where, db);
			}

			public ServerSessionKeyValueCollection Top(int count, WhereDelegate<ServerSessionKeyValueColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.Top(count, where, db);
			}

			public ServerSessionKeyValueCollection Top(int count, WhereDelegate<ServerSessionKeyValueColumns> where, OrderBy<ServerSessionKeyValueColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<ServerSessionKeyValueColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.Count(where, db);
			}
	}

	static ServerSessionKeyValueQueryContext _serverSessionKeyValues;
	static object _serverSessionKeyValuesLock = new object();
	public static ServerSessionKeyValueQueryContext ServerSessionKeyValues
	{
		get
		{
			return _serverSessionKeyValuesLock.DoubleCheckLock<ServerSessionKeyValueQueryContext>(ref _serverSessionKeyValues, () => new ServerSessionKeyValueQueryContext());
		}
	}
    }
}																								
