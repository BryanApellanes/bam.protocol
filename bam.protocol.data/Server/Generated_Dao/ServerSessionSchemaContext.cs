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


	public class AccountDataQueryContext
	{
			public AccountDataCollection Where(WhereDelegate<AccountDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.AccountData.Where(where, db);
			}
		   
			public AccountDataCollection Where(WhereDelegate<AccountDataColumns> where, OrderBy<AccountDataColumns> orderBy = null!, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.AccountData.Where(where, orderBy, db);
			}

			public AccountData OneWhere(WhereDelegate<AccountDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.AccountData.OneWhere(where, db);
			}

			public static AccountData GetOneWhere(WhereDelegate<AccountDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.AccountData.GetOneWhere(where, db);
			}
		
			public AccountData FirstOneWhere(WhereDelegate<AccountDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.AccountData.FirstOneWhere(where, db);
			}

			public AccountDataCollection Top(int count, WhereDelegate<AccountDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.AccountData.Top(count, where, db);
			}

			public AccountDataCollection Top(int count, WhereDelegate<AccountDataColumns> where, OrderBy<AccountDataColumns> orderBy, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.AccountData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<AccountDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.AccountData.Count(where, db);
			}
	}

	static AccountDataQueryContext _accountDatas = null!;
	static object _accountDatasLock = new object();
	public static AccountDataQueryContext AccountDatas
	{
		get
		{
			return _accountDatasLock.DoubleCheckLock<AccountDataQueryContext>(ref _accountDatas, () => new AccountDataQueryContext());
		}
	}
	public class InboxDataQueryContext
	{
			public InboxDataCollection Where(WhereDelegate<InboxDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.InboxData.Where(where, db);
			}
		   
			public InboxDataCollection Where(WhereDelegate<InboxDataColumns> where, OrderBy<InboxDataColumns> orderBy = null!, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.InboxData.Where(where, orderBy, db);
			}

			public InboxData OneWhere(WhereDelegate<InboxDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.InboxData.OneWhere(where, db);
			}

			public static InboxData GetOneWhere(WhereDelegate<InboxDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.InboxData.GetOneWhere(where, db);
			}
		
			public InboxData FirstOneWhere(WhereDelegate<InboxDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.InboxData.FirstOneWhere(where, db);
			}

			public InboxDataCollection Top(int count, WhereDelegate<InboxDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.InboxData.Top(count, where, db);
			}

			public InboxDataCollection Top(int count, WhereDelegate<InboxDataColumns> where, OrderBy<InboxDataColumns> orderBy, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.InboxData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<InboxDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.InboxData.Count(where, db);
			}
	}

	static InboxDataQueryContext _inboxDatas = null!;
	static object _inboxDatasLock = new object();
	public static InboxDataQueryContext InboxDatas
	{
		get
		{
			return _inboxDatasLock.DoubleCheckLock<InboxDataQueryContext>(ref _inboxDatas, () => new InboxDataQueryContext());
		}
	}
	public class OutboxDataQueryContext
	{
			public OutboxDataCollection Where(WhereDelegate<OutboxDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.OutboxData.Where(where, db);
			}
		   
			public OutboxDataCollection Where(WhereDelegate<OutboxDataColumns> where, OrderBy<OutboxDataColumns> orderBy = null!, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.OutboxData.Where(where, orderBy, db);
			}

			public OutboxData OneWhere(WhereDelegate<OutboxDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.OutboxData.OneWhere(where, db);
			}

			public static OutboxData GetOneWhere(WhereDelegate<OutboxDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.OutboxData.GetOneWhere(where, db);
			}
		
			public OutboxData FirstOneWhere(WhereDelegate<OutboxDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.OutboxData.FirstOneWhere(where, db);
			}

			public OutboxDataCollection Top(int count, WhereDelegate<OutboxDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.OutboxData.Top(count, where, db);
			}

			public OutboxDataCollection Top(int count, WhereDelegate<OutboxDataColumns> where, OrderBy<OutboxDataColumns> orderBy, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.OutboxData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<OutboxDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.OutboxData.Count(where, db);
			}
	}

	static OutboxDataQueryContext _outboxDatas = null!;
	static object _outboxDatasLock = new object();
	public static OutboxDataQueryContext OutboxDatas
	{
		get
		{
			return _outboxDatasLock.DoubleCheckLock<OutboxDataQueryContext>(ref _outboxDatas, () => new OutboxDataQueryContext());
		}
	}
	public class ServerAccountDataQueryContext
	{
			public ServerAccountDataCollection Where(WhereDelegate<ServerAccountDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerAccountData.Where(where, db);
			}
		   
			public ServerAccountDataCollection Where(WhereDelegate<ServerAccountDataColumns> where, OrderBy<ServerAccountDataColumns> orderBy = null!, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerAccountData.Where(where, orderBy, db);
			}

			public ServerAccountData OneWhere(WhereDelegate<ServerAccountDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerAccountData.OneWhere(where, db);
			}

			public static ServerAccountData GetOneWhere(WhereDelegate<ServerAccountDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerAccountData.GetOneWhere(where, db);
			}
		
			public ServerAccountData FirstOneWhere(WhereDelegate<ServerAccountDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerAccountData.FirstOneWhere(where, db);
			}

			public ServerAccountDataCollection Top(int count, WhereDelegate<ServerAccountDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerAccountData.Top(count, where, db);
			}

			public ServerAccountDataCollection Top(int count, WhereDelegate<ServerAccountDataColumns> where, OrderBy<ServerAccountDataColumns> orderBy, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerAccountData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<ServerAccountDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerAccountData.Count(where, db);
			}
	}

	static ServerAccountDataQueryContext _serverAccountDatas = null!;
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
			public ServerSessionCollection Where(WhereDelegate<ServerSessionColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSession.Where(where, db);
			}
		   
			public ServerSessionCollection Where(WhereDelegate<ServerSessionColumns> where, OrderBy<ServerSessionColumns> orderBy = null!, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSession.Where(where, orderBy, db);
			}

			public ServerSession OneWhere(WhereDelegate<ServerSessionColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSession.OneWhere(where, db);
			}

			public static ServerSession GetOneWhere(WhereDelegate<ServerSessionColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSession.GetOneWhere(where, db);
			}
		
			public ServerSession FirstOneWhere(WhereDelegate<ServerSessionColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSession.FirstOneWhere(where, db);
			}

			public ServerSessionCollection Top(int count, WhereDelegate<ServerSessionColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSession.Top(count, where, db);
			}

			public ServerSessionCollection Top(int count, WhereDelegate<ServerSessionColumns> where, OrderBy<ServerSessionColumns> orderBy, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSession.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<ServerSessionColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSession.Count(where, db);
			}
	}

	static ServerSessionQueryContext _serverSessions = null!;
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
			public ServerSessionKeyValuePairCollection Where(WhereDelegate<ServerSessionKeyValuePairColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.Where(where, db);
			}
		   
			public ServerSessionKeyValuePairCollection Where(WhereDelegate<ServerSessionKeyValuePairColumns> where, OrderBy<ServerSessionKeyValuePairColumns> orderBy = null!, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.Where(where, orderBy, db);
			}

			public ServerSessionKeyValuePair OneWhere(WhereDelegate<ServerSessionKeyValuePairColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.OneWhere(where, db);
			}

			public static ServerSessionKeyValuePair GetOneWhere(WhereDelegate<ServerSessionKeyValuePairColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.GetOneWhere(where, db);
			}
		
			public ServerSessionKeyValuePair FirstOneWhere(WhereDelegate<ServerSessionKeyValuePairColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.FirstOneWhere(where, db);
			}

			public ServerSessionKeyValuePairCollection Top(int count, WhereDelegate<ServerSessionKeyValuePairColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.Top(count, where, db);
			}

			public ServerSessionKeyValuePairCollection Top(int count, WhereDelegate<ServerSessionKeyValuePairColumns> where, OrderBy<ServerSessionKeyValuePairColumns> orderBy, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<ServerSessionKeyValuePairColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.Count(where, db);
			}
	}

	static ServerSessionKeyValuePairQueryContext _serverSessionKeyValuePairs = null!;
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
