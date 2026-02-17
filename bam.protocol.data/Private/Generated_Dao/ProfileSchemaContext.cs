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

namespace Bam.Protocol.Data.Private.Dao
{
	// schema = ProfileSchema
    public static class ProfileSchemaContext
    {
		public static string ConnectionName
		{
			get
			{
				return "ProfileSchema";
			}
		}

		public static IDatabase Db
		{
			get
			{
				return Bam.Data.Db.For(ConnectionName);
			}
		}


	public class EccPrivateKeyDataQueryContext
	{
			public EccPrivateKeyDataCollection Where(WhereDelegate<EccPrivateKeyDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.Where(where, db);
			}
		   
			public EccPrivateKeyDataCollection Where(WhereDelegate<EccPrivateKeyDataColumns> where, OrderBy<EccPrivateKeyDataColumns> orderBy = null!, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.Where(where, orderBy, db);
			}

			public EccPrivateKeyData OneWhere(WhereDelegate<EccPrivateKeyDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.OneWhere(where, db);
			}

			public static EccPrivateKeyData GetOneWhere(WhereDelegate<EccPrivateKeyDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.GetOneWhere(where, db);
			}
		
			public EccPrivateKeyData FirstOneWhere(WhereDelegate<EccPrivateKeyDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.FirstOneWhere(where, db);
			}

			public EccPrivateKeyDataCollection Top(int count, WhereDelegate<EccPrivateKeyDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.Top(count, where, db);
			}

			public EccPrivateKeyDataCollection Top(int count, WhereDelegate<EccPrivateKeyDataColumns> where, OrderBy<EccPrivateKeyDataColumns> orderBy, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<EccPrivateKeyDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.Count(where, db);
			}
	}

	static EccPrivateKeyDataQueryContext _eccPrivateKeyDatas = null!;
	static object _eccPrivateKeyDatasLock = new object();
	public static EccPrivateKeyDataQueryContext EccPrivateKeyDatas
	{
		get
		{
			return _eccPrivateKeyDatasLock.DoubleCheckLock<EccPrivateKeyDataQueryContext>(ref _eccPrivateKeyDatas, () => new EccPrivateKeyDataQueryContext());
		}
	}
	public class PrivateKeySetDataQueryContext
	{
			public PrivateKeySetDataCollection Where(WhereDelegate<PrivateKeySetDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.PrivateKeySetData.Where(where, db);
			}
		   
			public PrivateKeySetDataCollection Where(WhereDelegate<PrivateKeySetDataColumns> where, OrderBy<PrivateKeySetDataColumns> orderBy = null!, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.PrivateKeySetData.Where(where, orderBy, db);
			}

			public PrivateKeySetData OneWhere(WhereDelegate<PrivateKeySetDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.PrivateKeySetData.OneWhere(where, db);
			}

			public static PrivateKeySetData GetOneWhere(WhereDelegate<PrivateKeySetDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.PrivateKeySetData.GetOneWhere(where, db);
			}
		
			public PrivateKeySetData FirstOneWhere(WhereDelegate<PrivateKeySetDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.PrivateKeySetData.FirstOneWhere(where, db);
			}

			public PrivateKeySetDataCollection Top(int count, WhereDelegate<PrivateKeySetDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.PrivateKeySetData.Top(count, where, db);
			}

			public PrivateKeySetDataCollection Top(int count, WhereDelegate<PrivateKeySetDataColumns> where, OrderBy<PrivateKeySetDataColumns> orderBy, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.PrivateKeySetData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<PrivateKeySetDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.PrivateKeySetData.Count(where, db);
			}
	}

	static PrivateKeySetDataQueryContext _privateKeySetDatas = null!;
	static object _privateKeySetDatasLock = new object();
	public static PrivateKeySetDataQueryContext PrivateKeySetDatas
	{
		get
		{
			return _privateKeySetDatasLock.DoubleCheckLock<PrivateKeySetDataQueryContext>(ref _privateKeySetDatas, () => new PrivateKeySetDataQueryContext());
		}
	}
	public class RsaPrivateKeyDataQueryContext
	{
			public RsaPrivateKeyDataCollection Where(WhereDelegate<RsaPrivateKeyDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.Where(where, db);
			}
		   
			public RsaPrivateKeyDataCollection Where(WhereDelegate<RsaPrivateKeyDataColumns> where, OrderBy<RsaPrivateKeyDataColumns> orderBy = null!, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.Where(where, orderBy, db);
			}

			public RsaPrivateKeyData OneWhere(WhereDelegate<RsaPrivateKeyDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.OneWhere(where, db);
			}

			public static RsaPrivateKeyData GetOneWhere(WhereDelegate<RsaPrivateKeyDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.GetOneWhere(where, db);
			}
		
			public RsaPrivateKeyData FirstOneWhere(WhereDelegate<RsaPrivateKeyDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.FirstOneWhere(where, db);
			}

			public RsaPrivateKeyDataCollection Top(int count, WhereDelegate<RsaPrivateKeyDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.Top(count, where, db);
			}

			public RsaPrivateKeyDataCollection Top(int count, WhereDelegate<RsaPrivateKeyDataColumns> where, OrderBy<RsaPrivateKeyDataColumns> orderBy, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<RsaPrivateKeyDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.Count(where, db);
			}
	}

	static RsaPrivateKeyDataQueryContext _rsaPrivateKeyDatas = null!;
	static object _rsaPrivateKeyDatasLock = new object();
	public static RsaPrivateKeyDataQueryContext RsaPrivateKeyDatas
	{
		get
		{
			return _rsaPrivateKeyDatasLock.DoubleCheckLock<RsaPrivateKeyDataQueryContext>(ref _rsaPrivateKeyDatas, () => new RsaPrivateKeyDataQueryContext());
		}
	}
    }
}																								
