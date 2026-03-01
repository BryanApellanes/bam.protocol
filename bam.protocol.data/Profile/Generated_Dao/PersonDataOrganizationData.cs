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
	[Bam.Data.Table("PersonDataOrganizationData", "ProfileSchema")]
	public partial class PersonDataOrganizationData: Bam.Data.Dao
	{
		public PersonDataOrganizationData():base()
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public PersonDataOrganizationData(DataRow data)
			: base(data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public PersonDataOrganizationData(IDatabase db)
			: base(db)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		public PersonDataOrganizationData(IDatabase db, DataRow data)
			: base(db, data)
		{
			this.SetKeyColumnName();
			this.SetChildren();
		}

		[Bam.Exclude]
		public static implicit operator PersonDataOrganizationData(DataRow data)
		{
			return new PersonDataOrganizationData(data);
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


	// start PersonDataId -> PersonDataId
	[Bam.Data.ForeignKey(
        Table="PersonDataOrganizationData",
		Name="PersonDataId",
		DbDataType="BigInt",
		MaxLength="",
		AllowNull=false,
		ReferencedKey="Id",
		ReferencedTable="PersonData",
		Suffix="1")]
	public ulong? PersonDataId
	{
		get
		{
			return GetULongValue("PersonDataId", false);
		}
		set
		{
			SetValue("PersonDataId", value!, false);
		}
	}

    PersonData _personDataOfPersonDataId;
	public PersonData PersonDataOfPersonDataId
	{
		get
		{
			if(_personDataOfPersonDataId == null)
			{
				_personDataOfPersonDataId = Bam.Protocol.Data.Profile.Dao.PersonData.OneWhere(c => c.KeyColumn == this.PersonDataId, this.Database);
			}
			return _personDataOfPersonDataId;
		}
	}

	// start OrganizationDataId -> OrganizationDataId
	[Bam.Data.ForeignKey(
        Table="PersonDataOrganizationData",
		Name="OrganizationDataId",
		DbDataType="BigInt",
		MaxLength="",
		AllowNull=false,
		ReferencedKey="Id",
		ReferencedTable="OrganizationData",
		Suffix="2")]
	public ulong? OrganizationDataId
	{
		get
		{
			return GetULongValue("OrganizationDataId", false);
		}
		set
		{
			SetValue("OrganizationDataId", value!, false);
		}
	}

    OrganizationData _organizationDataOfOrganizationDataId;
	public OrganizationData OrganizationDataOfOrganizationDataId
	{
		get
		{
			if(_organizationDataOfOrganizationDataId == null)
			{
				_organizationDataOfOrganizationDataId = Bam.Protocol.Data.Profile.Dao.OrganizationData.OneWhere(c => c.KeyColumn == this.OrganizationDataId, this.Database);
			}
			return _organizationDataOfOrganizationDataId;
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
				var colFilter = new PersonDataOrganizationDataColumns();
				return (colFilter.KeyColumn == GetDbId());
			}
		}

		/// <summary>
        /// Return every record in the PersonDataOrganizationData table.
        /// </summary>
		/// <param name="database">
		/// The database to load from or null
		/// </param>
		public static PersonDataOrganizationDataCollection LoadAll(IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<PersonDataOrganizationData>();
            ISqlStringBuilder sql = db.GetSqlStringBuilder();
            sql.Select<PersonDataOrganizationData>();
            var results = new PersonDataOrganizationDataCollection(db, sql.ExecuteGetDataTable(db))
            {
                Database = db
            };
            return results;
        }

        /// <summary>
        /// Process all records in batches of the specified size
        /// </summary>
        [Bam.Exclude]
        public static async Task BatchAll(int batchSize, Action<IEnumerable<PersonDataOrganizationData>> batchProcessor, IDatabase database = null!)
		{
			await Task.Run(async ()=>
			{
				PersonDataOrganizationDataColumns columns = new PersonDataOrganizationDataColumns();
				var orderBy = Bam.Data.Order.By<PersonDataOrganizationDataColumns>(c => c.KeyColumn, Bam.Data.SortOrder.Ascending);
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

		public static PersonDataOrganizationData GetById(uint? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified PersonDataOrganizationData.Id was null");
			return GetById(id!.Value, database);
		}

		public static PersonDataOrganizationData GetById(uint id, IDatabase database = null!)
		{
			return GetById((ulong)id, database);
		}

		public static PersonDataOrganizationData GetById(int? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified PersonDataOrganizationData.Id was null");
			return GetById(id!.Value, database);
		}                                    
                                    
		public static PersonDataOrganizationData GetById(int id, IDatabase database = null!)
		{
			return GetById((long)id, database);
		}

		public static PersonDataOrganizationData GetById(long? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified PersonDataOrganizationData.Id was null");
			return GetById(id!.Value, database);
		}
                                    
		public static PersonDataOrganizationData GetById(long id, IDatabase database = null!)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static PersonDataOrganizationData GetById(ulong? id, IDatabase database = null!)
		{
			Args.ThrowIfNull(id, "id");
			Args.ThrowIf(!id.HasValue, "specified PersonDataOrganizationData.Id was null");
			return GetById(id!.Value, database);
		}
                                    
		public static PersonDataOrganizationData GetById(ulong id, IDatabase database = null!)
		{
			return OneWhere(c => c.KeyColumn == id, database);
		}

		public static PersonDataOrganizationData GetByUuid(string uuid, IDatabase database = null!)
		{
			return OneWhere(c => Bam.Data.Query.Where("Uuid") == uuid, database);
		}

		public static PersonDataOrganizationData GetByCuid(string cuid, IDatabase database = null!)
		{
			return OneWhere(c => Bam.Data.Query.Where("Cuid") == cuid, database);
		}

		[Bam.Exclude]
		public static PersonDataOrganizationDataCollection Query(QueryFilter filter, IDatabase database = null!)
		{
			return Where(filter, database);
		}

		[Bam.Exclude]
		public static PersonDataOrganizationDataCollection Where(QueryFilter filter, IDatabase database = null!)
		{
			WhereDelegate<PersonDataOrganizationDataColumns> whereDelegate = (c) => filter;
			return Where(whereDelegate, database);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A Func delegate that recieves a PersonDataOrganizationDataColumns
		/// and returns a QueryFilter which is the result of any comparisons
		/// between PersonDataOrganizationDataColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static PersonDataOrganizationDataCollection Where(Func<PersonDataOrganizationDataColumns, QueryFilter<PersonDataOrganizationDataColumns>> where, OrderBy<PersonDataOrganizationDataColumns> orderBy = null!, IDatabase database = null!)
		{
			database = database ?? Db.For<PersonDataOrganizationData>();
			return new PersonDataOrganizationDataCollection(database.GetQuery<PersonDataOrganizationDataColumns, PersonDataOrganizationData>(where, orderBy), true);
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a PersonDataOrganizationDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataOrganizationDataColumns and other values
		/// </param>
		/// <param name="db"></param>
		[Bam.Exclude]
		public static PersonDataOrganizationDataCollection Where(WhereDelegate<PersonDataOrganizationDataColumns> where, IDatabase database = null!)
		{
			database = database ?? Db.For<PersonDataOrganizationData>();
			var results = new PersonDataOrganizationDataCollection(database, database.GetQuery<PersonDataOrganizationDataColumns, PersonDataOrganizationData>(where), true);
			return results;
		}

		/// <summary>
		/// Execute a query and return the results.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a PersonDataOrganizationDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataOrganizationDataColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PersonDataOrganizationDataCollection Where(WhereDelegate<PersonDataOrganizationDataColumns> where, OrderBy<PersonDataOrganizationDataColumns> orderBy = null!, IDatabase database = null!)
		{
			database = database ?? Db.For<PersonDataOrganizationData>();
			var results = new PersonDataOrganizationDataCollection(database, database.GetQuery<PersonDataOrganizationDataColumns, PersonDataOrganizationData>(where, orderBy), true);
			return results;
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`PersonDataOrganizationDataColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static PersonDataOrganizationDataCollection Where(QiQuery where, IDatabase database = null!)
		{
			var results = new PersonDataOrganizationDataCollection(database, Select<PersonDataOrganizationDataColumns>.From<PersonDataOrganizationData>().Where(where, database));
			return results;
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static PersonDataOrganizationData GetOneWhere(QueryFilter where, IDatabase database = null!)
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
		public static PersonDataOrganizationData OneWhere(QueryFilter where, IDatabase database = null!)
		{
			WhereDelegate<PersonDataOrganizationDataColumns> whereDelegate = (c) => where;
			var result = Top(1, whereDelegate, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<PersonDataOrganizationDataColumns> where, IDatabase database = null!)
		{
			SetOneWhere(where, out PersonDataOrganizationData ignore, database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists
		/// one will be created; success will depend on the nullability
		/// of the specified columns.
		/// </summary>
		[Bam.Exclude]
		public static void SetOneWhere(WhereDelegate<PersonDataOrganizationDataColumns> where, out PersonDataOrganizationData result, IDatabase database = null!)
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
		public static PersonDataOrganizationData GetOneWhere(WhereDelegate<PersonDataOrganizationDataColumns> where, IDatabase database = null!)
		{
			var result = OneWhere(where, database);
			if(result == null)
			{
				PersonDataOrganizationDataColumns c = new PersonDataOrganizationDataColumns();
				IQueryFilter filter = where(c);
				result = CreateFromFilter(filter, database);
			}

			return result;
		}

		/// <summary>
		/// Execute a query that should return only one result.  If more
		/// than one result is returned a MultipleEntriesFoundException will
		/// be thrown.  This method is most commonly used to retrieve a
		/// single PersonDataOrganizationData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a PersonDataOrganizationDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataOrganizationDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PersonDataOrganizationData OneWhere(WhereDelegate<PersonDataOrganizationDataColumns> where, IDatabase database = null!)
		{
			var result = Top(1, where, database);
			return OneOrThrow(result);
		}

		/// <summary>
		/// This method is intended to respond to client side Qi queries.
		/// Use of this method from .Net should be avoided in favor of
		/// one of the methods that take a delegate of type
		/// WhereDelegate`PersonDataOrganizationDataColumns`.
		/// </summary>
		/// <param name="where"></param>
		/// <param name="database"></param>
		public static PersonDataOrganizationData OneWhere(QiQuery where, IDatabase database = null!)
		{
			var results = Top(1, where, database);
			return OneOrThrow(results);
		}

		/// <summary>
		/// Execute a query and return the first result.  This method will issue a sql TOP clause so only the
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a PersonDataOrganizationDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataOrganizationDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PersonDataOrganizationData FirstOneWhere(WhereDelegate<PersonDataOrganizationDataColumns> where, IDatabase database = null!)
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
		/// <param name="where">A WhereDelegate that recieves a PersonDataOrganizationDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataOrganizationDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PersonDataOrganizationData FirstOneWhere(WhereDelegate<PersonDataOrganizationDataColumns> where, OrderBy<PersonDataOrganizationDataColumns> orderBy, IDatabase database = null!)
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
		/// <param name="where">A WhereDelegate that recieves a PersonDataOrganizationDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataOrganizationDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PersonDataOrganizationData FirstOneWhere(QueryFilter where, OrderBy<PersonDataOrganizationDataColumns> orderBy = null!, IDatabase database = null!)
		{
			WhereDelegate<PersonDataOrganizationDataColumns> whereDelegate = (c) => where;
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
		/// <param name="where">A WhereDelegate that recieves a PersonDataOrganizationDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataOrganizationDataColumns and other values
		/// </param>
		/// <param name="database"></param>
		[Bam.Exclude]
		public static PersonDataOrganizationDataCollection Top(int count, WhereDelegate<PersonDataOrganizationDataColumns> where, IDatabase database = null!)
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
		/// <param name="where">A WhereDelegate that recieves a PersonDataOrganizationDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataOrganizationDataColumns and other values
		/// </param>
		/// <param name="orderBy">
		/// Specifies what column and direction to order the results by
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static PersonDataOrganizationDataCollection Top(int count, WhereDelegate<PersonDataOrganizationDataColumns> where, OrderBy<PersonDataOrganizationDataColumns> orderBy, IDatabase database = null!)
		{
			PersonDataOrganizationDataColumns c = new PersonDataOrganizationDataColumns();
			IQueryFilter filter = where(c);

			IDatabase db = database ?? Db.For<PersonDataOrganizationData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<PersonDataOrganizationData>(count);
			query.Where(filter);

			if(orderBy != null)
			{
				query.OrderBy<PersonDataOrganizationDataColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<PersonDataOrganizationDataCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static PersonDataOrganizationDataCollection Top(int count, QueryFilter where, IDatabase database)
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
		public static PersonDataOrganizationDataCollection Top(int count, QueryFilter where, OrderBy<PersonDataOrganizationDataColumns> orderBy = null!, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<PersonDataOrganizationData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<PersonDataOrganizationData>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy<PersonDataOrganizationDataColumns>(orderBy);
			}

			query.Execute(db);
			var results = query.Results.As<PersonDataOrganizationDataCollection>(0);
			results.Database = db;
			return results;
		}

		[Bam.Exclude]
		public static PersonDataOrganizationDataCollection Top(int count, QueryFilter where, string orderBy = null!, SortOrder sortOrder = SortOrder.Ascending, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<PersonDataOrganizationData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<PersonDataOrganizationData>(count);
			query.Where(where);

			if(orderBy != null)
			{
				query.OrderBy(orderBy, sortOrder);
			}

			query.Execute(db);
			var results = query.Results.As<PersonDataOrganizationDataCollection>(0);
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
		public static PersonDataOrganizationDataCollection Top(int count, QiQuery where, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<PersonDataOrganizationData>();
			IQuerySet query = GetQuerySet(db);
			query.Top<PersonDataOrganizationData>(count);
			query.Where(where);
			query.Execute(db);
			var results = query.Results.As<PersonDataOrganizationDataCollection>(0);
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
			IDatabase db = database ?? Db.For<PersonDataOrganizationData>();
            IQuerySet query = GetQuerySet(db);
            query.Count<PersonDataOrganizationData>();
            query.Execute(db);
            return query.Results.As<CountResult>(0).Value;
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that recieves a PersonDataOrganizationDataColumns
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataOrganizationDataColumns and other values
		/// </param>
		/// <param name="database">
		/// Which database to query or null to use the default
		/// </param>
		[Bam.Exclude]
		public static long Count(WhereDelegate<PersonDataOrganizationDataColumns> where, IDatabase database = null!)
		{
			PersonDataOrganizationDataColumns c = new PersonDataOrganizationDataColumns();
			IQueryFilter filter = where(c) ;

			IDatabase db = database ?? Db.For<PersonDataOrganizationData>();
			IQuerySet query = GetQuerySet(db);
			query.Count<PersonDataOrganizationData>();
			query.Where(filter);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		public static long Count(QiQuery where, IDatabase database = null!)
		{
		    IDatabase db = database ?? Db.For<PersonDataOrganizationData>();
			IQuerySet query = GetQuerySet(db);
			query.Count<PersonDataOrganizationData>();
			query.Where(where);
			query.Execute(db);
			return query.Results.As<CountResult>(0).Value;
		}

		private static PersonDataOrganizationData CreateFromFilter(IQueryFilter filter, IDatabase database = null!)
		{
			IDatabase db = database ?? Db.For<PersonDataOrganizationData>();
			var dao = new PersonDataOrganizationData();
			filter.Parameters.Each(p=>
			{
				dao.Property(p.ColumnName, p.Value!);
			});
			dao.Save(db);
			return dao;
		}

		private static PersonDataOrganizationData OneOrThrow(PersonDataOrganizationDataCollection c)
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
