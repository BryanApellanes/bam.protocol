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
	[Bam.Data.Table("ProcessDescriptorData", "CommonSchema")]
	public partial class ProcessDescriptorData: Bam.Data.Dao
	{
		public ProcessDescriptorData():base()
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public ProcessDescriptorData(DataRow data)
			: base(data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public ProcessDescriptorData(IDatabase db)
			: base(db)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public ProcessDescriptorData(IDatabase db, DataRow data)
			: base(db, data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		[Bam.Exclude]
		public static implicit operator ProcessDescriptorData(DataRow data)
		{
			return new ProcessDescriptorData(data);
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

    // property:InstanceIdentifier, columnName: InstanceIdentifier	
    [Bam.Data.Column(Name="InstanceIdentifier", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string InstanceIdentifier
    {
        get
        {
            return GetStringValue("InstanceIdentifier");
        }
        set
        {
            SetValue("InstanceIdentifier", value);
        }
    }

    // property:MachineId, columnName: MachineId	
    [Bam.Data.Column(Name="MachineId", DbDataType="BigInt", MaxLength="19", AllowNull=true)]
    public ulong? MachineId
    {
        get
        {
            return GetULongValue("MachineId");
        }
        set
        {
            SetValue("MachineId", value);
        }
    }

    // property:HashAlgorithm, columnName: HashAlgorithm	
    [Bam.Data.Column(Name="HashAlgorithm", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string HashAlgorithm
    {
        get
        {
            return GetStringValue("HashAlgorithm");
        }
        set
        {
            SetValue("HashAlgorithm", value);
        }
    }

    // property:Hash, columnName: Hash	
    [Bam.Data.Column(Name="Hash", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string Hash
    {
        get
        {
            return GetStringValue("Hash");
        }
        set
        {
            SetValue("Hash", value);
        }
    }

    // property:MachineName, columnName: MachineName	
    [Bam.Data.Column(Name="MachineName", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string MachineName
    {
        get
        {
            return GetStringValue("MachineName");
        }
        set
        {
            SetValue("MachineName", value);
        }
    }

    // property:ProcessId, columnName: ProcessId	
    [Bam.Data.Column(Name="ProcessId", DbDataType="Int", MaxLength="10", AllowNull=true)]
    public int? ProcessId
    {
        get
        {
            return GetIntValue("ProcessId");
        }
        set
        {
            SetValue("ProcessId", value);
        }
    }

    // property:StartTime, columnName: StartTime	
    [Bam.Data.Column(Name="StartTime", DbDataType="DateTime", MaxLength="8", AllowNull=true)]
    public DateTime? StartTime
    {
        get
        {
            return GetDateTimeValue("StartTime");
        }
        set
        {
            SetValue("StartTime", value);
        }
    }

    // property:HasExited, columnName: HasExited	
    [Bam.Data.Column(Name="HasExited", DbDataType="Bit", MaxLength="1", AllowNull=true)]
    public bool? HasExited
    {
        get
        {
            return GetBooleanValue("HasExited");
        }
        set
        {
            SetValue("HasExited", value);
        }
    }

    // property:ExitTime, columnName: ExitTime	
    [Bam.Data.Column(Name="ExitTime", DbDataType="DateTime", MaxLength="8", AllowNull=true)]
    public DateTime? ExitTime
    {
        get
        {
            return GetDateTimeValue("ExitTime");
        }
        set
        {
            SetValue("ExitTime", value);
        }
    }

    // property:ExitCode, columnName: ExitCode	
    [Bam.Data.Column(Name="ExitCode", DbDataType="Int", MaxLength="10", AllowNull=true)]
    public int? ExitCode
    {
        get
        {
            return GetIntValue("ExitCode");
        }
        set
        {
            SetValue("ExitCode", value);
        }
    }

    // property:FilePath, columnName: FilePath	
    [Bam.Data.Column(Name="FilePath", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string FilePath
    {
        get
        {
            return GetStringValue("FilePath");
        }
        set
        {
            SetValue("FilePath", value);
        }
    }

    // property:CommandLine, columnName: CommandLine	
    [Bam.Data.Column(Name="CommandLine", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string CommandLine
    {
        get
        {
            return GetStringValue("CommandLine");
        }
        set
        {
            SetValue("CommandLine", value);
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
				var colFilter = new ProcessDescriptorDataColumns();
				return (colFilter.KeyColumn == GetDbId());
			}
		}

		/// <summary>
        /// Return every record in the ProcessDescriptorData table.
        /// </summary>
		/// <param name="database">
		/// The database to load from or null
		/// </param>
		public static ProcessDescriptorDataCollection LoadAll(IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<ProcessDescriptorData>();
            ISqlStringBuilder sql = db.GetSqlStringBuilder();
            sql.Select<ProcessDescriptorData>();
            var results = new ProcessDescriptorDataCollection(db, sql.ExecuteGetDataTable(db))
            {
                Database = db
            };
            return results;
        }

        /// <summary>
        /// Process all records in batches of the specified size
        /// </summary>
        [Bam.Exclude]
        public static async Task BatchAll(int batchSize, Action<IEnumerable<ProcessDescriptorData>> batchProcessor, IDatabase database = null)
		{
			await Task.Run(async ()=>
			{
				ProcessDescriptorDataColumns columns = new ProcessDescriptorDataColumns();
				var orderBy = Bam.Data.Order.By<ProcessDescriptorDataColumns>(c => c.KeyColumn, Bam.Data.SortOrder.Ascending);
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

		public static ProcessDescriptorData GetById(uint? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified ProcessDescriptorData.Id was null");
			return GetById(id.Value, database);
		}

		public static ProcessDescriptorData GetById(uint id, IDatabase database = null)
		{
			return GetById((ulong)id, database);
		}

		public static ProcessDescriptorData GetById(int? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified ProcessDescriptorData.Id was null");
			return GetById(id.Value, database);
		}                                    
                                    
		public static ProcessDescriptorData GetById(int id, IDatabase database = null)
		{
			return GetById((long)id, database);
		}

		public static ProcessDescriptorData GetById(long? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified ProcessDescriptorData.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static ProcessDescriptorData GetById(long id, IDatabase database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static ProcessDescriptorData GetById(ulong? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified ProcessDescriptorData.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static ProcessDescriptorData GetById(ulong id, IDatabase database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static ProcessDescriptorData GetByUuid(string uuid, IDatabase database = null)
		{
			return OneWhere(c => Bam.Data.Query.Where("Uuid") == uuid, database);
		}

		public static ProcessDescriptorData GetByCuid(string cuid, IDatabase database = null)
		{
			return OneWhere(c => Bam.Data.Query.Where("Cuid") == cuid, database);
		}

		[Bam.Exclude]
		public static ProcessDescriptorDataCollection Query(QueryFilter filter, IDatabase database = null)
		{
			return Where(filter, database);
		}

		[Bam.Exclude]
		public static ProcessDescriptorDataCollection Where(QueryFilter filter, IDatabase database = null)
		{
			WhereDelegate<ProcessDescriptorDataColumns> whereDelegate = (c) => filter;
			return Where(whereDelegate, database);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A Func delegate that recieves a ProcessDescriptorDataColumns
		/// and returns a QueryFilter which is the result of any comparisons
		/// between ProcessDescriptorDataColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static ProcessDescriptorDataCollection Where(Func<ProcessDescriptorDataColumns, QueryFilter<ProcessDescriptorDataColumns>> where, OrderBy<ProcessDescriptorDataColumns> orderBy = null, IDatabase database = null)
		{
			database = database ?? Db.For<ProcessDescriptorData>();
			return new ProcessDescriptorDataCollection(database.GetQuery<ProcessDescriptorDataColumns, ProcessDescriptorData>(where, orderBy), true);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a ProcessDescriptorDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProcessDescriptorDataColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static ProcessDescriptorDataCollection Where(WhereDelegate<ProcessDescriptorDataColumns> where, IDatabase database = null)
		{
			database = database ?? Db.For<ProcessDescriptorData>();
			var results = new ProcessDescriptorDataCollection(database, database.GetQuery<ProcessDescriptorDataColumns, ProcessDescriptorData>(where), true);
			return results;
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a ProcessDescriptorDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProcessDescriptorDataColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static ProcessDescriptorDataCollection Where(WhereDelegate<ProcessDescriptorDataColumns> where, OrderBy<ProcessDescriptorDataColumns> orderBy = null, IDatabase database = null)
		{
			database = database ?? Db.For<ProcessDescriptorData>();
			var results = new ProcessDescriptorDataCollection(database, database.GetQuery<ProcessDescriptorDataColumns, ProcessDescriptorData>(where, orderBy), true);
			return results;
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`ProcessDescriptorDataColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static ProcessDescriptorDataCollection Where(QiQuery where, IDatabase database = null)
		{
			var results = new ProcessDescriptorDataCollection(database, Select<ProcessDescriptorDataColumns>.From<ProcessDescriptorData>().Where(where, database));
			return results;
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static ProcessDescriptorData GetOneWhere(QueryFilter where, IDatabase database = null)
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
		public static ProcessDescriptorData OneWhere(QueryFilter where, IDatabase database = null)
		{
			WhereDelegate<ProcessDescriptorDataColumns> whereDelegate = (c) => where;
			var result = Top(1, whereDelegate, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<ProcessDescriptorDataColumns> where, IDatabase database = null)
		{
			SetOneWhere(where, out ProcessDescriptorData ignore, database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<ProcessDescriptorDataColumns> where, out ProcessDescriptorData result, IDatabase database = null)
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
		public static ProcessDescriptorData GetOneWhere(WhereDelegate<ProcessDescriptorDataColumns> where, IDatabase database = null)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				ProcessDescriptorDataColumns c = new ProcessDescriptorDataColumns();
				IQueryFilter filter = where(c);
				result = CreateFromFilter(filter, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.  This method is most commonly used to retrieve a
		/// single ProcessDescriptorData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a ProcessDescriptorDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProcessDescriptorDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static ProcessDescriptorData OneWhere(WhereDelegate<ProcessDescriptorDataColumns> where, IDatabase database = null)
		{
			var result = Top(1, where, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`ProcessDescriptorDataColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static ProcessDescriptorData OneWhere(QiQuery where, IDatabase database = null)
		{
			var results = Top(1, where, database);
			return OneOrThrow(results);
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a ProcessDescriptorDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProcessDescriptorDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static ProcessDescriptorData FirstOneWhere(WhereDelegate<ProcessDescriptorDataColumns> where, IDatabase database = null)
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
		/// <param name="where">A WhereDelegate that recieves a ProcessDescriptorDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProcessDescriptorDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static ProcessDescriptorData FirstOneWhere(WhereDelegate<ProcessDescriptorDataColumns> where, OrderBy<ProcessDescriptorDataColumns> orderBy, IDatabase database = null)
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
		/// <param name="where">A WhereDelegate that recieves a ProcessDescriptorDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProcessDescriptorDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static ProcessDescriptorData FirstOneWhere(QueryFilter where, OrderBy<ProcessDescriptorDataColumns> orderBy = null, IDatabase database = null)
		{
			WhereDelegate<ProcessDescriptorDataColumns> whereDelegate = (c) => where;
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
		/// <param name="where">A WhereDelegate that recieves a ProcessDescriptorDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProcessDescriptorDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static ProcessDescriptorDataCollection Top(int count, WhereDelegate<ProcessDescriptorDataColumns> where, IDatabase database = null)
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
		/// <param name="where">A WhereDelegate that recieves a ProcessDescriptorDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProcessDescriptorDataColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static ProcessDescriptorDataCollection Top(int count, WhereDelegate<ProcessDescriptorDataColumns> where, OrderBy<ProcessDescriptorDataColumns> orderBy, IDatabase database = null)
		{
			ProcessDescriptorDataColumns c = new ProcessDescriptorDataColumns();
			IQueryFilter filter = where(c);

			IDatabase db = database ?? Db.For<ProcessDescriptorData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<ProcessDescriptorData>(count);
			query.Where(filter);

			if(orderBy != null)
			{
				query.OrderBy<ProcessDescriptorDataColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<ProcessDescriptorDataCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static ProcessDescriptorDataCollection Top(int count, QueryFilter where, IDatabase database)
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
		public static ProcessDescriptorDataCollection Top(int count, QueryFilter where, OrderBy<ProcessDescriptorDataColumns> orderBy = null, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<ProcessDescriptorData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<ProcessDescriptorData>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy<ProcessDescriptorDataColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<ProcessDescriptorDataCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static ProcessDescriptorDataCollection Top(int count, QueryFilter where, string orderBy = null, SortOrder sortOrder = SortOrder.Ascending, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<ProcessDescriptorData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<ProcessDescriptorData>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy(orderBy, sortOrder);
			}

			query.Execute(db);
			var results = query.Results.As<ProcessDescriptorDataCollection>(0);
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
		public static ProcessDescriptorDataCollection Top(int count, QiQuery where, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<ProcessDescriptorData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<ProcessDescriptorData>(count);
			query.Where(where);
			query.Execute(db);
			var results = query.Results.As<ProcessDescriptorDataCollection>(0);
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
			IDatabase db = database ?? Db.For<ProcessDescriptorData>();
            IQuerySet query = GetQuerySet(db);
            query.Count<ProcessDescriptorData>();
            query.Execute(db);
            return (long)query.Results[0].DataRow[0];
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a ProcessDescriptorDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProcessDescriptorDataColumns and other values
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static long Count(WhereDelegate<ProcessDescriptorDataColumns> where, IDatabase database = null)
		{
			ProcessDescriptorDataColumns c = new ProcessDescriptorDataColumns();
			IQueryFilter filter = where(c) ;

			IDatabase db = database ?? Db.For<ProcessDescriptorData>();
			IQuerySet query = GetQuerySet(db);
			query.Count<ProcessDescriptorData>();
			query.Where(filter);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		public static long Count(QiQuery where, IDatabase database = null)
		{
		    IDatabase db = database ?? Db.For<ProcessDescriptorData>();
			IQuerySet query = GetQuerySet(db);
			query.Count<ProcessDescriptorData>();
			query.Where(where);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		private static ProcessDescriptorData CreateFromFilter(IQueryFilter filter, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<ProcessDescriptorData>();
			var dao = new ProcessDescriptorData();
			filter.Parameters.Each(p=>
			{
				dao.Property(p.ColumnName, p.Value);
			});
			dao.Save(db);
			return dao;
		}

		private static ProcessDescriptorData OneOrThrow(ProcessDescriptorDataCollection c)
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
