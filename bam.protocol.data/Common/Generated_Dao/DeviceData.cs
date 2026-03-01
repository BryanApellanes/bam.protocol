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
	[Bam.Data.Table("DeviceData", "CommonSchema")]
	public partial class DeviceData: Bam.Data.Dao
	{
		public DeviceData():base()
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public DeviceData(DataRow data)
			: base(data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public DeviceData(IDatabase db)
			: base(db)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public DeviceData(IDatabase db, DataRow data)
			: base(db, data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		[Bam.Exclude]
		public static implicit operator DeviceData(DataRow data)
		{
			return new DeviceData(data);
		}

		private void SetChildren()
		{


			if(_database != null)
			{
				this.ChildCollections.Add("HostAddressData_DeviceDataId", new HostAddressDataCollection(Database.GetQuery<HostAddressDataColumns, HostAddressData>((c) => c.DeviceDataId == GetULongValue("Id", false)), this, "DeviceDataId"));
			}
			if(_database != null)
			{
				this.ChildCollections.Add("NicData_DeviceDataId", new NicDataCollection(Database.GetQuery<NicDataColumns, NicData>((c) => c.DeviceDataId == GetULongValue("Id", false)), this, "DeviceDataId"));
			}


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

    // property:ProcessDescriptorId, columnName: ProcessDescriptorId	
    [Bam.Data.Column(Name="ProcessDescriptorId", DbDataType="BigInt", MaxLength="19", AllowNull=true)]
    public ulong? ProcessDescriptorId
    {
        get
        {
            return GetULongValue("ProcessDescriptorId");
        }
        set
        {
            SetValue("ProcessDescriptorId", value!);
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

    // property:DnsName, columnName: DnsName	
    [Bam.Data.Column(Name="DnsName", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string DnsName
    {
        get
        {
            return GetStringValue("DnsName");
        }
        set
        {
            SetValue("DnsName", value!);
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



	[Bam.Exclude]
	public HostAddressDataCollection HostAddressDatasByDeviceDataId
	{
		get
		{
			if (this.IsNew)
			{
				throw new InvalidOperationException($"The current instance of type({this.GetType().Name}) hasn't been saved and will have no child collections, call Save() or Save(Database) first.");
			}

			if(!this.ChildCollections.ContainsKey("HostAddressData_DeviceDataId"))
			{
				SetChildren();
			}

			var c = (HostAddressDataCollection)this.ChildCollections["HostAddressData_DeviceDataId"];
			if(!c.Loaded)
			{
				c.Load(Database);
			}
			return c;
		}
	}
	[Bam.Exclude]
	public NicDataCollection NicDatasByDeviceDataId
	{
		get
		{
			if (this.IsNew)
			{
				throw new InvalidOperationException($"The current instance of type({this.GetType().Name}) hasn't been saved and will have no child collections, call Save() or Save(Database) first.");
			}

			if(!this.ChildCollections.ContainsKey("NicData_DeviceDataId"))
			{
				SetChildren();
			}

			var c = (NicDataCollection)this.ChildCollections["NicData_DeviceDataId"];
			if(!c.Loaded)
			{
				c.Load(Database);
			}
			return c;
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
				var colFilter = new DeviceDataColumns();
				return (colFilter.KeyColumn == GetDbId());
			}
		}

		/// <summary>
        /// Return every record in the DeviceData table.
        /// </summary>
		/// <param name="database">
		/// The database to load from or null
		/// </param>
		public static DeviceDataCollection LoadAll(IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<DeviceData>();
            ISqlStringBuilder sql = db.GetSqlStringBuilder();
            sql.Select<DeviceData>();
            var results = new DeviceDataCollection(db, sql.ExecuteGetDataTable(db))
            {
                Database = db
            };
            return results;
        }

        /// <summary>
        /// Process all records in batches of the specified size
        /// </summary>
        [Bam.Exclude]
        public static async Task BatchAll(int batchSize, Action<IEnumerable<DeviceData>> batchProcessor, IDatabase database = null!)
		{
			await Task.Run(async ()=>
			{
				DeviceDataColumns columns = new DeviceDataColumns();
				var orderBy = Bam.Data.Order.By<DeviceDataColumns>(c => c.KeyColumn, Bam.Data.SortOrder.Ascending);
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

		public static DeviceData GetById(uint? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified DeviceData.Id was null");
			return GetById(id!.Value, database);
		}

		public static DeviceData GetById(uint id, IDatabase database = null!)
		{
			return GetById((ulong)id, database);
		}

		public static DeviceData GetById(int? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified DeviceData.Id was null");
			return GetById(id!.Value, database);
		}                                    
                                    
		public static DeviceData GetById(int id, IDatabase database = null!)
		{
			return GetById((long)id, database);
		}

		public static DeviceData GetById(long? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified DeviceData.Id was null");
			return GetById(id!.Value, database);
		}
                                    
		public static DeviceData GetById(long id, IDatabase database = null!)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static DeviceData GetById(ulong? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified DeviceData.Id was null");
			return GetById(id!.Value, database);
		}
                                    
		public static DeviceData GetById(ulong id, IDatabase database = null!)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static DeviceData GetByUuid(string uuid, IDatabase database = null!)
		{
			return OneWhere(c => Bam.Data.Query.Where("Uuid") == uuid, database);
		}

		public static DeviceData GetByCuid(string cuid, IDatabase database = null!)
		{
			return OneWhere(c => Bam.Data.Query.Where("Cuid") == cuid, database);
		}

		[Bam.Exclude]
		public static DeviceDataCollection Query(QueryFilter filter, IDatabase database = null!)
		{
			return Where(filter, database);
		}

		[Bam.Exclude]
		public static DeviceDataCollection Where(QueryFilter filter, IDatabase database = null!)
		{
			WhereDelegate<DeviceDataColumns> whereDelegate = (c) => filter;
			return Where(whereDelegate, database);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A Func delegate that recieves a DeviceDataColumns
		/// and returns a QueryFilter which is the result of any comparisons
		/// between DeviceDataColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static DeviceDataCollection Where(Func<DeviceDataColumns, QueryFilter<DeviceDataColumns>> where, OrderBy<DeviceDataColumns> orderBy = null!, IDatabase database = null!)
		{
			database = database ?? Db.For<DeviceData>();
			return new DeviceDataCollection(database.GetQuery<DeviceDataColumns, DeviceData>(where, orderBy), true);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a DeviceDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceDataColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static DeviceDataCollection Where(WhereDelegate<DeviceDataColumns> where, IDatabase database = null!)
		{
			database = database ?? Db.For<DeviceData>();
			var results = new DeviceDataCollection(database, database.GetQuery<DeviceDataColumns, DeviceData>(where), true);
			return results;
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a DeviceDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceDataColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static DeviceDataCollection Where(WhereDelegate<DeviceDataColumns> where, OrderBy<DeviceDataColumns> orderBy = null!, IDatabase database = null!)
		{
			database = database ?? Db.For<DeviceData>();
			var results = new DeviceDataCollection(database, database.GetQuery<DeviceDataColumns, DeviceData>(where, orderBy), true);
			return results;
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`DeviceDataColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static DeviceDataCollection Where(QiQuery where, IDatabase database = null!)
		{
			var results = new DeviceDataCollection(database, Select<DeviceDataColumns>.From<DeviceData>().Where(where, database));
			return results;
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static DeviceData GetOneWhere(QueryFilter where, IDatabase database = null!)
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
		public static DeviceData OneWhere(QueryFilter where, IDatabase database = null!)
		{
			WhereDelegate<DeviceDataColumns> whereDelegate = (c) => where;
			var result = Top(1, whereDelegate, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<DeviceDataColumns> where, IDatabase database = null!)
		{
			SetOneWhere(where, out DeviceData ignore, database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<DeviceDataColumns> where, out DeviceData result, IDatabase database = null!)
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
		public static DeviceData GetOneWhere(WhereDelegate<DeviceDataColumns> where, IDatabase database = null!)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				DeviceDataColumns c = new DeviceDataColumns();
				IQueryFilter filter = where(c);
				result = CreateFromFilter(filter, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.  This method is most commonly used to retrieve a
		/// single DeviceData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a DeviceDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static DeviceData OneWhere(WhereDelegate<DeviceDataColumns> where, IDatabase database = null!)
		{
			var result = Top(1, where, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`DeviceDataColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static DeviceData OneWhere(QiQuery where, IDatabase database = null!)
		{
			var results = Top(1, where, database);
			return OneOrThrow(results);
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a DeviceDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static DeviceData FirstOneWhere(WhereDelegate<DeviceDataColumns> where, IDatabase database = null!)
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
		/// <param name="where">A WhereDelegate that recieves a DeviceDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static DeviceData FirstOneWhere(WhereDelegate<DeviceDataColumns> where, OrderBy<DeviceDataColumns> orderBy, IDatabase database = null!)
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
		/// <param name="where">A WhereDelegate that recieves a DeviceDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static DeviceData FirstOneWhere(QueryFilter where, OrderBy<DeviceDataColumns> orderBy = null!, IDatabase database = null!)
		{
			WhereDelegate<DeviceDataColumns> whereDelegate = (c) => where;
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
		/// <param name="where">A WhereDelegate that recieves a DeviceDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static DeviceDataCollection Top(int count, WhereDelegate<DeviceDataColumns> where, IDatabase database = null!)
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
		/// <param name="where">A WhereDelegate that recieves a DeviceDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceDataColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static DeviceDataCollection Top(int count, WhereDelegate<DeviceDataColumns> where, OrderBy<DeviceDataColumns> orderBy, IDatabase database = null!)
		{
			DeviceDataColumns c = new DeviceDataColumns();
			IQueryFilter filter = where(c);

			IDatabase db = database ?? Db.For<DeviceData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<DeviceData>(count);
			query.Where(filter);

			if(orderBy != null)
			{
				query.OrderBy<DeviceDataColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<DeviceDataCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static DeviceDataCollection Top(int count, QueryFilter where, IDatabase database)
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
		public static DeviceDataCollection Top(int count, QueryFilter where, OrderBy<DeviceDataColumns> orderBy = null!, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<DeviceData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<DeviceData>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy<DeviceDataColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<DeviceDataCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static DeviceDataCollection Top(int count, QueryFilter where, string orderBy = null!, SortOrder sortOrder = SortOrder.Ascending, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<DeviceData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<DeviceData>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy(orderBy, sortOrder);
			}

			query.Execute(db);
			var results = query.Results.As<DeviceDataCollection>(0);
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
		public static DeviceDataCollection Top(int count, QiQuery where, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<DeviceData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<DeviceData>(count);
			query.Where(where);
			query.Execute(db);
			var results = query.Results.As<DeviceDataCollection>(0);
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
			IDatabase db = database ?? Db.For<DeviceData>();
            IQuerySet query = GetQuerySet(db);
            query.Count<DeviceData>();
            query.Execute(db);
            return query.Results.As<CountResult>(0).Value;
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a DeviceDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceDataColumns and other values
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static long Count(WhereDelegate<DeviceDataColumns> where, IDatabase database = null!)
		{
			DeviceDataColumns c = new DeviceDataColumns();
			IQueryFilter filter = where(c) ;

			IDatabase db = database ?? Db.For<DeviceData>();
			IQuerySet query = GetQuerySet(db);
			query.Count<DeviceData>();
			query.Where(filter);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		public static long Count(QiQuery where, IDatabase database = null!)
		{
		    IDatabase db = database ?? Db.For<DeviceData>();
			IQuerySet query = GetQuerySet(db);
			query.Count<DeviceData>();
			query.Where(where);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		private static DeviceData CreateFromFilter(IQueryFilter filter, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<DeviceData>();
			var dao = new DeviceData();
			filter.Parameters.Each(p=>
			{
				dao.Property(p.ColumnName, p.Value!);
			});
			dao.Save(db);
			return dao;
		}

		private static DeviceData OneOrThrow(DeviceDataCollection c)
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
