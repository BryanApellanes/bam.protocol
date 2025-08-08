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
using Bam.Protocol.Data.Client;

namespace Bam.Protocol.Data.Client.Dao.Repository
{
	[Serializable]
	public partial class ClientSessionDataRepository: DaoRepository
	{
		public ClientSessionDataRepository()
		{
			SchemaName = "ClientSessionData";
			BaseNamespace = "Bam.Protocol.Data.Client";

			
			AddType<Bam.Protocol.Data.Client.ClientKeySetData>();
			
			
			AddType<Bam.Protocol.Data.Client.ClientSessionKeyValue>();
			

			DaoAssembly = typeof(ClientSessionDataRepository).Assembly;
		}

		object _addLock = new object();
        public override void AddType(Type type)
        {
            lock (_addLock)
            {
                base.AddType(type);
                DaoAssembly = typeof(ClientSessionDataRepository).Assembly;
            }
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneClientKeySetDataWhere(WhereDelegate<ClientKeySetDataColumns> where)
		{
			Bam.Protocol.Data.Client.Dao.ClientKeySetData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneClientKeySetDataWhere(WhereDelegate<ClientKeySetDataColumns> where, out Bam.Protocol.Data.Client.ClientKeySetData result)
		{
			Bam.Protocol.Data.Client.Dao.ClientKeySetData.SetOneWhere(where, out Bam.Protocol.Data.Client.Dao.ClientKeySetData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Client.ClientKeySetData>();
            result = new DaoRepoData<Bam.Protocol.Data.Client.ClientKeySetData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Client.ClientKeySetData GetOneClientKeySetDataWhere(WhereDelegate<ClientKeySetDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Client.ClientKeySetData>();
			var data = (Bam.Protocol.Data.Client.ClientKeySetData)Bam.Protocol.Data.Client.Dao.ClientKeySetData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this); 
            return new DaoRepoData<Bam.Protocol.Data.Client.ClientKeySetData>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single ClientKeySetData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ClientKeySetDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ClientKeySetDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Client.ClientKeySetData OneClientKeySetDataWhere(WhereDelegate<ClientKeySetDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Client.ClientKeySetData>();
            var data = (Bam.Protocol.Data.Client.ClientKeySetData)Bam.Protocol.Data.Client.Dao.ClientKeySetData.OneWhere(where, Database)?.CopyAs(wrapperType, this);
            return new DaoRepoData<Bam.Protocol.Data.Client.ClientKeySetData>(data, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Client.ClientKeySetDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Client.ClientKeySetDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Client.ClientKeySetData> ClientKeySetDatasWhere(WhereDelegate<ClientKeySetDataColumns> where, OrderBy<ClientKeySetDataColumns> orderBy = null)
        {
            return Wrap<Bam.Protocol.Data.Client.ClientKeySetData>(Bam.Protocol.Data.Client.Dao.ClientKeySetData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a ClientKeySetDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ClientKeySetDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Client.ClientKeySetData> TopClientKeySetDatasWhere(int count, WhereDelegate<ClientKeySetDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Client.ClientKeySetData>(Bam.Protocol.Data.Client.Dao.ClientKeySetData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Client.ClientKeySetData> TopClientKeySetDatasWhere(int count, WhereDelegate<ClientKeySetDataColumns> where, OrderBy<ClientKeySetDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Client.ClientKeySetData>(Bam.Protocol.Data.Client.Dao.ClientKeySetData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of ClientKeySetDatas
		/// </summary>
		public long CountClientKeySetDatas()
        {
            return Bam.Protocol.Data.Client.Dao.ClientKeySetData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ClientKeySetDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ClientKeySetDataColumns and other values
		/// </param>
        public long CountClientKeySetDatasWhere(WhereDelegate<ClientKeySetDataColumns> where)
        {
            return Bam.Protocol.Data.Client.Dao.ClientKeySetData.Count(where, Database);
        }
        
        /*public async Task BatchQueryClientKeySetDatas(int batchSize, WhereDelegate<ClientKeySetDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Client.ClientKeySetData>> batchProcessor)
        {
            await Bam.Protocol.Data.Client.Dao.ClientKeySetData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Client.ClientKeySetData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllClientKeySetDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Client.ClientKeySetData>> batchProcessor)
        {
            await Bam.Protocol.Data.Client.Dao.ClientKeySetData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Client.ClientKeySetData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneClientSessionKeyValueWhere(WhereDelegate<ClientSessionKeyValueColumns> where)
		{
			Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneClientSessionKeyValueWhere(WhereDelegate<ClientSessionKeyValueColumns> where, out Bam.Protocol.Data.Client.ClientSessionKeyValue result)
		{
			Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.SetOneWhere(where, out Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Client.ClientSessionKeyValue>();
            result = new DaoRepoData<Bam.Protocol.Data.Client.ClientSessionKeyValue>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Client.ClientSessionKeyValue GetOneClientSessionKeyValueWhere(WhereDelegate<ClientSessionKeyValueColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Client.ClientSessionKeyValue>();
			var data = (Bam.Protocol.Data.Client.ClientSessionKeyValue)Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.GetOneWhere(where, Database)?.CopyAs(wrapperType, this); 
            return new DaoRepoData<Bam.Protocol.Data.Client.ClientSessionKeyValue>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single ClientSessionKeyValue instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ClientSessionKeyValueColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ClientSessionKeyValueColumns and other values
		/// </param>
		public Bam.Protocol.Data.Client.ClientSessionKeyValue OneClientSessionKeyValueWhere(WhereDelegate<ClientSessionKeyValueColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Client.ClientSessionKeyValue>();
            var data = (Bam.Protocol.Data.Client.ClientSessionKeyValue)Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.OneWhere(where, Database)?.CopyAs(wrapperType, this);
            return new DaoRepoData<Bam.Protocol.Data.Client.ClientSessionKeyValue>(data, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Client.ClientSessionKeyValueColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Client.ClientSessionKeyValueColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Client.ClientSessionKeyValue> ClientSessionKeyValuesWhere(WhereDelegate<ClientSessionKeyValueColumns> where, OrderBy<ClientSessionKeyValueColumns> orderBy = null)
        {
            return Wrap<Bam.Protocol.Data.Client.ClientSessionKeyValue>(Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a ClientSessionKeyValueColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ClientSessionKeyValueColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Client.ClientSessionKeyValue> TopClientSessionKeyValuesWhere(int count, WhereDelegate<ClientSessionKeyValueColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Client.ClientSessionKeyValue>(Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Client.ClientSessionKeyValue> TopClientSessionKeyValuesWhere(int count, WhereDelegate<ClientSessionKeyValueColumns> where, OrderBy<ClientSessionKeyValueColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Client.ClientSessionKeyValue>(Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of ClientSessionKeyValues
		/// </summary>
		public long CountClientSessionKeyValues()
        {
            return Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ClientSessionKeyValueColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ClientSessionKeyValueColumns and other values
		/// </param>
        public long CountClientSessionKeyValuesWhere(WhereDelegate<ClientSessionKeyValueColumns> where)
        {
            return Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.Count(where, Database);
        }
        
        /*public async Task BatchQueryClientSessionKeyValues(int batchSize, WhereDelegate<ClientSessionKeyValueColumns> where, Action<IEnumerable<Bam.Protocol.Data.Client.ClientSessionKeyValue>> batchProcessor)
        {
            await Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Client.ClientSessionKeyValue>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllClientSessionKeyValues(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Client.ClientSessionKeyValue>> batchProcessor)
        {
            await Bam.Protocol.Data.Client.Dao.ClientSessionKeyValue.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Client.ClientSessionKeyValue>(batch));
            }, Database);
        }


	}
}																								
