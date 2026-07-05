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
	[Bam.Data.Table("PersonData", "ProfileSchema")]
	public partial class PersonData: Bam.Data.Dao
	{
		public PersonData():base()
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public PersonData(DataRow data)
			: base(data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public PersonData(IDatabase db)
			: base(db)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public PersonData(IDatabase db, DataRow data)
			: base(db, data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		[Bam.Exclude]
		public static implicit operator PersonData(DataRow data)
		{
			return new PersonData(data);
		}

		private void SetChildren()
		{


			if(_database != null)
			{
				this.ChildCollections.Add("GroupDataPersonData_PersonDataId", new GroupDataPersonDataCollection(Database.GetQuery<GroupDataPersonDataColumns, GroupDataPersonData>((c) => c.PersonDataId == GetULongValue("Id", false)), this, "PersonDataId"));
			}
			if(_database != null)
			{
				this.ChildCollections.Add("PersonDataOrganizationData_PersonDataId", new PersonDataOrganizationDataCollection(Database.GetQuery<PersonDataOrganizationDataColumns, PersonDataOrganizationData>((c) => c.PersonDataId == GetULongValue("Id", false)), this, "PersonDataId"));
			}
            this.ChildCollections.Add("PersonData_PersonDataOrganizationData_OrganizationData",  new XrefDaoCollection<PersonDataOrganizationData, OrganizationData>(this, false));

            this.ChildCollections.Add("PersonData_GroupDataPersonData_GroupData",  new XrefDaoCollection<GroupDataPersonData, GroupData>(this, false));

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

    // property:Phone, columnName: Phone	
    [Bam.Data.Column(Name="Phone", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string Phone
    {
        get
        {
            return GetStringValue("Phone");
        }
        set
        {
            SetValue("Phone", value!);
        }
    }

    // property:Email, columnName: Email	
    [Bam.Data.Column(Name="Email", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string Email
    {
        get
        {
            return GetStringValue("Email");
        }
        set
        {
            SetValue("Email", value!);
        }
    }

    // property:FirstName, columnName: FirstName	
    [Bam.Data.Column(Name="FirstName", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string FirstName
    {
        get
        {
            return GetStringValue("FirstName");
        }
        set
        {
            SetValue("FirstName", value!);
        }
    }

    // property:LastName, columnName: LastName	
    [Bam.Data.Column(Name="LastName", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string LastName
    {
        get
        {
            return GetStringValue("LastName");
        }
        set
        {
            SetValue("LastName", value!);
        }
    }

    // property:MiddleName, columnName: MiddleName	
    [Bam.Data.Column(Name="MiddleName", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string MiddleName
    {
        get
        {
            return GetStringValue("MiddleName");
        }
        set
        {
            SetValue("MiddleName", value!);
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
            SetValue("Key", value!);
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
            SetValue("CompositeKeyId", value!);
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
            SetValue("CompositeKey", value!);
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
            SetValue("CreatedBy", value!);
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
            SetValue("ModifiedBy", value!);
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
            SetValue("Modified", value!);
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
            SetValue("Deleted", value!);
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
	public GroupDataPersonDataCollection GroupDataPersonDatasByPersonDataId
	{
		get
		{
			if (this.IsNew)
			{
				throw new InvalidOperationException($"The current instance of type({this.GetType().Name}) hasn't been saved and will have no child collections, call Save() or Save(Database) first.");
			}

			if(!this.ChildCollections.ContainsKey("GroupDataPersonData_PersonDataId"))
			{
				SetChildren();
			}

			var c = (GroupDataPersonDataCollection)this.ChildCollections["GroupDataPersonData_PersonDataId"];
			if(!c.Loaded)
			{
				c.Load(Database);
			}
			return c;
		}
	}
	[Bam.Exclude]
	public PersonDataOrganizationDataCollection PersonDataOrganizationDatasByPersonDataId
	{
		get
		{
			if (this.IsNew)
			{
				throw new InvalidOperationException($"The current instance of type({this.GetType().Name}) hasn't been saved and will have no child collections, call Save() or Save(Database) first.");
			}

			if(!this.ChildCollections.ContainsKey("PersonDataOrganizationData_PersonDataId"))
			{
				SetChildren();
			}

			var c = (PersonDataOrganizationDataCollection)this.ChildCollections["PersonDataOrganizationData_PersonDataId"];
			if(!c.Loaded)
			{
				c.Load(Database);
			}
			return c;
		}
	}

		// Xref       
        public XrefDaoCollection<PersonDataOrganizationData, OrganizationData> OrganizationDatas
        {
            get
            {			
				if (this.IsNew)
				{
					throw new InvalidOperationException($"The current instance of type({this.GetType().Name}) hasn't been saved and will have no child collections, call Save() or Save(Database) first.");
				}

				if(!this.ChildCollections.ContainsKey("PersonData_PersonDataOrganizationData_OrganizationData"))
				{
					SetChildren();
				}

				var xref = (XrefDaoCollection<PersonDataOrganizationData, OrganizationData>)this.ChildCollections["PersonData_PersonDataOrganizationData_OrganizationData"];
				if(!xref.Loaded)
				{
					xref.Load(Database);
				}

				return xref;
            }
        }
		// Xref       
        public XrefDaoCollection<GroupDataPersonData, GroupData> GroupDatas
        {
            get
            {			
				if (this.IsNew)
				{
					throw new InvalidOperationException($"The current instance of type({this.GetType().Name}) hasn't been saved and will have no child collections, call Save() or Save(Database) first.");
				}

				if(!this.ChildCollections.ContainsKey("PersonData_GroupDataPersonData_GroupData"))
				{
					SetChildren();
				}

				var xref = (XrefDaoCollection<GroupDataPersonData, GroupData>)this.ChildCollections["PersonData_GroupDataPersonData_GroupData"];
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
				var colFilter = new PersonDataColumns();
				return (colFilter.KeyColumn == GetDbId());
			}
		}

		/// <summary>
        /// Return every record in the PersonData table.
        /// </summary>
		/// <param name="database">
		/// The database to load from or null
		/// </param>
		public static PersonDataCollection LoadAll(IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<PersonData>();
            ISqlStringBuilder sql = db.GetSqlStringBuilder();
            sql.Select<PersonData>();
            var results = new PersonDataCollection(db, sql.ExecuteGetDataTable(db))
            {
                Database = db
            };
            return results;
        }

        /// <summary>
        /// Process all records in batches of the specified size
        /// </summary>
        [Bam.Exclude]
        public static async Task BatchAll(int batchSize, Action<IEnumerable<PersonData>> batchProcessor, IDatabase database = null!)
		{
			await Task.Run(async ()=>
			{
				PersonDataColumns columns = new PersonDataColumns();
				var orderBy = Bam.Data.Order.By<PersonDataColumns>(c => c.KeyColumn, Bam.Data.SortOrder.Ascending);
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

		public static PersonData GetById(uint? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified PersonData.Id was null");
			return GetById(id!.Value, database);
		}

		public static PersonData GetById(uint id, IDatabase database = null!)
		{
			return GetById((ulong)id, database);
		}

		public static PersonData GetById(int? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified PersonData.Id was null");
			return GetById(id!.Value, database);
		}                                    
                                    
		public static PersonData GetById(int id, IDatabase database = null!)
		{
			return GetById((long)id, database);
		}

		public static PersonData GetById(long? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified PersonData.Id was null");
			return GetById(id!.Value, database);
		}
                                    
		public static PersonData GetById(long id, IDatabase database = null!)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static PersonData GetById(ulong? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified PersonData.Id was null");
			return GetById(id!.Value, database);
		}
                                    
		public static PersonData GetById(ulong id, IDatabase database = null!)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static PersonData GetByUuid(string uuid, IDatabase database = null!)
		{
			return OneWhere(c => Bam.Data.Query.Where("Uuid") == uuid, database);
		}

		public static PersonData GetByCuid(string cuid, IDatabase database = null!)
		{
			return OneWhere(c => Bam.Data.Query.Where("Cuid") == cuid, database);
		}

		[Bam.Exclude]
		public static PersonDataCollection Query(QueryFilter filter, IDatabase database = null!)
		{
			return Where(filter, database);
		}

		[Bam.Exclude]
		public static PersonDataCollection Where(QueryFilter filter, IDatabase database = null!)
		{
			WhereDelegate<PersonDataColumns> whereDelegate = (c) => filter;
			return Where(whereDelegate, database);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A Func delegate that recieves a PersonDataColumns
		/// and returns a QueryFilter which is the result of any comparisons
		/// between PersonDataColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static PersonDataCollection Where(Func<PersonDataColumns, QueryFilter<PersonDataColumns>> where, OrderBy<PersonDataColumns> orderBy = null!, IDatabase database = null!)
		{
			database = database ?? Db.For<PersonData>();
			return new PersonDataCollection(database.GetQuery<PersonDataColumns, PersonData>(where, orderBy), true);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a PersonDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static PersonDataCollection Where(WhereDelegate<PersonDataColumns> where, IDatabase database = null!)
		{
			database = database ?? Db.For<PersonData>();
			var results = new PersonDataCollection(database, database.GetQuery<PersonDataColumns, PersonData>(where), true);
			return results;
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a PersonDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PersonDataCollection Where(WhereDelegate<PersonDataColumns> where, OrderBy<PersonDataColumns> orderBy = null!, IDatabase database = null!)
		{
			database = database ?? Db.For<PersonData>();
			var results = new PersonDataCollection(database, database.GetQuery<PersonDataColumns, PersonData>(where, orderBy), true);
			return results;
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`PersonDataColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static PersonDataCollection Where(QiQuery where, IDatabase database = null!)
		{
			var results = new PersonDataCollection(database, Select<PersonDataColumns>.From<PersonData>().Where(where, database));
			return results;
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static PersonData GetOneWhere(QueryFilter where, IDatabase database = null!)
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
		public static PersonData OneWhere(QueryFilter where, IDatabase database = null!)
		{
			WhereDelegate<PersonDataColumns> whereDelegate = (c) => where;
			var result = Top(1, whereDelegate, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<PersonDataColumns> where, IDatabase database = null!)
		{
			SetOneWhere(where, out PersonData ignore, database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<PersonDataColumns> where, out PersonData result, IDatabase database = null!)
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
		public static PersonData GetOneWhere(WhereDelegate<PersonDataColumns> where, IDatabase database = null!)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				PersonDataColumns c = new PersonDataColumns();
				IQueryFilter filter = where(c);
				result = CreateFromFilter(filter, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.  This method is most commonly used to retrieve a
		/// single PersonData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a PersonDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PersonData OneWhere(WhereDelegate<PersonDataColumns> where, IDatabase database = null!)
		{
			var result = Top(1, where, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`PersonDataColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static PersonData OneWhere(QiQuery where, IDatabase database = null!)
		{
			var results = Top(1, where, database);
			return OneOrThrow(results);
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a PersonDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PersonData FirstOneWhere(WhereDelegate<PersonDataColumns> where, IDatabase database = null!)
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
		/// <param name="where">A WhereDelegate that recieves a PersonDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PersonData FirstOneWhere(WhereDelegate<PersonDataColumns> where, OrderBy<PersonDataColumns> orderBy, IDatabase database = null!)
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
		/// <param name="where">A WhereDelegate that recieves a PersonDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PersonData FirstOneWhere(QueryFilter where, OrderBy<PersonDataColumns> orderBy = null!, IDatabase database = null!)
		{
			WhereDelegate<PersonDataColumns> whereDelegate = (c) => where;
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
		/// <param name="where">A WhereDelegate that recieves a PersonDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PersonDataCollection Top(int count, WhereDelegate<PersonDataColumns> where, IDatabase database = null!)
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
		/// <param name="where">A WhereDelegate that recieves a PersonDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static PersonDataCollection Top(int count, WhereDelegate<PersonDataColumns> where, OrderBy<PersonDataColumns> orderBy, IDatabase database = null!)
		{
			PersonDataColumns c = new PersonDataColumns();
			IQueryFilter filter = where(c);

			IDatabase db = database ?? Db.For<PersonData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<PersonData>(count);
			query.Where(filter);

			if(orderBy != null)
			{
				query.OrderBy<PersonDataColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<PersonDataCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static PersonDataCollection Top(int count, QueryFilter where, IDatabase database)
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
		public static PersonDataCollection Top(int count, QueryFilter where, OrderBy<PersonDataColumns> orderBy = null!, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<PersonData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<PersonData>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy<PersonDataColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<PersonDataCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static PersonDataCollection Top(int count, QueryFilter where, string orderBy = null!, SortOrder sortOrder = SortOrder.Ascending, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<PersonData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<PersonData>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy(orderBy, sortOrder);
			}

			query.Execute(db);
			var results = query.Results.As<PersonDataCollection>(0);
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
		public static PersonDataCollection Top(int count, QiQuery where, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<PersonData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<PersonData>(count);
			query.Where(where);
			query.Execute(db);
			var results = query.Results.As<PersonDataCollection>(0);
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
			IDatabase db = database ?? Db.For<PersonData>();
            IQuerySet query = GetQuerySet(db);
            query.Count<PersonData>();
            query.Execute(db);
            return query.Results.As<CountResult>(0).Value;
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a PersonDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataColumns and other values
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static long Count(WhereDelegate<PersonDataColumns> where, IDatabase database = null!)
		{
			PersonDataColumns c = new PersonDataColumns();
			IQueryFilter filter = where(c) ;

			IDatabase db = database ?? Db.For<PersonData>();
			IQuerySet query = GetQuerySet(db);
			query.Count<PersonData>();
			query.Where(filter);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		public static long Count(QiQuery where, IDatabase database = null!)
		{
		    IDatabase db = database ?? Db.For<PersonData>();
			IQuerySet query = GetQuerySet(db);
			query.Count<PersonData>();
			query.Where(where);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		private static PersonData CreateFromFilter(IQueryFilter filter, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<PersonData>();
			var dao = new PersonData();
			filter.Parameters.Each(p=>
			{
				dao.Property(p.ColumnName, p.Value!);
			});
			dao.Save(db);
			return dao;
		}

		private static PersonData OneOrThrow(PersonDataCollection c)
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
