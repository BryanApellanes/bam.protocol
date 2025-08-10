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
	// schema = ProfileSchema
	// connection Name = ProfileSchema
	[Serializable]
	[Bam.Data.Table("DeviceCertificateData", "ProfileSchema")]
	public partial class DeviceCertificateData: Bam.Data.Dao
	{
		public DeviceCertificateData():base()
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public DeviceCertificateData(DataRow data)
			: base(data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public DeviceCertificateData(IDatabase db)
			: base(db)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public DeviceCertificateData(IDatabase db, DataRow data)
			: base(db, data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		[Bam.Exclude]
		public static implicit operator DeviceCertificateData(DataRow data)
		{
			return new DeviceCertificateData(data);
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

    // property:DeviceHandle, columnName: DeviceHandle	
    [Bam.Data.Column(Name="DeviceHandle", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string DeviceHandle
    {
        get
        {
            return GetStringValue("DeviceHandle");
        }
        set
        {
            SetValue("DeviceHandle", value);
        }
    }

    // property:CertificateHash, columnName: CertificateHash	
    [Bam.Data.Column(Name="CertificateHash", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string CertificateHash
    {
        get
        {
            return GetStringValue("CertificateHash");
        }
        set
        {
            SetValue("CertificateHash", value);
        }
    }

    // property:CertificateHashAlgorithm, columnName: CertificateHashAlgorithm	
    [Bam.Data.Column(Name="CertificateHashAlgorithm", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string CertificateHashAlgorithm
    {
        get
        {
            return GetStringValue("CertificateHashAlgorithm");
        }
        set
        {
            SetValue("CertificateHashAlgorithm", value);
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
				var colFilter = new DeviceCertificateDataColumns();
				return (colFilter.KeyColumn == GetDbId());
			}
		}

		/// <summary>
        /// Return every record in the DeviceCertificateData table.
        /// </summary>
		/// <param name="database">
		/// The database to load from or null
		/// </param>
		public static DeviceCertificateDataCollection LoadAll(IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<DeviceCertificateData>();
            ISqlStringBuilder sql = db.GetSqlStringBuilder();
            sql.Select<DeviceCertificateData>();
            var results = new DeviceCertificateDataCollection(db, sql.ExecuteGetDataTable(db))
            {
                Database = db
            };
            return results;
        }

        /// <summary>
        /// Process all records in batches of the specified size
        /// </summary>
        [Bam.Exclude]
        public static async Task BatchAll(int batchSize, Action<IEnumerable<DeviceCertificateData>> batchProcessor, IDatabase database = null)
		{
			await Task.Run(async ()=>
			{
				DeviceCertificateDataColumns columns = new DeviceCertificateDataColumns();
				var orderBy = Bam.Data.Order.By<DeviceCertificateDataColumns>(c => c.KeyColumn, Bam.Data.SortOrder.Ascending);
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

		public static DeviceCertificateData GetById(uint? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified DeviceCertificateData.Id was null");
			return GetById(id.Value, database);
		}

		public static DeviceCertificateData GetById(uint id, IDatabase database = null)
		{
			return GetById((ulong)id, database);
		}

		public static DeviceCertificateData GetById(int? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified DeviceCertificateData.Id was null");
			return GetById(id.Value, database);
		}                                    
                                    
		public static DeviceCertificateData GetById(int id, IDatabase database = null)
		{
			return GetById((long)id, database);
		}

		public static DeviceCertificateData GetById(long? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified DeviceCertificateData.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static DeviceCertificateData GetById(long id, IDatabase database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static DeviceCertificateData GetById(ulong? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified DeviceCertificateData.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static DeviceCertificateData GetById(ulong id, IDatabase database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static DeviceCertificateData GetByUuid(string uuid, IDatabase database = null)
		{
			return OneWhere(c => Bam.Data.Query.Where("Uuid") == uuid, database);
		}

		public static DeviceCertificateData GetByCuid(string cuid, IDatabase database = null)
		{
			return OneWhere(c => Bam.Data.Query.Where("Cuid") == cuid, database);
		}

		[Bam.Exclude]
		public static DeviceCertificateDataCollection Query(QueryFilter filter, IDatabase database = null)
		{
			return Where(filter, database);
		}

		[Bam.Exclude]
		public static DeviceCertificateDataCollection Where(QueryFilter filter, IDatabase database = null)
		{
			WhereDelegate<DeviceCertificateDataColumns> whereDelegate = (c) => filter;
			return Where(whereDelegate, database);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A Func delegate that recieves a DeviceCertificateDataColumns
		/// and returns a QueryFilter which is the result of any comparisons
		/// between DeviceCertificateDataColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static DeviceCertificateDataCollection Where(Func<DeviceCertificateDataColumns, QueryFilter<DeviceCertificateDataColumns>> where, OrderBy<DeviceCertificateDataColumns> orderBy = null, IDatabase database = null)
		{
			database = database ?? Db.For<DeviceCertificateData>();
			return new DeviceCertificateDataCollection(database.GetQuery<DeviceCertificateDataColumns, DeviceCertificateData>(where, orderBy), true);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a DeviceCertificateDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceCertificateDataColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static DeviceCertificateDataCollection Where(WhereDelegate<DeviceCertificateDataColumns> where, IDatabase database = null)
		{
			database = database ?? Db.For<DeviceCertificateData>();
			var results = new DeviceCertificateDataCollection(database, database.GetQuery<DeviceCertificateDataColumns, DeviceCertificateData>(where), true);
			return results;
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a DeviceCertificateDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceCertificateDataColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static DeviceCertificateDataCollection Where(WhereDelegate<DeviceCertificateDataColumns> where, OrderBy<DeviceCertificateDataColumns> orderBy = null, IDatabase database = null)
		{
			database = database ?? Db.For<DeviceCertificateData>();
			var results = new DeviceCertificateDataCollection(database, database.GetQuery<DeviceCertificateDataColumns, DeviceCertificateData>(where, orderBy), true);
			return results;
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`DeviceCertificateDataColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static DeviceCertificateDataCollection Where(QiQuery where, IDatabase database = null)
		{
			var results = new DeviceCertificateDataCollection(database, Select<DeviceCertificateDataColumns>.From<DeviceCertificateData>().Where(where, database));
			return results;
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static DeviceCertificateData GetOneWhere(QueryFilter where, IDatabase database = null)
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
		public static DeviceCertificateData OneWhere(QueryFilter where, IDatabase database = null)
		{
			WhereDelegate<DeviceCertificateDataColumns> whereDelegate = (c) => where;
			var result = Top(1, whereDelegate, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<DeviceCertificateDataColumns> where, IDatabase database = null)
		{
			SetOneWhere(where, out DeviceCertificateData ignore, database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<DeviceCertificateDataColumns> where, out DeviceCertificateData result, IDatabase database = null)
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
		public static DeviceCertificateData GetOneWhere(WhereDelegate<DeviceCertificateDataColumns> where, IDatabase database = null)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				DeviceCertificateDataColumns c = new DeviceCertificateDataColumns();
				IQueryFilter filter = where(c);
				result = CreateFromFilter(filter, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.  This method is most commonly used to retrieve a
		/// single DeviceCertificateData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a DeviceCertificateDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceCertificateDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static DeviceCertificateData OneWhere(WhereDelegate<DeviceCertificateDataColumns> where, IDatabase database = null)
		{
			var result = Top(1, where, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`DeviceCertificateDataColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static DeviceCertificateData OneWhere(QiQuery where, IDatabase database = null)
		{
			var results = Top(1, where, database);
			return OneOrThrow(results);
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a DeviceCertificateDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceCertificateDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static DeviceCertificateData FirstOneWhere(WhereDelegate<DeviceCertificateDataColumns> where, IDatabase database = null)
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
		/// <param name="where">A WhereDelegate that recieves a DeviceCertificateDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceCertificateDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static DeviceCertificateData FirstOneWhere(WhereDelegate<DeviceCertificateDataColumns> where, OrderBy<DeviceCertificateDataColumns> orderBy, IDatabase database = null)
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
		/// <param name="where">A WhereDelegate that recieves a DeviceCertificateDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceCertificateDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static DeviceCertificateData FirstOneWhere(QueryFilter where, OrderBy<DeviceCertificateDataColumns> orderBy = null, IDatabase database = null)
		{
			WhereDelegate<DeviceCertificateDataColumns> whereDelegate = (c) => where;
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
		/// <param name="where">A WhereDelegate that recieves a DeviceCertificateDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceCertificateDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static DeviceCertificateDataCollection Top(int count, WhereDelegate<DeviceCertificateDataColumns> where, IDatabase database = null)
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
		/// <param name="where">A WhereDelegate that recieves a DeviceCertificateDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceCertificateDataColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static DeviceCertificateDataCollection Top(int count, WhereDelegate<DeviceCertificateDataColumns> where, OrderBy<DeviceCertificateDataColumns> orderBy, IDatabase database = null)
		{
			DeviceCertificateDataColumns c = new DeviceCertificateDataColumns();
			IQueryFilter filter = where(c);

			IDatabase db = database ?? Db.For<DeviceCertificateData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<DeviceCertificateData>(count);
			query.Where(filter);

			if(orderBy != null)
			{
				query.OrderBy<DeviceCertificateDataColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<DeviceCertificateDataCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static DeviceCertificateDataCollection Top(int count, QueryFilter where, IDatabase database)
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
		public static DeviceCertificateDataCollection Top(int count, QueryFilter where, OrderBy<DeviceCertificateDataColumns> orderBy = null, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<DeviceCertificateData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<DeviceCertificateData>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy<DeviceCertificateDataColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<DeviceCertificateDataCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static DeviceCertificateDataCollection Top(int count, QueryFilter where, string orderBy = null, SortOrder sortOrder = SortOrder.Ascending, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<DeviceCertificateData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<DeviceCertificateData>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy(orderBy, sortOrder);
			}

			query.Execute(db);
			var results = query.Results.As<DeviceCertificateDataCollection>(0);
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
		public static DeviceCertificateDataCollection Top(int count, QiQuery where, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<DeviceCertificateData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<DeviceCertificateData>(count);
			query.Where(where);
			query.Execute(db);
			var results = query.Results.As<DeviceCertificateDataCollection>(0);
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
			IDatabase db = database ?? Db.For<DeviceCertificateData>();
            IQuerySet query = GetQuerySet(db);
            query.Count<DeviceCertificateData>();
            query.Execute(db);
            return (long)query.Results[0].DataRow[0];
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a DeviceCertificateDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceCertificateDataColumns and other values
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static long Count(WhereDelegate<DeviceCertificateDataColumns> where, IDatabase database = null)
		{
			DeviceCertificateDataColumns c = new DeviceCertificateDataColumns();
			IQueryFilter filter = where(c) ;

			IDatabase db = database ?? Db.For<DeviceCertificateData>();
			IQuerySet query = GetQuerySet(db);
			query.Count<DeviceCertificateData>();
			query.Where(filter);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		public static long Count(QiQuery where, IDatabase database = null)
		{
		    IDatabase db = database ?? Db.For<DeviceCertificateData>();
			IQuerySet query = GetQuerySet(db);
			query.Count<DeviceCertificateData>();
			query.Where(where);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		private static DeviceCertificateData CreateFromFilter(IQueryFilter filter, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<DeviceCertificateData>();
			var dao = new DeviceCertificateData();
			filter.Parameters.Each(p=>
			{
				dao.Property(p.ColumnName, p.Value);
			});
			dao.Save(db);
			return dao;
		}

		private static DeviceCertificateData OneOrThrow(DeviceCertificateDataCollection c)
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
