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
	[Bam.Data.Table("AdditionalProperty", "ProfileSchema")]
	public partial class AdditionalProperty: Bam.Data.Dao
	{
		public AdditionalProperty():base()
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public AdditionalProperty(DataRow data)
			: base(data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public AdditionalProperty(IDatabase db)
			: base(db)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public AdditionalProperty(IDatabase db, DataRow data)
			: base(db, data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		[Bam.Exclude]
		public static implicit operator AdditionalProperty(DataRow data)
		{
			return new AdditionalProperty(data);
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

    // property:Value, columnName: Value	
    [Bam.Data.Column(Name="Value", DbDataType="VarChar", MaxLength="4000", AllowNull=true)]
    public string Value
    {
        get
        {
            return GetStringValue("Value");
        }
        set
        {
            SetValue("Value", value);
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
				var colFilter = new AdditionalPropertyColumns();
				return (colFilter.KeyColumn == GetDbId());
			}
		}

		/// <summary>
        /// Return every record in the AdditionalProperty table.
        /// </summary>
		/// <param name="database">
		/// The database to load from or null
		/// </param>
		public static AdditionalPropertyCollection LoadAll(IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<AdditionalProperty>();
            ISqlStringBuilder sql = db.GetSqlStringBuilder();
            sql.Select<AdditionalProperty>();
            var results = new AdditionalPropertyCollection(db, sql.ExecuteGetDataTable(db))
            {
                Database = db
            };
            return results;
        }

        /// <summary>
        /// Process all records in batches of the specified size
        /// </summary>
        [Bam.Exclude]
        public static async Task BatchAll(int batchSize, Action<IEnumerable<AdditionalProperty>> batchProcessor, IDatabase database = null)
		{
			await Task.Run(async ()=>
			{
				AdditionalPropertyColumns columns = new AdditionalPropertyColumns();
				var orderBy = Bam.Data.Order.By<AdditionalPropertyColumns>(c => c.KeyColumn, Bam.Data.SortOrder.Ascending);
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

		public static AdditionalProperty GetById(uint? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified AdditionalProperty.Id was null");
			return GetById(id.Value, database);
		}

		public static AdditionalProperty GetById(uint id, IDatabase database = null)
		{
			return GetById((ulong)id, database);
		}

		public static AdditionalProperty GetById(int? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified AdditionalProperty.Id was null");
			return GetById(id.Value, database);
		}                                    
                                    
		public static AdditionalProperty GetById(int id, IDatabase database = null)
		{
			return GetById((long)id, database);
		}

		public static AdditionalProperty GetById(long? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified AdditionalProperty.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static AdditionalProperty GetById(long id, IDatabase database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static AdditionalProperty GetById(ulong? id, IDatabase database = null)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified AdditionalProperty.Id was null");
			return GetById(id.Value, database);
		}
                                    
		public static AdditionalProperty GetById(ulong id, IDatabase database = null)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static AdditionalProperty GetByUuid(string uuid, IDatabase database = null)
		{
			return OneWhere(c => Bam.Data.Query.Where("Uuid") == uuid, database);
		}

		public static AdditionalProperty GetByCuid(string cuid, IDatabase database = null)
		{
			return OneWhere(c => Bam.Data.Query.Where("Cuid") == cuid, database);
		}

		[Bam.Exclude]
		public static AdditionalPropertyCollection Query(QueryFilter filter, IDatabase database = null)
		{
			return Where(filter, database);
		}

		[Bam.Exclude]
		public static AdditionalPropertyCollection Where(QueryFilter filter, IDatabase database = null)
		{
			WhereDelegate<AdditionalPropertyColumns> whereDelegate = (c) => filter;
			return Where(whereDelegate, database);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A Func delegate that recieves a AdditionalPropertyColumns
		/// and returns a QueryFilter which is the result of any comparisons
		/// between AdditionalPropertyColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static AdditionalPropertyCollection Where(Func<AdditionalPropertyColumns, QueryFilter<AdditionalPropertyColumns>> where, OrderBy<AdditionalPropertyColumns> orderBy = null, IDatabase database = null)
		{
			database = database ?? Db.For<AdditionalProperty>();
			return new AdditionalPropertyCollection(database.GetQuery<AdditionalPropertyColumns, AdditionalProperty>(where, orderBy), true);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a AdditionalPropertyColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AdditionalPropertyColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static AdditionalPropertyCollection Where(WhereDelegate<AdditionalPropertyColumns> where, IDatabase database = null)
		{
			database = database ?? Db.For<AdditionalProperty>();
			var results = new AdditionalPropertyCollection(database, database.GetQuery<AdditionalPropertyColumns, AdditionalProperty>(where), true);
			return results;
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a AdditionalPropertyColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AdditionalPropertyColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static AdditionalPropertyCollection Where(WhereDelegate<AdditionalPropertyColumns> where, OrderBy<AdditionalPropertyColumns> orderBy = null, IDatabase database = null)
		{
			database = database ?? Db.For<AdditionalProperty>();
			var results = new AdditionalPropertyCollection(database, database.GetQuery<AdditionalPropertyColumns, AdditionalProperty>(where, orderBy), true);
			return results;
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`AdditionalPropertyColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static AdditionalPropertyCollection Where(QiQuery where, IDatabase database = null)
		{
			var results = new AdditionalPropertyCollection(database, Select<AdditionalPropertyColumns>.From<AdditionalProperty>().Where(where, database));
			return results;
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static AdditionalProperty GetOneWhere(QueryFilter where, IDatabase database = null)
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
		public static AdditionalProperty OneWhere(QueryFilter where, IDatabase database = null)
		{
			WhereDelegate<AdditionalPropertyColumns> whereDelegate = (c) => where;
			var result = Top(1, whereDelegate, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<AdditionalPropertyColumns> where, IDatabase database = null)
		{
			SetOneWhere(where, out AdditionalProperty ignore, database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<AdditionalPropertyColumns> where, out AdditionalProperty result, IDatabase database = null)
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
		public static AdditionalProperty GetOneWhere(WhereDelegate<AdditionalPropertyColumns> where, IDatabase database = null)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				AdditionalPropertyColumns c = new AdditionalPropertyColumns();
				IQueryFilter filter = where(c);
				result = CreateFromFilter(filter, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.  This method is most commonly used to retrieve a
		/// single AdditionalProperty instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a AdditionalPropertyColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AdditionalPropertyColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static AdditionalProperty OneWhere(WhereDelegate<AdditionalPropertyColumns> where, IDatabase database = null)
		{
			var result = Top(1, where, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`AdditionalPropertyColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static AdditionalProperty OneWhere(QiQuery where, IDatabase database = null)
		{
			var results = Top(1, where, database);
			return OneOrThrow(results);
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a AdditionalPropertyColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AdditionalPropertyColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static AdditionalProperty FirstOneWhere(WhereDelegate<AdditionalPropertyColumns> where, IDatabase database = null)
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
		/// <param name="where">A WhereDelegate that recieves a AdditionalPropertyColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AdditionalPropertyColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static AdditionalProperty FirstOneWhere(WhereDelegate<AdditionalPropertyColumns> where, OrderBy<AdditionalPropertyColumns> orderBy, IDatabase database = null)
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
		/// <param name="where">A WhereDelegate that recieves a AdditionalPropertyColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AdditionalPropertyColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static AdditionalProperty FirstOneWhere(QueryFilter where, OrderBy<AdditionalPropertyColumns> orderBy = null, IDatabase database = null)
		{
			WhereDelegate<AdditionalPropertyColumns> whereDelegate = (c) => where;
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
		/// <param name="where">A WhereDelegate that recieves a AdditionalPropertyColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AdditionalPropertyColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static AdditionalPropertyCollection Top(int count, WhereDelegate<AdditionalPropertyColumns> where, IDatabase database = null)
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
		/// <param name="where">A WhereDelegate that recieves a AdditionalPropertyColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AdditionalPropertyColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static AdditionalPropertyCollection Top(int count, WhereDelegate<AdditionalPropertyColumns> where, OrderBy<AdditionalPropertyColumns> orderBy, IDatabase database = null)
		{
			AdditionalPropertyColumns c = new AdditionalPropertyColumns();
			IQueryFilter filter = where(c);

			IDatabase db = database ?? Db.For<AdditionalProperty>();
			IQuerySet query = GetQuerySet(db);
			query.Top<AdditionalProperty>(count);
			query.Where(filter);

			if(orderBy != null)
			{
				query.OrderBy<AdditionalPropertyColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<AdditionalPropertyCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static AdditionalPropertyCollection Top(int count, QueryFilter where, IDatabase database)
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
		public static AdditionalPropertyCollection Top(int count, QueryFilter where, OrderBy<AdditionalPropertyColumns> orderBy = null, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<AdditionalProperty>();
			IQuerySet query = GetQuerySet(db);
			query.Top<AdditionalProperty>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy<AdditionalPropertyColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<AdditionalPropertyCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static AdditionalPropertyCollection Top(int count, QueryFilter where, string orderBy = null, SortOrder sortOrder = SortOrder.Ascending, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<AdditionalProperty>();
			IQuerySet query = GetQuerySet(db);
			query.Top<AdditionalProperty>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy(orderBy, sortOrder);
			}

			query.Execute(db);
			var results = query.Results.As<AdditionalPropertyCollection>(0);
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
		public static AdditionalPropertyCollection Top(int count, QiQuery where, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<AdditionalProperty>();
			IQuerySet query = GetQuerySet(db);
			query.Top<AdditionalProperty>(count);
			query.Where(where);
			query.Execute(db);
			var results = query.Results.As<AdditionalPropertyCollection>(0);
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
			IDatabase db = database ?? Db.For<AdditionalProperty>();
            IQuerySet query = GetQuerySet(db);
            query.Count<AdditionalProperty>();
            query.Execute(db);
            return (long)query.Results[0].DataRow[0];
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a AdditionalPropertyColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AdditionalPropertyColumns and other values
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static long Count(WhereDelegate<AdditionalPropertyColumns> where, IDatabase database = null)
		{
			AdditionalPropertyColumns c = new AdditionalPropertyColumns();
			IQueryFilter filter = where(c) ;

			IDatabase db = database ?? Db.For<AdditionalProperty>();
			IQuerySet query = GetQuerySet(db);
			query.Count<AdditionalProperty>();
			query.Where(filter);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		public static long Count(QiQuery where, IDatabase database = null)
		{
		    IDatabase db = database ?? Db.For<AdditionalProperty>();
			IQuerySet query = GetQuerySet(db);
			query.Count<AdditionalProperty>();
			query.Where(where);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		private static AdditionalProperty CreateFromFilter(IQueryFilter filter, IDatabase database = null)
		{
			IDatabase db = database ?? Db.For<AdditionalProperty>();
			var dao = new AdditionalProperty();
			filter.Parameters.Each(p=>
			{
				dao.Property(p.ColumnName, p.Value);
			});
			dao.Save(db);
			return dao;
		}

		private static AdditionalProperty OneOrThrow(AdditionalPropertyCollection c)
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
