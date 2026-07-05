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

namespace Bam.Protocol.Data.Common.Dao
{
	// schema = CommonSchema
	// connection Name = CommonSchema
	[Serializable]
	[Bam.Data.Table("AgentData", "CommonSchema")]
	public partial class AgentData: Bam.Data.Dao
	{
		public AgentData():base()
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public AgentData(DataRow data)
			: base(data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public AgentData(IDatabase db)
			: base(db)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public AgentData(IDatabase db, DataRow data)
			: base(db, data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		[Bam.Exclude]
		public static implicit operator AgentData(DataRow data)
		{
			return new AgentData(data);
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
			SetValue("Id", value!);
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
            SetValue("Uuid", value!);
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
            SetValue("Cuid", value!);
        }
    }

    // property:ActorDataId, columnName: ActorDataId	
    [Bam.Data.Column(Name="ActorDataId", DbDataType="BigInt", MaxLength="19", AllowNull=true)]
    public ulong? ActorDataId
    {
        get
        {
            return GetULongValue("ActorDataId");
        }
        set
        {
            SetValue("ActorDataId", value!);
        }
    }

    // property:DeviceDataId, columnName: DeviceDataId	
    [Bam.Data.Column(Name="DeviceDataId", DbDataType="BigInt", MaxLength="19", AllowNull=true)]
    public ulong? DeviceDataId
    {
        get
        {
            return GetULongValue("DeviceDataId");
        }
        set
        {
            SetValue("DeviceDataId", value!);
        }
    }

    // property:ProcessDescriptorDataId, columnName: ProcessDescriptorDataId	
    [Bam.Data.Column(Name="ProcessDescriptorDataId", DbDataType="BigInt", MaxLength="19", AllowNull=true)]
    public ulong? ProcessDescriptorDataId
    {
        get
        {
            return GetULongValue("ProcessDescriptorDataId");
        }
        set
        {
            SetValue("ProcessDescriptorDataId", value!);
        }
    }

    // property:Handle, columnName: Handle	
    [Bam.Data.Column(Name="Handle", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string Handle
    {
        get
        {
            return GetStringValue("Handle");
        }
        set
        {
            SetValue("Handle", value!);
        }
    }

    // property:Name, columnName: Name	
    [Bam.Data.Column(Name="Name", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string Name
    {
        get
        {
            return GetStringValue("Name");
        }
        set
        {
            SetValue("Name", value!);
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
            SetValue("Created", value!);
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
				var colFilter = new AgentDataColumns();
				return (colFilter.KeyColumn == GetDbId());
			}
		}

		/// <summary>
        /// Return every record in the AgentData table.
        /// </summary>
		/// <param name="database">
		/// The database to load from or null
		/// </param>
		public static AgentDataCollection LoadAll(IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<AgentData>();
            ISqlStringBuilder sql = db.GetSqlStringBuilder();
            sql.Select<AgentData>();
            var results = new AgentDataCollection(db, sql.ExecuteGetDataTable(db))
            {
                Database = db
            };
            return results;
        }

        /// <summary>
        /// Process all records in batches of the specified size
        /// </summary>
        [Bam.Exclude]
        public static async Task BatchAll(int batchSize, Action<IEnumerable<AgentData>> batchProcessor, IDatabase database = null!)
		{
			await Task.Run(async ()=>
			{
				AgentDataColumns columns = new AgentDataColumns();
				var orderBy = Bam.Data.Order.By<AgentDataColumns>(c => c.KeyColumn, Bam.Data.SortOrder.Ascending);
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

		public static AgentData GetById(uint? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified AgentData.Id was null");
			return GetById(id!.Value, database);
		}

		public static AgentData GetById(uint id, IDatabase database = null!)
		{
			return GetById((ulong)id, database);
		}

		public static AgentData GetById(int? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified AgentData.Id was null");
			return GetById(id!.Value, database);
		}                                    
                                    
		public static AgentData GetById(int id, IDatabase database = null!)
		{
			return GetById((long)id, database);
		}

		public static AgentData GetById(long? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified AgentData.Id was null");
			return GetById(id!.Value, database);
		}
                                    
		public static AgentData GetById(long id, IDatabase database = null!)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static AgentData GetById(ulong? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified AgentData.Id was null");
			return GetById(id!.Value, database);
		}
                                    
		public static AgentData GetById(ulong id, IDatabase database = null!)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static AgentData GetByUuid(string uuid, IDatabase database = null!)
		{
			return OneWhere(c => Bam.Data.Query.Where("Uuid") == uuid, database);
		}

		public static AgentData GetByCuid(string cuid, IDatabase database = null!)
		{
			return OneWhere(c => Bam.Data.Query.Where("Cuid") == cuid, database);
		}

		[Bam.Exclude]
		public static AgentDataCollection Query(QueryFilter filter, IDatabase database = null!)
		{
			return Where(filter, database);
		}

		[Bam.Exclude]
		public static AgentDataCollection Where(QueryFilter filter, IDatabase database = null!)
		{
			WhereDelegate<AgentDataColumns> whereDelegate = (c) => filter;
			return Where(whereDelegate, database);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A Func delegate that recieves a AgentDataColumns
		/// and returns a QueryFilter which is the result of any comparisons
		/// between AgentDataColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static AgentDataCollection Where(Func<AgentDataColumns, QueryFilter<AgentDataColumns>> where, OrderBy<AgentDataColumns> orderBy = null!, IDatabase database = null!)
		{
			database = database ?? Db.For<AgentData>();
			return new AgentDataCollection(database.GetQuery<AgentDataColumns, AgentData>(where, orderBy), true);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a AgentDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AgentDataColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static AgentDataCollection Where(WhereDelegate<AgentDataColumns> where, IDatabase database = null!)
		{
			database = database ?? Db.For<AgentData>();
			var results = new AgentDataCollection(database, database.GetQuery<AgentDataColumns, AgentData>(where), true);
			return results;
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a AgentDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AgentDataColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static AgentDataCollection Where(WhereDelegate<AgentDataColumns> where, OrderBy<AgentDataColumns> orderBy = null!, IDatabase database = null!)
		{
			database = database ?? Db.For<AgentData>();
			var results = new AgentDataCollection(database, database.GetQuery<AgentDataColumns, AgentData>(where, orderBy), true);
			return results;
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`AgentDataColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static AgentDataCollection Where(QiQuery where, IDatabase database = null!)
		{
			var results = new AgentDataCollection(database, Select<AgentDataColumns>.From<AgentData>().Where(where, database));
			return results;
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static AgentData GetOneWhere(QueryFilter where, IDatabase database = null!)
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
		public static AgentData OneWhere(QueryFilter where, IDatabase database = null!)
		{
			WhereDelegate<AgentDataColumns> whereDelegate = (c) => where;
			var result = Top(1, whereDelegate, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<AgentDataColumns> where, IDatabase database = null!)
		{
			SetOneWhere(where, out AgentData ignore, database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<AgentDataColumns> where, out AgentData result, IDatabase database = null!)
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
		public static AgentData GetOneWhere(WhereDelegate<AgentDataColumns> where, IDatabase database = null!)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				AgentDataColumns c = new AgentDataColumns();
				IQueryFilter filter = where(c);
				result = CreateFromFilter(filter, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.  This method is most commonly used to retrieve a
		/// single AgentData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a AgentDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AgentDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static AgentData OneWhere(WhereDelegate<AgentDataColumns> where, IDatabase database = null!)
		{
			var result = Top(1, where, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`AgentDataColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static AgentData OneWhere(QiQuery where, IDatabase database = null!)
		{
			var results = Top(1, where, database);
			return OneOrThrow(results);
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a AgentDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AgentDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static AgentData FirstOneWhere(WhereDelegate<AgentDataColumns> where, IDatabase database = null!)
		{
			var results = Top(1, where, database);
			if(results.Count > 0)
			{
				return results[0];
			}
			else
			{
				return null!;
			}
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a AgentDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AgentDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static AgentData FirstOneWhere(WhereDelegate<AgentDataColumns> where, OrderBy<AgentDataColumns> orderBy, IDatabase database = null!)
		{
			var results = Top(1, where, orderBy, database);
			if(results.Count > 0)
			{
				return results[0];
			}
			else
			{
				return null!;
			}
		}

		/// <summary>
		/// Shortcut for Top(1, where, orderBy, database)
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a AgentDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AgentDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static AgentData FirstOneWhere(QueryFilter where, OrderBy<AgentDataColumns> orderBy = null!, IDatabase database = null!)
		{
			WhereDelegate<AgentDataColumns> whereDelegate = (c) => where;
			var results = Top(1, whereDelegate, orderBy, database);
			if(results.Count > 0)
			{
				return results[0];
			}
			else
			{
				return null!;
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
		/// <param name="where">A WhereDelegate that recieves a AgentDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AgentDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static AgentDataCollection Top(int count, WhereDelegate<AgentDataColumns> where, IDatabase database = null!)
		{
			return Top(count, where, null!, database);
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
		/// <param name="where">A WhereDelegate that recieves a AgentDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AgentDataColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static AgentDataCollection Top(int count, WhereDelegate<AgentDataColumns> where, OrderBy<AgentDataColumns> orderBy, IDatabase database = null!)
		{
			AgentDataColumns c = new AgentDataColumns();
			IQueryFilter filter = where(c);

			IDatabase db = database ?? Db.For<AgentData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<AgentData>(count);
			query.Where(filter);

			if(orderBy != null)
			{
				query.OrderBy<AgentDataColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<AgentDataCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static AgentDataCollection Top(int count, QueryFilter where, IDatabase database)
		{
			return Top(count, where, null!, database);
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
		public static AgentDataCollection Top(int count, QueryFilter where, OrderBy<AgentDataColumns> orderBy = null!, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<AgentData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<AgentData>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy<AgentDataColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<AgentDataCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static AgentDataCollection Top(int count, QueryFilter where, string orderBy = null!, SortOrder sortOrder = SortOrder.Ascending, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<AgentData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<AgentData>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy(orderBy, sortOrder);
			}

			query.Execute(db);
			var results = query.Results.As<AgentDataCollection>(0);
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
		public static AgentDataCollection Top(int count, QiQuery where, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<AgentData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<AgentData>(count);
			query.Where(where);
			query.Execute(db);
			var results = query.Results.As<AgentDataCollection>(0);
			results.Database = db;
			return results;
		}

		/// <summary>
		/// Return the count of @(Model.ClassName.Pluralize())
		/// </summary>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		public static long Count(IDatabase database = null!)
        {
			IDatabase db = database ?? Db.For<AgentData>();
            IQuerySet query = GetQuerySet(db);
            query.Count<AgentData>();
            query.Execute(db);
            return query.Results.As<CountResult>(0).Value;
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a AgentDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AgentDataColumns and other values
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static long Count(WhereDelegate<AgentDataColumns> where, IDatabase database = null!)
		{
			AgentDataColumns c = new AgentDataColumns();
			IQueryFilter filter = where(c) ;

			IDatabase db = database ?? Db.For<AgentData>();
			IQuerySet query = GetQuerySet(db);
			query.Count<AgentData>();
			query.Where(filter);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		public static long Count(QiQuery where, IDatabase database = null!)
		{
		    IDatabase db = database ?? Db.For<AgentData>();
			IQuerySet query = GetQuerySet(db);
			query.Count<AgentData>();
			query.Where(where);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		private static AgentData CreateFromFilter(IQueryFilter filter, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<AgentData>();
			var dao = new AgentData();
			filter.Parameters.Each(p=>
			{
				dao.Property(p.ColumnName, p.Value!);
			});
			dao.Save(db);
			return dao;
		}

		private static AgentData OneOrThrow(AgentDataCollection c)
		{
			if(c.Count == 1)
			{
				return c[0];
			}
			else if(c.Count > 1)
			{
				throw new MultipleEntriesFoundException();
			}

			return null!;
		}

	}
}
