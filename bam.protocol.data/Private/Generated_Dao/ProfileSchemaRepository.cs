/*
This file was generated and should not be modified directly
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Bam;
using Bam.Data;
using Bam.Data.Repositories;
using Bam.Protocol.Data.Private;

namespace Bam.Protocol.Data.Private.Dao.Repository
{
	[Serializable]
	public partial class ProfileSchemaRepository: DaoRepository
	{
		public ProfileSchemaRepository()
		{
			SchemaName = "ProfileSchema";
			BaseNamespace = "Bam.Protocol.Data.Private";

			
			AddType<Bam.Protocol.Data.Private.EccPrivateKeyData>();
			
			
			AddType<Bam.Protocol.Data.Private.PrivateKeySetData>();
			
			
			AddType<Bam.Protocol.Data.Private.RsaPrivateKeyData>();
			

			DaoAssembly = typeof(ProfileSchemaRepository).Assembly;
		}

		object _addLock = new object();
        public override void AddType(Type type)
        {
            lock (_addLock)
            {
                base.AddType(type);
                DaoAssembly = typeof(ProfileSchemaRepository).Assembly;
            }
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneEccPrivateKeyDataWhere(WhereDelegate<EccPrivateKeyDataColumns> where)
		{
			Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneEccPrivateKeyDataWhere(WhereDelegate<EccPrivateKeyDataColumns> where, out Bam.Protocol.Data.Private.EccPrivateKeyData result)
		{
			Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.SetOneWhere(where, out Bam.Protocol.Data.Private.Dao.EccPrivateKeyData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Private.EccPrivateKeyData>()!;
            result = new DaoRepoData<Bam.Protocol.Data.Private.EccPrivateKeyData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Private.EccPrivateKeyData GetOneEccPrivateKeyDataWhere(WhereDelegate<EccPrivateKeyDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Private.EccPrivateKeyData>();
			var data = (Bam.Protocol.Data.Private.EccPrivateKeyData)Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Private.EccPrivateKeyData>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single EccPrivateKeyData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a EccPrivateKeyDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between EccPrivateKeyDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Private.EccPrivateKeyData OneEccPrivateKeyDataWhere(WhereDelegate<EccPrivateKeyDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Private.EccPrivateKeyData>();
            var data = (Bam.Protocol.Data.Private.EccPrivateKeyData)Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Private.EccPrivateKeyData>(data!, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Private.EccPrivateKeyDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Private.EccPrivateKeyDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Private.EccPrivateKeyData> EccPrivateKeyDatasWhere(WhereDelegate<EccPrivateKeyDataColumns> where, OrderBy<EccPrivateKeyDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Private.EccPrivateKeyData>(Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.Where(where, orderBy, Database));
        }
		
		/// <summary>
		/// Execute a query and return the specified number
		/// of values. This method issues a sql TOP clause so only the 
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="count">The number of values to return.
		/// This value is used in the sql query so no more than this 
		/// number of values will be returned by the database.
		/// </param>
		/// <param name="where">A WhereDelegate that receives a EccPrivateKeyDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between EccPrivateKeyDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Private.EccPrivateKeyData> TopEccPrivateKeyDatasWhere(int count, WhereDelegate<EccPrivateKeyDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Private.EccPrivateKeyData>(Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Private.EccPrivateKeyData> TopEccPrivateKeyDatasWhere(int count, WhereDelegate<EccPrivateKeyDataColumns> where, OrderBy<EccPrivateKeyDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Private.EccPrivateKeyData>(Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of EccPrivateKeyDatas
		/// </summary>
		public long CountEccPrivateKeyDatas()
        {
            return Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a EccPrivateKeyDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between EccPrivateKeyDataColumns and other values
		/// </param>
        public long CountEccPrivateKeyDatasWhere(WhereDelegate<EccPrivateKeyDataColumns> where)
        {
            return Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.Count(where, Database);
        }
        
        /*public async Task BatchQueryEccPrivateKeyDatas(int batchSize, WhereDelegate<EccPrivateKeyDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Private.EccPrivateKeyData>> batchProcessor)
        {
            await Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Private.EccPrivateKeyData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllEccPrivateKeyDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Private.EccPrivateKeyData>> batchProcessor)
        {
            await Bam.Protocol.Data.Private.Dao.EccPrivateKeyData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Private.EccPrivateKeyData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOnePrivateKeySetDataWhere(WhereDelegate<PrivateKeySetDataColumns> where)
		{
			Bam.Protocol.Data.Private.Dao.PrivateKeySetData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOnePrivateKeySetDataWhere(WhereDelegate<PrivateKeySetDataColumns> where, out Bam.Protocol.Data.Private.PrivateKeySetData result)
		{
			Bam.Protocol.Data.Private.Dao.PrivateKeySetData.SetOneWhere(where, out Bam.Protocol.Data.Private.Dao.PrivateKeySetData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Private.PrivateKeySetData>()!;
            result = new DaoRepoData<Bam.Protocol.Data.Private.PrivateKeySetData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Private.PrivateKeySetData GetOnePrivateKeySetDataWhere(WhereDelegate<PrivateKeySetDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Private.PrivateKeySetData>();
			var data = (Bam.Protocol.Data.Private.PrivateKeySetData)Bam.Protocol.Data.Private.Dao.PrivateKeySetData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Private.PrivateKeySetData>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single PrivateKeySetData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a PrivateKeySetDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PrivateKeySetDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Private.PrivateKeySetData OnePrivateKeySetDataWhere(WhereDelegate<PrivateKeySetDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Private.PrivateKeySetData>();
            var data = (Bam.Protocol.Data.Private.PrivateKeySetData)Bam.Protocol.Data.Private.Dao.PrivateKeySetData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Private.PrivateKeySetData>(data!, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Private.PrivateKeySetDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Private.PrivateKeySetDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Private.PrivateKeySetData> PrivateKeySetDatasWhere(WhereDelegate<PrivateKeySetDataColumns> where, OrderBy<PrivateKeySetDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Private.PrivateKeySetData>(Bam.Protocol.Data.Private.Dao.PrivateKeySetData.Where(where, orderBy, Database));
        }
		
		/// <summary>
		/// Execute a query and return the specified number
		/// of values. This method issues a sql TOP clause so only the 
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="count">The number of values to return.
		/// This value is used in the sql query so no more than this 
		/// number of values will be returned by the database.
		/// </param>
		/// <param name="where">A WhereDelegate that receives a PrivateKeySetDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PrivateKeySetDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Private.PrivateKeySetData> TopPrivateKeySetDatasWhere(int count, WhereDelegate<PrivateKeySetDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Private.PrivateKeySetData>(Bam.Protocol.Data.Private.Dao.PrivateKeySetData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Private.PrivateKeySetData> TopPrivateKeySetDatasWhere(int count, WhereDelegate<PrivateKeySetDataColumns> where, OrderBy<PrivateKeySetDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Private.PrivateKeySetData>(Bam.Protocol.Data.Private.Dao.PrivateKeySetData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of PrivateKeySetDatas
		/// </summary>
		public long CountPrivateKeySetDatas()
        {
            return Bam.Protocol.Data.Private.Dao.PrivateKeySetData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a PrivateKeySetDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PrivateKeySetDataColumns and other values
		/// </param>
        public long CountPrivateKeySetDatasWhere(WhereDelegate<PrivateKeySetDataColumns> where)
        {
            return Bam.Protocol.Data.Private.Dao.PrivateKeySetData.Count(where, Database);
        }
        
        /*public async Task BatchQueryPrivateKeySetDatas(int batchSize, WhereDelegate<PrivateKeySetDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Private.PrivateKeySetData>> batchProcessor)
        {
            await Bam.Protocol.Data.Private.Dao.PrivateKeySetData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Private.PrivateKeySetData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllPrivateKeySetDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Private.PrivateKeySetData>> batchProcessor)
        {
            await Bam.Protocol.Data.Private.Dao.PrivateKeySetData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Private.PrivateKeySetData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneRsaPrivateKeyDataWhere(WhereDelegate<RsaPrivateKeyDataColumns> where)
		{
			Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneRsaPrivateKeyDataWhere(WhereDelegate<RsaPrivateKeyDataColumns> where, out Bam.Protocol.Data.Private.RsaPrivateKeyData result)
		{
			Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.SetOneWhere(where, out Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Private.RsaPrivateKeyData>()!;
            result = new DaoRepoData<Bam.Protocol.Data.Private.RsaPrivateKeyData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Private.RsaPrivateKeyData GetOneRsaPrivateKeyDataWhere(WhereDelegate<RsaPrivateKeyDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Private.RsaPrivateKeyData>();
			var data = (Bam.Protocol.Data.Private.RsaPrivateKeyData)Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Private.RsaPrivateKeyData>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single RsaPrivateKeyData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a RsaPrivateKeyDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between RsaPrivateKeyDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Private.RsaPrivateKeyData OneRsaPrivateKeyDataWhere(WhereDelegate<RsaPrivateKeyDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Private.RsaPrivateKeyData>();
            var data = (Bam.Protocol.Data.Private.RsaPrivateKeyData)Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Private.RsaPrivateKeyData>(data!, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Private.RsaPrivateKeyDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Private.RsaPrivateKeyDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Private.RsaPrivateKeyData> RsaPrivateKeyDatasWhere(WhereDelegate<RsaPrivateKeyDataColumns> where, OrderBy<RsaPrivateKeyDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Private.RsaPrivateKeyData>(Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.Where(where, orderBy, Database));
        }
		
		/// <summary>
		/// Execute a query and return the specified number
		/// of values. This method issues a sql TOP clause so only the 
		/// specified number of values will be returned.
		/// </summary>
		/// <param name="count">The number of values to return.
		/// This value is used in the sql query so no more than this 
		/// number of values will be returned by the database.
		/// </param>
		/// <param name="where">A WhereDelegate that receives a RsaPrivateKeyDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between RsaPrivateKeyDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Private.RsaPrivateKeyData> TopRsaPrivateKeyDatasWhere(int count, WhereDelegate<RsaPrivateKeyDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Private.RsaPrivateKeyData>(Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Private.RsaPrivateKeyData> TopRsaPrivateKeyDatasWhere(int count, WhereDelegate<RsaPrivateKeyDataColumns> where, OrderBy<RsaPrivateKeyDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Private.RsaPrivateKeyData>(Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of RsaPrivateKeyDatas
		/// </summary>
		public long CountRsaPrivateKeyDatas()
        {
            return Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a RsaPrivateKeyDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between RsaPrivateKeyDataColumns and other values
		/// </param>
        public long CountRsaPrivateKeyDatasWhere(WhereDelegate<RsaPrivateKeyDataColumns> where)
        {
            return Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.Count(where, Database);
        }
        
        /*public async Task BatchQueryRsaPrivateKeyDatas(int batchSize, WhereDelegate<RsaPrivateKeyDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Private.RsaPrivateKeyData>> batchProcessor)
        {
            await Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Private.RsaPrivateKeyData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllRsaPrivateKeyDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Private.RsaPrivateKeyData>> batchProcessor)
        {
            await Bam.Protocol.Data.Private.Dao.RsaPrivateKeyData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Private.RsaPrivateKeyData>(batch));
            }, Database);
        }


	}
}																								
