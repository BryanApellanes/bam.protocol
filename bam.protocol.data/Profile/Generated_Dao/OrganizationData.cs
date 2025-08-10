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
	[Bam.Data.Table("OrganizationData", "ProfileSchema")]
	public partial class OrganizationData: Bam.Data.Dao
	{
		public OrganizationData():base()
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public OrganizationData(DataRow data)
			: base(data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public OrganizationData(IDatabase db)
			: base(db)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public OrganizationData(IDatabase db, DataRow data)
			: base(db, data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		[Bam.Exclude]
		public static implicit operator OrganizationData(DataRow data)
		{
			return new OrganizationData(data);
		}

		private void SetChildren()
		{


			if(_database != null)
			{
				this.ChildCollections.Add("PersonDataOrganizationData_OrganizationDataId", new PersonDataOrganizationDataCollection(Database.GetQuery<PersonDataOrganizationDataColumns, PersonDataOrganizationData>((c) => c.OrganizationDataId == GetULongValue("Id", false)), this, "OrganizationDataId"));
			}

            this.ChildCollections.Add("OrganizationData_PersonDataOrganizationData_PersonData",  new XrefDaoCollection<PersonDataOrganizationData, PersonData>(this, false));

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
            SetValue("Handle", value);
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
            SetValue("Name", value);
        }
    }

    // property:Key, columnName: Key	
    [Bam.Data.Column(Name="Key", DbDataType="BigInt", MaxLength="19", AllowNull=true)]
    public ulong? Key
    {
        get
        {
            return GetULongValue("Key");
        }
        set
        {
            SetValue("Key", value);
        }
    }

    // property:CompositeKeyId, columnName: CompositeKeyId	
    [Bam.Data.Column(Name="CompositeKeyId", DbDataType="BigInt", MaxLength="19", AllowNull=true)]
    public ulong? CompositeKeyId
    {
        get
        {
            return GetULongValue("CompositeKeyId");
        }
        set
        {
            SetValue("CompositeKeyId", value);
        }
    }

    // property:CompositeKey, columnName: CompositeKey	
    [Bam.Data.Column(Name="CompositeKey", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string CompositeKey
    {
        get
        {
            return GetStringValue("CompositeKey");
        }
        set
        {
            SetValue("CompositeKey", value);
        }
    }

    // property:CreatedBy, columnName: CreatedBy	
    [Bam.Data.Column(Name="CreatedBy", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string CreatedBy
    {
        get
        {
            return GetStringValue("CreatedBy");
        }
        set
        {
            SetValue("CreatedBy", value);
        }
    }

    // property:ModifiedBy, columnName: ModifiedBy	
    [Bam.Data.Column(Name="ModifiedBy", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string ModifiedBy
    {
        get
        {
            return GetStringValue("ModifiedBy");
        }
        set
        {
            SetValue("ModifiedBy", value);
        }
    }

    // property:Modified, columnName: Modified	
    [Bam.Data.Column(Name="Modified", DbDataType="DateTime", MaxLength="8", AllowNull=true)]
    public DateTime? Modified
    {
        get
        {
            return GetDateTimeValue("Modified");
        }
        set
        {
            SetValue("Modified", value);
        }
    }

    // property:Deleted, columnName: Deleted	
    [Bam.Data.Column(Name="Deleted", DbDataType="DateTime", MaxLength="8", AllowNull=true)]
    public DateTime? Deleted
    {
        get
        {
            return GetDateTimeValue("Deleted");
        }
        set
        {
            SetValue("Deleted", value);
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



	[Bam.Exclude]
	public PersonDataOrganizationDataCollection PersonDataOrganizationDatasByOrganizationDataId
	{
		get
		{
			if (this.IsNew)
			{
				throw new InvalidOperationException($"The current instance of type({this.GetType().Name}) hasn't been saved and will have no child collections, call Save() or Save(Database) first.");
			}

			if(!this.ChildCollections.ContainsKey("PersonDataOrganizationData_OrganizationDataId"))
			{
				SetChildren();
			}

			var c = (PersonDataOrganizationDataCollection)this.ChildCollections["PersonDataOrganizationData_OrganizationDataId"];
			if(!c.Loaded)
			{
				c.Load(Database);
			}
			return c;
		}
	}


		// Xref       
        public XrefDaoCollection<PersonDataOrganizationData, PersonData> PersonDatas
        {
            get
            {			
				if (this.IsNew)
				{
					throw new InvalidOperationException($"The current instance of type({this.GetType().Name}) hasn't been saved and will have no child collections, call Save() or Save(Database) first.");
				}

				if(!this.ChildCollections.ContainsKey("OrganizationData_PersonDataOrganizationData_PersonData"))
				{
					SetChildren();
				}

				var xref = (XrefDaoCollection<PersonDataOrganizationData, PersonData>)this.ChildCollections["OrganizationData_PersonDataOrganizationData_PersonData"];
				if(!xref.Loaded)
				{
					xref.Load(Database);
				}

				return xref;
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
				var colFilter = new OrganizationDataColumns();
				return (colFilter.KeyColumn == GetDbId());
			}
		}

		/// <summary>
        /// Return every record in the OrganizationData table.
        /// </summary>
		/// <param name="database">
		/// The database to load from or null
		/// </param>
		public static OrganizationDataCollection LoadAll(IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<OrganizationData>();
            ISqlStringBuilder sql = db.GetSqlStringBuilder();
            sql.Select<OrganizationData>();
            var results = new OrganizationDataCollection(db, sql.ExecuteGetDataTable(db))
            {
                Database = db
            };
            return results;
        }

        /// <summary>
        /// Process all records in batches of the specified size
        /// </summary>
        [Bam.Exclude]
        public static async Task BatchAll(int batchSize, Action<IEnumerable<OrganizationData>> batchProcessor, IDatabase database = null)
		{
			await Task.Run(async ()=>
			{
				OrganizationDataColumns columns = new OrganizationDataColumns();
				var orderBy = Bam.Data.Order.By<OrganizationDataColumns>(c => c.KeyColumn, Bam.Data.SortOrder.Ascending);
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

		public static OrganizationData GetById(uint? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified OrganizationData.Id was null");
			return GetById(id.Value, database);
		}

		public static OrganizationData GetById(uint id, IDatabase database = null)
		{
			return GetById((ulong)id, database);
		}

		public static OrganizationData GetById(int? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified OrganizationData.Id was null");
			return GetById(id.Value, database);
		}                                    
                                    
		public static OrganizationData GetById(int id, IDatabase database = null)
		{
			return GetById((long)id, database);
		}

		public static OrganizationData GetById(long? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified OrganizationData.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static OrganizationData GetById(long id, IDatabase database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static OrganizationData GetById(ulong? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified OrganizationData.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static OrganizationData GetById(ulong id, IDatabase database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static OrganizationData GetByUuid(string uuid, IDatabase database = null)
		{
			return OneWhere(c => Bam.Data.Query.Where("Uuid") == uuid, database);
		}

		public static OrganizationData GetByCuid(string cuid, IDatabase database = null)
		{
			return OneWhere(c => Bam.Data.Query.Where("Cuid") == cuid, database);
		}

		[Bam.Exclude]
		public static OrganizationDataCollection Query(QueryFilter filter, IDatabase database = null)
		{
			return Where(filter, database);
		}

		[Bam.Exclude]
		public static OrganizationDataCollection Where(QueryFilter filter, IDatabase database = null)
		{
			WhereDelegate<OrganizationDataColumns> whereDelegate = (c) => filter;
			return Where(whereDelegate, database);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A Func delegate that recieves a OrganizationDataColumns
		/// and returns a QueryFilter which is the result of any comparisons
		/// between OrganizationDataColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static OrganizationDataCollection Where(Func<OrganizationDataColumns, QueryFilter<OrganizationDataColumns>> where, OrderBy<OrganizationDataColumns> orderBy = null, IDatabase database = null)
		{
			database = database ?? Db.For<OrganizationData>();
			return new OrganizationDataCollection(database.GetQuery<OrganizationDataColumns, OrganizationData>(where, orderBy), true);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a OrganizationDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationDataColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static OrganizationDataCollection Where(WhereDelegate<OrganizationDataColumns> where, IDatabase database = null)
		{
			database = database ?? Db.For<OrganizationData>();
			var results = new OrganizationDataCollection(database, database.GetQuery<OrganizationDataColumns, OrganizationData>(where), true);
			return results;
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a OrganizationDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationDataColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static OrganizationDataCollection Where(WhereDelegate<OrganizationDataColumns> where, OrderBy<OrganizationDataColumns> orderBy = null, IDatabase database = null)
		{
			database = database ?? Db.For<OrganizationData>();
			var results = new OrganizationDataCollection(database, database.GetQuery<OrganizationDataColumns, OrganizationData>(where, orderBy), true);
			return results;
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`OrganizationDataColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static OrganizationDataCollection Where(QiQuery where, IDatabase database = null)
		{
			var results = new OrganizationDataCollection(database, Select<OrganizationDataColumns>.From<OrganizationData>().Where(where, database));
			return results;
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static OrganizationData GetOneWhere(QueryFilter where, IDatabase database = null)
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
		public static OrganizationData OneWhere(QueryFilter where, IDatabase database = null)
		{
			WhereDelegate<OrganizationDataColumns> whereDelegate = (c) => where;
			var result = Top(1, whereDelegate, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<OrganizationDataColumns> where, IDatabase database = null)
		{
			SetOneWhere(where, out OrganizationData ignore, database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<OrganizationDataColumns> where, out OrganizationData result, IDatabase database = null)
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
		public static OrganizationData GetOneWhere(WhereDelegate<OrganizationDataColumns> where, IDatabase database = null)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				OrganizationDataColumns c = new OrganizationDataColumns();
				IQueryFilter filter = where(c);
				result = CreateFromFilter(filter, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.  This method is most commonly used to retrieve a
		/// single OrganizationData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a OrganizationDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static OrganizationData OneWhere(WhereDelegate<OrganizationDataColumns> where, IDatabase database = null)
		{
			var result = Top(1, where, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`OrganizationDataColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static OrganizationData OneWhere(QiQuery where, IDatabase database = null)
		{
			var results = Top(1, where, database);
			return OneOrThrow(results);
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a OrganizationDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static OrganizationData FirstOneWhere(WhereDelegate<OrganizationDataColumns> where, IDatabase database = null)
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
		/// <param name="where">A WhereDelegate that recieves a OrganizationDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static OrganizationData FirstOneWhere(WhereDelegate<OrganizationDataColumns> where, OrderBy<OrganizationDataColumns> orderBy, IDatabase database = null)
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
		/// <param name="where">A WhereDelegate that recieves a OrganizationDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static OrganizationData FirstOneWhere(QueryFilter where, OrderBy<OrganizationDataColumns> orderBy = null, IDatabase database = null)
		{
			WhereDelegate<OrganizationDataColumns> whereDelegate = (c) => where;
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
		/// <param name="where">A WhereDelegate that recieves a OrganizationDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static OrganizationDataCollection Top(int count, WhereDelegate<OrganizationDataColumns> where, IDatabase database = null)
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
		/// <param name="where">A WhereDelegate that recieves a OrganizationDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationDataColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static OrganizationDataCollection Top(int count, WhereDelegate<OrganizationDataColumns> where, OrderBy<OrganizationDataColumns> orderBy, IDatabase database = null)
		{
			OrganizationDataColumns c = new OrganizationDataColumns();
			IQueryFilter filter = where(c);

			IDatabase db = database ?? Db.For<OrganizationData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<OrganizationData>(count);
			query.Where(filter);

			if(orderBy != null)
			{
				query.OrderBy<OrganizationDataColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<OrganizationDataCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static OrganizationDataCollection Top(int count, QueryFilter where, IDatabase database)
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
		public static OrganizationDataCollection Top(int count, QueryFilter where, OrderBy<OrganizationDataColumns> orderBy = null, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<OrganizationData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<OrganizationData>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy<OrganizationDataColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<OrganizationDataCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static OrganizationDataCollection Top(int count, QueryFilter where, string orderBy = null, SortOrder sortOrder = SortOrder.Ascending, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<OrganizationData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<OrganizationData>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy(orderBy, sortOrder);
			}

			query.Execute(db);
			var results = query.Results.As<OrganizationDataCollection>(0);
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
		public static OrganizationDataCollection Top(int count, QiQuery where, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<OrganizationData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<OrganizationData>(count);
			query.Where(where);
			query.Execute(db);
			var results = query.Results.As<OrganizationDataCollection>(0);
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
			IDatabase db = database ?? Db.For<OrganizationData>();
            IQuerySet query = GetQuerySet(db);
            query.Count<OrganizationData>();
            query.Execute(db);
            return (long)query.Results[0].DataRow[0];
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a OrganizationDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationDataColumns and other values
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static long Count(WhereDelegate<OrganizationDataColumns> where, IDatabase database = null)
		{
			OrganizationDataColumns c = new OrganizationDataColumns();
			IQueryFilter filter = where(c) ;

			IDatabase db = database ?? Db.For<OrganizationData>();
			IQuerySet query = GetQuerySet(db);
			query.Count<OrganizationData>();
			query.Where(filter);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		public static long Count(QiQuery where, IDatabase database = null)
		{
		    IDatabase db = database ?? Db.For<OrganizationData>();
			IQuerySet query = GetQuerySet(db);
			query.Count<OrganizationData>();
			query.Where(where);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		private static OrganizationData CreateFromFilter(IQueryFilter filter, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<OrganizationData>();
			var dao = new OrganizationData();
			filter.Parameters.Each(p=>
			{
				dao.Property(p.ColumnName, p.Value);
			});
			dao.Save(db);
			return dao;
		}

		private static OrganizationData OneOrThrow(OrganizationDataCollection c)
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
