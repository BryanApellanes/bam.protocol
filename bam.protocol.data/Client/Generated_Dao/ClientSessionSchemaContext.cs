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

namespace Bam.Protocol.Data.Client.Dao
{
	// schema = ClientSessionSchema
    public static class ClientSessionSchemaContext
    {
		public static string ConnectionName
		{
			get
			{
				return "ClientSessionSchema";
			}
		}

		public static IDatabase Db
		{
			get
			{
				return Bam.Data.Db.For(ConnectionName);
			}
		}


	public class ClientKeySetDataQueryContext
	{
			public ClientKeySetDataCollection Where(WhereDelegate<ClientKeySetDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientKeySetData.Where(where, db);
			}
		   
			public ClientKeySetDataCollection Where(WhereDelegate<ClientKeySetDataColumns> where, OrderBy<ClientKeySetDataColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientKeySetData.Where(where, orderBy, db);
			}

			public ClientKeySetData OneWhere(WhereDelegate<ClientKeySetDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientKeySetData.OneWhere(where, db);
			}

			public static ClientKeySetData GetOneWhere(WhereDelegate<ClientKeySetDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientKeySetData.GetOneWhere(where, db);
			}
		
			public ClientKeySetData FirstOneWhere(WhereDelegate<ClientKeySetDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientKeySetData.FirstOneWhere(where, db);
			}

			public ClientKeySetDataCollection Top(int count, WhereDelegate<ClientKeySetDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientKeySetData.Top(count, where, db);
			}

			public ClientKeySetDataCollection Top(int count, WhereDelegate<ClientKeySetDataColumns> where, OrderBy<ClientKeySetDataColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientKeySetData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<ClientKeySetDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientKeySetData.Count(where, db);
			}
	}

	static ClientKeySetDataQueryContext _clientKeySetDatas;
	static object _clientKeySetDatasLock = new object();
	public static ClientKeySetDataQueryContext ClientKeySetDatas
	{
		get
		{
			return _clientKeySetDatasLock.DoubleCheckLock<ClientKeySetDataQueryContext>(ref _clientKeySetDatas, () => new ClientKeySetDataQueryContext());
		}
	}
	public class ClientSessionDataQueryContext
	{
			public ClientSessionDataCollection Where(WhereDelegate<ClientSessionDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientSessionData.Where(where, db);
			}
		   
			public ClientSessionDataCollection Where(WhereDelegate<ClientSessionDataColumns> where, OrderBy<ClientSessionDataColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientSessionData.Where(where, orderBy, db);
			}

			public ClientSessionData OneWhere(WhereDelegate<ClientSessionDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientSessionData.OneWhere(where, db);
			}

			public static ClientSessionData GetOneWhere(WhereDelegate<ClientSessionDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientSessionData.GetOneWhere(where, db);
			}
		
			public ClientSessionData FirstOneWhere(WhereDelegate<ClientSessionDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientSessionData.FirstOneWhere(where, db);
			}

			public ClientSessionDataCollection Top(int count, WhereDelegate<ClientSessionDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientSessionData.Top(count, where, db);
			}

			public ClientSessionDataCollection Top(int count, WhereDelegate<ClientSessionDataColumns> where, OrderBy<ClientSessionDataColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientSessionData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<ClientSessionDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientSessionData.Count(where, db);
			}
	}

	static ClientSessionDataQueryContext _clientSessionDatas;
	static object _clientSessionDatasLock = new object();
	public static ClientSessionDataQueryContext ClientSessionDatas
	{
		get
		{
			return _clientSessionDatasLock.DoubleCheckLock<ClientSessionDataQueryContext>(ref _clientSessionDatas, () => new ClientSessionDataQueryContext());
		}
	}
	public class ClientSessionKeyValueQueryContext
	{
			public ClientSessionKeyValueCollection Where(WhereDelegate<ClientSessionKeyValueColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.Where(where, db);
			}
		   
			public ClientSessionKeyValueCollection Where(WhereDelegate<ClientSessionKeyValueColumns> where, OrderBy<ClientSessionKeyValueColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.Where(where, orderBy, db);
			}

			public ClientSessionKeyValue OneWhere(WhereDelegate<ClientSessionKeyValueColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.OneWhere(where, db);
			}

			public static ClientSessionKeyValue GetOneWhere(WhereDelegate<ClientSessionKeyValueColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.GetOneWhere(where, db);
			}
		
			public ClientSessionKeyValue FirstOneWhere(WhereDelegate<ClientSessionKeyValueColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.FirstOneWhere(where, db);
			}

			public ClientSessionKeyValueCollection Top(int count, WhereDelegate<ClientSessionKeyValueColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.Top(count, where, db);
			}

			public ClientSessionKeyValueCollection Top(int count, WhereDelegate<ClientSessionKeyValueColumns> where, OrderBy<ClientSessionKeyValueColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<ClientSessionKeyValueColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.Count(where, db);
			}
	}

	static ClientSessionKeyValueQueryContext _clientSessionKeyValues;
	static object _clientSessionKeyValuesLock = new object();
	public static ClientSessionKeyValueQueryContext ClientSessionKeyValues
	{
		get
		{
			return _clientSessionKeyValuesLock.DoubleCheckLock<ClientSessionKeyValueQueryContext>(ref _clientSessionKeyValues, () => new ClientSessionKeyValueQueryContext());
		}
	}
    }
}																								
