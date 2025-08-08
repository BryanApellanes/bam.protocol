/*
	This file was generated and should not be modified directly (handlebars template)
*/
// Model is Table
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Bam;
using Bam.Data;
using Bam.Data.Qi;

namespace Bam.Protocol.Data.Profile.Dao
{
	// schema = ServerSessionData
	// connection Name = ServerSessionData
	[Serializable]
	[Bam.Data.Table("PrivateKeySet", "ServerSessionData")]
	public partial class PrivateKeySet: Bam.Data.Dao
	{
		public PrivateKeySet():base()
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public PrivateKeySet(DataRow data)
			: base(data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public PrivateKeySet(IDatabase db)
			: base(db)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public PrivateKeySet(IDatabase db, DataRow data)
			: base(db, data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		[Bam.Exclude]
		public static implicit operator PrivateKeySet(DataRow data)
		{
			return new PrivateKeySet(data);
		}

		private void SetChildren()
		{




		} // end SetChildren

	// property: Id, columnName: Id
	[Bam.Exclude]
	[Bam.Data.KeyColumn(Name="Id", DbDataType="BigInt", MaxLength="19")]
	public ulong? Id
	{
		get
		{
			return GetULongValue("Id");
		}
		set
		{
			SetValue("Id", value);
		}
	}
    // property:Uuid, columnName: Uuid	
    [Bam.Data.Column(Name="Uuid", DbDataType="VarChar", MaxLength="4000", AllowNull=false)]
    public string Uuid
    {
        get
        {
            return GetStringValue("Uuid");
        }
        set
        {
            SetValue("Uuid", value);
        }
    }

    // property:Cuid, columnName: Cuid	
    [Bam.Data.Column(Name="Cuid", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string Cuid
    {
        get
        {
            return GetStringValue("Cuid");
        }
        set
        {
            SetValue("Cuid", value);
        }
    }

    // property:PrivateRsaKey, columnName: PrivateRsaKey	
    [Bam.Data.Column(Name="PrivateRsaKey", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string PrivateRsaKey
    {
        get
        {
            return GetStringValue("PrivateRsaKey");
        }
        set
        {
            SetValue("PrivateRsaKey", value);
        }
    }

    // property:PrivateEccKey, columnName: PrivateEccKey	
    [Bam.Data.Column(Name="PrivateEccKey", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string PrivateEccKey
    {
        get
        {
            return GetStringValue("PrivateEccKey");
        }
        set
        {
            SetValue("PrivateEccKey", value);
        }
    }

    // property:Identifier, columnName: Identifier	
    [Bam.Data.Column(Name="Identifier", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string Identifier
    {
        get
        {
            return GetStringValue("Identifier");
        }
        set
        {
            SetValue("Identifier", value);
        }
    }

    // property:PersonHandleHmac, columnName: PersonHandleHmac	
    [Bam.Data.Column(Name="PersonHandleHmac", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string PersonHandleHmac
    {
        get
        {
            return GetStringValue("PersonHandleHmac");
        }
        set
        {
            SetValue("PersonHandleHmac", value);
        }
    }

    // property:PublicRsaKey, columnName: PublicRsaKey	
    [Bam.Data.Column(Name="PublicRsaKey", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string PublicRsaKey
    {
        get
        {
            return GetStringValue("PublicRsaKey");
        }
        set
        {
            SetValue("PublicRsaKey", value);
        }
    }

    // property:PublicEccKey, columnName: PublicEccKey	
    [Bam.Data.Column(Name="PublicEccKey", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string PublicEccKey
    {
        get
        {
            return GetStringValue("PublicEccKey");
        }
        set
        {
            SetValue("PublicEccKey", value);
        }
    }

    // property:Created, columnName: Created	
    [Bam.Data.Column(Name="Created", DbDataType="DateTime", MaxLength="8", AllowNull=true)]
    public DateTime? Created
    {
        get
        {
            return GetDateTimeValue("Created");
        }
        set
        {
            SetValue("Created", value);
        }
    }







		/// <summary>
        /// Gets a query filter that should uniquely identify
        /// the current instance.  The default implementation
        /// compares the Id/key field to the current instance's.
        /// </summary>
		[Bam.Exclude]
		public override IQueryFilter GetUniqueFilter()
		{
			if(UniqueFilterProvider != null)
			{
				return UniqueFilterProvider(this);
			}
			else
			{
				var colFilter = new PrivateKeySetColumns();
				return (colFilter.KeyColumn == GetDbId());
			}
		}

		/// <summary>
        /// Return every record in the PrivateKeySet table.
        /// </summary>
		/// <param name="database">
		/// The database to load from or null
		/// </param>
		public static PrivateKeySetCollection LoadAll(IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<PrivateKeySet>();
            ISqlStringBuilder sql = db.GetSqlStringBuilder();
            sql.Select<PrivateKeySet>();
            var results = new PrivateKeySetCollection(db, sql.ExecuteGetDataTable(db))
            {
                Database = db
            };
            return results;
        }

        /// <summary>
        /// Process all records in batches of the specified size
        /// </summary>
        [Bam.Exclude]
        public static async Task BatchAll(int batchSize, Action<IEnumerable<PrivateKeySet>> batchProcessor, IDatabase database = null)
		{
			await Task.Run(async ()=>
			{
				PrivateKeySetColumns columns = new PrivateKeySetColumns();
				var orderBy = Bam.Data.Order.By<PrivateKeySetColumns>(c => c.KeyColumn, Bam.Data.SortOrder.Ascending);
				var results = Top(batchSize, (c) => c.KeyColumn > 0, orderBy, database);
				while(results.Count > 0)
				{
					await Task.Run(()=>
					{
						batchProcessor(results);
					});
					long topId = results.Select(d => d.Property<long>(columns.KeyColumn.ToString())).ToArray().Largest();
					results = Top(batchSize, (c) => c.KeyColumn > topId, orderBy, database);
				}
			});
		}

		public static PrivateKeySet GetById(uint? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified PrivateKeySet.Id was null");
			return GetById(id.Value, database);
		}

		public static PrivateKeySet GetById(uint id, IDatabase database = null)
		{
			return GetById((ulong)id, database);
		}

		public static PrivateKeySet GetById(int? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified PrivateKeySet.Id was null");
			return GetById(id.Value, database);
		}                                    
                                    
		public static PrivateKeySet GetById(int id, IDatabase database = null)
		{
			return GetById((long)id, database);
		}

		public static PrivateKeySet GetById(long? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified PrivateKeySet.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static PrivateKeySet GetById(long id, IDatabase database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static PrivateKeySet GetById(ulong? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified PrivateKeySet.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static PrivateKeySet GetById(ulong id, IDatabase database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static PrivateKeySet GetByUuid(string uuid, IDatabase database = null)
		{
			return OneWhere(c => Bam.Data.Query.Where("Uuid") == uuid, database);
		}

		public static PrivateKeySet GetByCuid(string cuid, IDatabase database = null)
		{
			return OneWhere(c => Bam.Data.Query.Where("Cuid") == cuid, database);
		}

		[Bam.Exclude]
		public static PrivateKeySetCollection Query(QueryFilter filter, IDatabase database = null)
		{
			return Where(filter, database);
		}

		[Bam.Exclude]
		public static PrivateKeySetCollection Where(QueryFilter filter, IDatabase database = null)
		{
			WhereDelegate<PrivateKeySetColumns> whereDelegate = (c) => filter;
			return Where(whereDelegate, database);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A Func delegate that recieves a PrivateKeySetColumns
		/// and returns a QueryFilter which is the result of any comparisons
		/// between PrivateKeySetColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static PrivateKeySetCollection Where(Func<PrivateKeySetColumns, QueryFilter<PrivateKeySetColumns>> where, OrderBy<PrivateKeySetColumns> orderBy = null, IDatabase database = null)
		{
			database = database ?? Db.For<PrivateKeySet>();
			return new PrivateKeySetCollection(database.GetQuery<PrivateKeySetColumns, PrivateKeySet>(where, orderBy), true);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a PrivateKeySetColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PrivateKeySetColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static PrivateKeySetCollection Where(WhereDelegate<PrivateKeySetColumns> where, IDatabase database = null)
		{
			database = database ?? Db.For<PrivateKeySet>();
			var results = new PrivateKeySetCollection(database, database.GetQuery<PrivateKeySetColumns, PrivateKeySet>(where), true);
			return results;
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a PrivateKeySetColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PrivateKeySetColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PrivateKeySetCollection Where(WhereDelegate<PrivateKeySetColumns> where, OrderBy<PrivateKeySetColumns> orderBy = null, IDatabase database = null)
		{
			database = database ?? Db.For<PrivateKeySet>();
			var results = new PrivateKeySetCollection(database, database.GetQuery<PrivateKeySetColumns, PrivateKeySet>(where, orderBy), true);
			return results;
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`PrivateKeySetColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static PrivateKeySetCollection Where(QiQuery where, IDatabase database = null)
		{
			var results = new PrivateKeySetCollection(database, Select<PrivateKeySetColumns>.From<PrivateKeySet>().Where(where, database));
			return results;
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static PrivateKeySet GetOneWhere(QueryFilter where, IDatabase database = null)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				result = CreateFromFilter(where, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PrivateKeySet OneWhere(QueryFilter where, IDatabase database = null)
		{
			WhereDelegate<PrivateKeySetColumns> whereDelegate = (c) => where;
			var result = Top(1, whereDelegate, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<PrivateKeySetColumns> where, IDatabase database = null)
		{
			SetOneWhere(where, out PrivateKeySet ignore, database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<PrivateKeySetColumns> where, out PrivateKeySet result, IDatabase database = null)
		{
			result = GetOneWhere(where, database);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PrivateKeySet GetOneWhere(WhereDelegate<PrivateKeySetColumns> where, IDatabase database = null)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				PrivateKeySetColumns c = new PrivateKeySetColumns();
				IQueryFilter filter = where(c);
				result = CreateFromFilter(filter, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.  This method is most commonly used to retrieve a
		/// single PrivateKeySet instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a PrivateKeySetColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PrivateKeySetColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PrivateKeySet OneWhere(WhereDelegate<PrivateKeySetColumns> where, IDatabase database = null)
		{
			var result = Top(1, where, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`PrivateKeySetColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static PrivateKeySet OneWhere(QiQuery where, IDatabase database = null)
		{
			var results = Top(1, where, database);
			return OneOrThrow(results);
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a PrivateKeySetColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PrivateKeySetColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PrivateKeySet FirstOneWhere(WhereDelegate<PrivateKeySetColumns> where, IDatabase database = null)
		{
			var results = Top(1, where, database);
			if(results.Count > 0)
			{
				return results[0];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a PrivateKeySetColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PrivateKeySetColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PrivateKeySet FirstOneWhere(WhereDelegate<PrivateKeySetColumns> where, OrderBy<PrivateKeySetColumns> orderBy, IDatabase database = null)
		{
			var results = Top(1, where, orderBy, database);
			if(results.Count > 0)
			{
				return results[0];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Shortcut for Top(1, where, orderBy, database)
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a PrivateKeySetColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PrivateKeySetColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PrivateKeySet FirstOneWhere(QueryFilter where, OrderBy<PrivateKeySetColumns> orderBy = null, IDatabase database = null)
		{
			WhereDelegate<PrivateKeySetColumns> whereDelegate = (c) => where;
			var results = Top(1, whereDelegate, orderBy, database);
			if(results.Count > 0)
			{
				return results[0];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Execute a query and return the specified number
		/// of values. This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="count">The number of values to return.
		/// This value is used in the sql query so no more than this
		/// number of values will be returned by the database.
		/// </param>
		/// <param name="where">A WhereDelegate that recieves a PrivateKeySetColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PrivateKeySetColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PrivateKeySetCollection Top(int count, WhereDelegate<PrivateKeySetColumns> where, IDatabase database = null)
		{
			return Top(count, where, null, database);
		}

		/// <summary>
		/// Execute a query and return the specified number of values.  This method
		/// will issue a sql TOP clause so only the specified number of values
		/// will be returned.
		/// </summary>
		/// <param name="count">The number of values to return.
		/// This value is used in the sql query so no more than this
		/// number of values will be returned by the database.
		/// </param>
		/// <param name="where">A WhereDelegate that recieves a PrivateKeySetColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PrivateKeySetColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static PrivateKeySetCollection Top(int count, WhereDelegate<PrivateKeySetColumns> where, OrderBy<PrivateKeySetColumns> orderBy, IDatabase database = null)
		{
			PrivateKeySetColumns c = new PrivateKeySetColumns();
			IQueryFilter filter = where(c);

			IDatabase db = database ?? Db.For<PrivateKeySet>();
			IQuerySet query = GetQuerySet(db);
			query.Top<PrivateKeySet>(count);
			query.Where(filter);

			if(orderBy != null)
			{
				query.OrderBy<PrivateKeySetColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<PrivateKeySetCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static PrivateKeySetCollection Top(int count, QueryFilter where, IDatabase database)
		{
			return Top(count, where, null, database);
		}
		/// <summary>
		/// Execute a query and return the specified number of values.  This method
		/// will issue a sql TOP clause so only the specified number of values
		/// will be returned.
		/// of values
		/// </summary>
		/// <param name="count">The number of values to return.
		/// This value is used in the sql query so no more than this
		/// number of values will be returned by the database.
		/// </param>
		/// <param name="where">A QueryFilter used to filter the
		/// results
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static PrivateKeySetCollection Top(int count, QueryFilter where, OrderBy<PrivateKeySetColumns> orderBy = null, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<PrivateKeySet>();
			IQuerySet query = GetQuerySet(db);
			query.Top<PrivateKeySet>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy<PrivateKeySetColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<PrivateKeySetCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static PrivateKeySetCollection Top(int count, QueryFilter where, string orderBy = null, SortOrder sortOrder = SortOrder.Ascending, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<PrivateKeySet>();
			IQuerySet query = GetQuerySet(db);
			query.Top<PrivateKeySet>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy(orderBy, sortOrder);
			}

			query.Execute(db);
			var results = query.Results.As<PrivateKeySetCollection>(0);
			results.Database = db;
			return results;
		}

		/// <summary>
		/// Execute a query and return the specified number of values.  This method
		/// will issue a sql TOP clause so only the specified number of values
		/// will be returned.
		/// of values
		/// </summary>
		/// <param name="count">The number of values to return.
		/// This value is used in the sql query so no more than this
		/// number of values will be returned by the database.
		/// </param>
		/// <param name="where">A QueryFilter used to filter the
		/// results
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		public static PrivateKeySetCollection Top(int count, QiQuery where, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<PrivateKeySet>();
			IQuerySet query = GetQuerySet(db);
			query.Top<PrivateKeySet>(count);
			query.Where(where);
			query.Execute(db);
			var results = query.Results.As<PrivateKeySetCollection>(0);
			results.Database = db;
			return results;
		}

		/// <summary>
		/// Return the count of @(Model.ClassName.Pluralize())
		/// </summary>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		public static long Count(IDatabase database = null)
        {
			IDatabase db = database ?? Db.For<PrivateKeySet>();
            IQuerySet query = GetQuerySet(db);
            query.Count<PrivateKeySet>();
            query.Execute(db);
            return (long)query.Results[0].DataRow[0];
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a PrivateKeySetColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PrivateKeySetColumns and other values
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static long Count(WhereDelegate<PrivateKeySetColumns> where, IDatabase database = null)
		{
			PrivateKeySetColumns c = new PrivateKeySetColumns();
			IQueryFilter filter = where(c) ;

			IDatabase db = database ?? Db.For<PrivateKeySet>();
			IQuerySet query = GetQuerySet(db);
			query.Count<PrivateKeySet>();
			query.Where(filter);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		public static long Count(QiQuery where, IDatabase database = null)
		{
		    IDatabase db = database ?? Db.For<PrivateKeySet>();
			IQuerySet query = GetQuerySet(db);
			query.Count<PrivateKeySet>();
			query.Where(where);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		private static PrivateKeySet CreateFromFilter(IQueryFilter filter, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<PrivateKeySet>();
			var dao = new PrivateKeySet();
			filter.Parameters.Each(p=>
			{
				dao.Property(p.ColumnName, p.Value);
			});
			dao.Save(db);
			return dao;
		}

		private static PrivateKeySet OneOrThrow(PrivateKeySetCollection c)
		{
			if(c.Count == 1)
			{
				return c[0];
			}
			else if(c.Count > 1)
			{
				throw new MultipleEntriesFoundException();
			}

			return null;
		}

	}
}
