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
	[Bam.Data.Table("HostAddressData", "CommonSchema")]
	public partial class HostAddressData: Bam.Data.Dao
	{
		public HostAddressData():base()
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public HostAddressData(DataRow data)
			: base(data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public HostAddressData(IDatabase db)
			: base(db)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public HostAddressData(IDatabase db, DataRow data)
			: base(db, data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		[Bam.Exclude]
		public static implicit operator HostAddressData(DataRow data)
		{
			return new HostAddressData(data);
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

    // property:IpAddress, columnName: IpAddress	
    [Bam.Data.Column(Name="IpAddress", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string IpAddress
    {
        get
        {
            return GetStringValue("IpAddress");
        }
        set
        {
            SetValue("IpAddress", value);
        }
    }

    // property:AddressFamily, columnName: AddressFamily	
    [Bam.Data.Column(Name="AddressFamily", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string AddressFamily
    {
        get
        {
            return GetStringValue("AddressFamily");
        }
        set
        {
            SetValue("AddressFamily", value);
        }
    }

    // property:HostName, columnName: HostName	
    [Bam.Data.Column(Name="HostName", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string HostName
    {
        get
        {
            return GetStringValue("HostName");
        }
        set
        {
            SetValue("HostName", value);
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


	// start DeviceDataId -> DeviceDataId
	[Bam.Data.ForeignKey(
        Table="HostAddressData",
		Name="DeviceDataId",
		DbDataType="BigInt",
		MaxLength="",
		AllowNull=true,
		ReferencedKey="Id",
		ReferencedTable="DeviceData",
		Suffix="1")]
	public ulong? DeviceDataId
	{
		get
		{
			return GetULongValue("DeviceDataId", false);
		}
		set
		{
			SetValue("DeviceDataId", value!, false);
		}
	}

    DeviceData _deviceDataOfDeviceDataId = null!;
	public DeviceData DeviceDataOfDeviceDataId
	{
		get
		{
			if(_deviceDataOfDeviceDataId == null)
			{
				_deviceDataOfDeviceDataId = Bam.Protocol.Data.Common.Dao.DeviceData.OneWhere(c => c.KeyColumn == this.DeviceDataId, this.Database);
			}
			return _deviceDataOfDeviceDataId;
		}
	}

	// start MachineDataId -> MachineDataId
	[Bam.Data.ForeignKey(
        Table="HostAddressData",
		Name="MachineDataId",
		DbDataType="BigInt",
		MaxLength="",
		AllowNull=true,
		ReferencedKey="Id",
		ReferencedTable="MachineData",
		Suffix="2")]
	public ulong? MachineDataId
	{
		get
		{
			return GetULongValue("MachineDataId", false);
		}
		set
		{
			SetValue("MachineDataId", value!, false);
		}
	}

    MachineData _machineDataOfMachineDataId = null!;
	public MachineData MachineDataOfMachineDataId
	{
		get
		{
			if(_machineDataOfMachineDataId == null)
			{
				_machineDataOfMachineDataId = Bam.Protocol.Data.Common.Dao.MachineData.OneWhere(c => c.KeyColumn == this.MachineDataId, this.Database);
			}
			return _machineDataOfMachineDataId;
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
				var colFilter = new HostAddressDataColumns();
				return (colFilter.KeyColumn == GetDbId());
			}
		}

		/// <summary>
        /// Return every record in the HostAddressData table.
        /// </summary>
		/// <param name="database">
		/// The database to load from or null
		/// </param>
		public static HostAddressDataCollection LoadAll(IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<HostAddressData>();
            ISqlStringBuilder sql = db.GetSqlStringBuilder();
            sql.Select<HostAddressData>();
            var results = new HostAddressDataCollection(db, sql.ExecuteGetDataTable(db))
            {
                Database = db
            };
            return results;
        }

        /// <summary>
        /// Process all records in batches of the specified size
        /// </summary>
        [Bam.Exclude]
        public static async Task BatchAll(int batchSize, Action<IEnumerable<HostAddressData>> batchProcessor, IDatabase database = null!)
		{
			await Task.Run(async ()=>
			{
				HostAddressDataColumns columns = new HostAddressDataColumns();
				var orderBy = Bam.Data.Order.By<HostAddressDataColumns>(c => c.KeyColumn, Bam.Data.SortOrder.Ascending);
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

		public static HostAddressData GetById(uint? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified HostAddressData.Id was null");
			return GetById(id!.Value, database);
		}

		public static HostAddressData GetById(uint id, IDatabase database = null!)
		{
			return GetById((ulong)id, database);
		}

		public static HostAddressData GetById(int? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified HostAddressData.Id was null");
			return GetById(id!.Value, database);
		}                                    
                                    
		public static HostAddressData GetById(int id, IDatabase database = null!)
		{
			return GetById((long)id, database);
		}

		public static HostAddressData GetById(long? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified HostAddressData.Id was null");
			return GetById(id!.Value, database);
		}
                                    
		public static HostAddressData GetById(long id, IDatabase database = null!)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static HostAddressData GetById(ulong? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified HostAddressData.Id was null");
			return GetById(id!.Value, database);
		}
                                    
		public static HostAddressData GetById(ulong id, IDatabase database = null!)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static HostAddressData GetByUuid(string uuid, IDatabase database = null!)
		{
			return OneWhere(c => Bam.Data.Query.Where("Uuid") == uuid, database);
		}

		public static HostAddressData GetByCuid(string cuid, IDatabase database = null!)
		{
			return OneWhere(c => Bam.Data.Query.Where("Cuid") == cuid, database);
		}

		[Bam.Exclude]
		public static HostAddressDataCollection Query(QueryFilter filter, IDatabase database = null!)
		{
			return Where(filter, database);
		}

		[Bam.Exclude]
		public static HostAddressDataCollection Where(QueryFilter filter, IDatabase database = null!)
		{
			WhereDelegate<HostAddressDataColumns> whereDelegate = (c) => filter;
			return Where(whereDelegate, database);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A Func delegate that recieves a HostAddressDataColumns
		/// and returns a QueryFilter which is the result of any comparisons
		/// between HostAddressDataColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static HostAddressDataCollection Where(Func<HostAddressDataColumns, QueryFilter<HostAddressDataColumns>> where, OrderBy<HostAddressDataColumns> orderBy = null!, IDatabase database = null!)
		{
			database = database ?? Db.For<HostAddressData>();
			return new HostAddressDataCollection(database.GetQuery<HostAddressDataColumns, HostAddressData>(where, orderBy), true);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a HostAddressDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between HostAddressDataColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static HostAddressDataCollection Where(WhereDelegate<HostAddressDataColumns> where, IDatabase database = null!)
		{
			database = database ?? Db.For<HostAddressData>();
			var results = new HostAddressDataCollection(database, database.GetQuery<HostAddressDataColumns, HostAddressData>(where), true);
			return results;
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a HostAddressDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between HostAddressDataColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static HostAddressDataCollection Where(WhereDelegate<HostAddressDataColumns> where, OrderBy<HostAddressDataColumns> orderBy = null!, IDatabase database = null!)
		{
			database = database ?? Db.For<HostAddressData>();
			var results = new HostAddressDataCollection(database, database.GetQuery<HostAddressDataColumns, HostAddressData>(where, orderBy), true);
			return results;
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`HostAddressDataColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static HostAddressDataCollection Where(QiQuery where, IDatabase database = null!)
		{
			var results = new HostAddressDataCollection(database, Select<HostAddressDataColumns>.From<HostAddressData>().Where(where, database));
			return results;
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static HostAddressData GetOneWhere(QueryFilter where, IDatabase database = null!)
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
		public static HostAddressData OneWhere(QueryFilter where, IDatabase database = null!)
		{
			WhereDelegate<HostAddressDataColumns> whereDelegate = (c) => where;
			var result = Top(1, whereDelegate, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<HostAddressDataColumns> where, IDatabase database = null!)
		{
			SetOneWhere(where, out HostAddressData ignore, database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<HostAddressDataColumns> where, out HostAddressData result, IDatabase database = null!)
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
		public static HostAddressData GetOneWhere(WhereDelegate<HostAddressDataColumns> where, IDatabase database = null!)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				HostAddressDataColumns c = new HostAddressDataColumns();
				IQueryFilter filter = where(c);
				result = CreateFromFilter(filter, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.  This method is most commonly used to retrieve a
		/// single HostAddressData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a HostAddressDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between HostAddressDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static HostAddressData OneWhere(WhereDelegate<HostAddressDataColumns> where, IDatabase database = null!)
		{
			var result = Top(1, where, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`HostAddressDataColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static HostAddressData OneWhere(QiQuery where, IDatabase database = null!)
		{
			var results = Top(1, where, database);
			return OneOrThrow(results);
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a HostAddressDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between HostAddressDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static HostAddressData FirstOneWhere(WhereDelegate<HostAddressDataColumns> where, IDatabase database = null!)
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
		/// <param name="where">A WhereDelegate that recieves a HostAddressDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between HostAddressDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static HostAddressData FirstOneWhere(WhereDelegate<HostAddressDataColumns> where, OrderBy<HostAddressDataColumns> orderBy, IDatabase database = null!)
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
		/// <param name="where">A WhereDelegate that recieves a HostAddressDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between HostAddressDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static HostAddressData FirstOneWhere(QueryFilter where, OrderBy<HostAddressDataColumns> orderBy = null!, IDatabase database = null!)
		{
			WhereDelegate<HostAddressDataColumns> whereDelegate = (c) => where;
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
		/// <param name="where">A WhereDelegate that recieves a HostAddressDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between HostAddressDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static HostAddressDataCollection Top(int count, WhereDelegate<HostAddressDataColumns> where, IDatabase database = null!)
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
		/// <param name="where">A WhereDelegate that recieves a HostAddressDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between HostAddressDataColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static HostAddressDataCollection Top(int count, WhereDelegate<HostAddressDataColumns> where, OrderBy<HostAddressDataColumns> orderBy, IDatabase database = null!)
		{
			HostAddressDataColumns c = new HostAddressDataColumns();
			IQueryFilter filter = where(c);

			IDatabase db = database ?? Db.For<HostAddressData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<HostAddressData>(count);
			query.Where(filter);

			if(orderBy != null)
			{
				query.OrderBy<HostAddressDataColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<HostAddressDataCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static HostAddressDataCollection Top(int count, QueryFilter where, IDatabase database)
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
		public static HostAddressDataCollection Top(int count, QueryFilter where, OrderBy<HostAddressDataColumns> orderBy = null!, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<HostAddressData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<HostAddressData>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy<HostAddressDataColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<HostAddressDataCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static HostAddressDataCollection Top(int count, QueryFilter where, string orderBy = null!, SortOrder sortOrder = SortOrder.Ascending, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<HostAddressData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<HostAddressData>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy(orderBy, sortOrder);
			}

			query.Execute(db);
			var results = query.Results.As<HostAddressDataCollection>(0);
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
		public static HostAddressDataCollection Top(int count, QiQuery where, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<HostAddressData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<HostAddressData>(count);
			query.Where(where);
			query.Execute(db);
			var results = query.Results.As<HostAddressDataCollection>(0);
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
			IDatabase db = database ?? Db.For<HostAddressData>();
            IQuerySet query = GetQuerySet(db);
            query.Count<HostAddressData>();
            query.Execute(db);
            return (long)query.Results[0].DataRow[0];
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a HostAddressDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between HostAddressDataColumns and other values
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static long Count(WhereDelegate<HostAddressDataColumns> where, IDatabase database = null!)
		{
			HostAddressDataColumns c = new HostAddressDataColumns();
			IQueryFilter filter = where(c) ;

			IDatabase db = database ?? Db.For<HostAddressData>();
			IQuerySet query = GetQuerySet(db);
			query.Count<HostAddressData>();
			query.Where(filter);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		public static long Count(QiQuery where, IDatabase database = null!)
		{
		    IDatabase db = database ?? Db.For<HostAddressData>();
			IQuerySet query = GetQuerySet(db);
			query.Count<HostAddressData>();
			query.Where(where);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		private static HostAddressData CreateFromFilter(IQueryFilter filter, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<HostAddressData>();
			var dao = new HostAddressData();
			filter.Parameters.Each(p=>
			{
				dao.Property(p.ColumnName, p.Value!);
			});
			dao.Save(db);
			return dao;
		}

		private static HostAddressData OneOrThrow(HostAddressDataCollection c)
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
