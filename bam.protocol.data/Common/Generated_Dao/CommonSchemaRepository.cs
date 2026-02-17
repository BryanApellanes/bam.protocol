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
using Bam.Protocol.Data.Common;

namespace Bam.Protocol.Data.Common.Dao.Repository
{
	[Serializable]
	public partial class CommonSchemaRepository: DaoRepository
	{
		public CommonSchemaRepository()
		{
			SchemaName = "CommonSchema";
			BaseNamespace = "Bam.Protocol.Data.Common";

			
			AddType<Bam.Protocol.Data.Common.ActorData>();
			
			
			AddType<Bam.Protocol.Data.Common.AgentData>();
			
			
			AddType<Bam.Protocol.Data.Common.DeviceData>();
			
			
			AddType<Bam.Protocol.Data.Common.HostAddressData>();
			
			
			AddType<Bam.Protocol.Data.Common.MachineData>();
			
			
			AddType<Bam.Protocol.Data.Common.NicData>();
			
			
			AddType<Bam.Protocol.Data.Common.ProcessDescriptorData>();
			

			DaoAssembly = typeof(CommonSchemaRepository).Assembly;
		}

		object _addLock = new object();
        public override void AddType(Type type)
        {
            lock (_addLock)
            {
                base.AddType(type);
                DaoAssembly = typeof(CommonSchemaRepository).Assembly;
            }
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneActorDataWhere(WhereDelegate<ActorDataColumns> where)
		{
			Bam.Protocol.Data.Common.Dao.ActorData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneActorDataWhere(WhereDelegate<ActorDataColumns> where, out Bam.Protocol.Data.Common.ActorData result)
		{
			Bam.Protocol.Data.Common.Dao.ActorData.SetOneWhere(where, out Bam.Protocol.Data.Common.Dao.ActorData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Common.ActorData>();
            result = new DaoRepoData<Bam.Protocol.Data.Common.ActorData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Common.ActorData GetOneActorDataWhere(WhereDelegate<ActorDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Common.ActorData>();
			var data = (Bam.Protocol.Data.Common.ActorData)Bam.Protocol.Data.Common.Dao.ActorData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Common.ActorData>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single ActorData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ActorDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ActorDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Common.ActorData OneActorDataWhere(WhereDelegate<ActorDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Common.ActorData>();
            var data = (Bam.Protocol.Data.Common.ActorData)Bam.Protocol.Data.Common.Dao.ActorData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Common.ActorData>(data, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Common.ActorDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Common.ActorDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Common.ActorData> ActorDatasWhere(WhereDelegate<ActorDataColumns> where, OrderBy<ActorDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Common.ActorData>(Bam.Protocol.Data.Common.Dao.ActorData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a ActorDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ActorDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Common.ActorData> TopActorDatasWhere(int count, WhereDelegate<ActorDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Common.ActorData>(Bam.Protocol.Data.Common.Dao.ActorData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Common.ActorData> TopActorDatasWhere(int count, WhereDelegate<ActorDataColumns> where, OrderBy<ActorDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Common.ActorData>(Bam.Protocol.Data.Common.Dao.ActorData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of ActorDatas
		/// </summary>
		public long CountActorDatas()
        {
            return Bam.Protocol.Data.Common.Dao.ActorData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ActorDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ActorDataColumns and other values
		/// </param>
        public long CountActorDatasWhere(WhereDelegate<ActorDataColumns> where)
        {
            return Bam.Protocol.Data.Common.Dao.ActorData.Count(where, Database);
        }
        
        /*public async Task BatchQueryActorDatas(int batchSize, WhereDelegate<ActorDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Common.ActorData>> batchProcessor)
        {
            await Bam.Protocol.Data.Common.Dao.ActorData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Common.ActorData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllActorDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Common.ActorData>> batchProcessor)
        {
            await Bam.Protocol.Data.Common.Dao.ActorData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Common.ActorData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneAgentDataWhere(WhereDelegate<AgentDataColumns> where)
		{
			Bam.Protocol.Data.Common.Dao.AgentData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneAgentDataWhere(WhereDelegate<AgentDataColumns> where, out Bam.Protocol.Data.Common.AgentData result)
		{
			Bam.Protocol.Data.Common.Dao.AgentData.SetOneWhere(where, out Bam.Protocol.Data.Common.Dao.AgentData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Common.AgentData>();
            result = new DaoRepoData<Bam.Protocol.Data.Common.AgentData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Common.AgentData GetOneAgentDataWhere(WhereDelegate<AgentDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Common.AgentData>();
			var data = (Bam.Protocol.Data.Common.AgentData)Bam.Protocol.Data.Common.Dao.AgentData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Common.AgentData>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single AgentData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a AgentDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AgentDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Common.AgentData OneAgentDataWhere(WhereDelegate<AgentDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Common.AgentData>();
            var data = (Bam.Protocol.Data.Common.AgentData)Bam.Protocol.Data.Common.Dao.AgentData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Common.AgentData>(data, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Common.AgentDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Common.AgentDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Common.AgentData> AgentDatasWhere(WhereDelegate<AgentDataColumns> where, OrderBy<AgentDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Common.AgentData>(Bam.Protocol.Data.Common.Dao.AgentData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a AgentDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AgentDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Common.AgentData> TopAgentDatasWhere(int count, WhereDelegate<AgentDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Common.AgentData>(Bam.Protocol.Data.Common.Dao.AgentData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Common.AgentData> TopAgentDatasWhere(int count, WhereDelegate<AgentDataColumns> where, OrderBy<AgentDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Common.AgentData>(Bam.Protocol.Data.Common.Dao.AgentData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of AgentDatas
		/// </summary>
		public long CountAgentDatas()
        {
            return Bam.Protocol.Data.Common.Dao.AgentData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a AgentDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AgentDataColumns and other values
		/// </param>
        public long CountAgentDatasWhere(WhereDelegate<AgentDataColumns> where)
        {
            return Bam.Protocol.Data.Common.Dao.AgentData.Count(where, Database);
        }
        
        /*public async Task BatchQueryAgentDatas(int batchSize, WhereDelegate<AgentDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Common.AgentData>> batchProcessor)
        {
            await Bam.Protocol.Data.Common.Dao.AgentData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Common.AgentData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllAgentDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Common.AgentData>> batchProcessor)
        {
            await Bam.Protocol.Data.Common.Dao.AgentData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Common.AgentData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneDeviceDataWhere(WhereDelegate<DeviceDataColumns> where)
		{
			Bam.Protocol.Data.Common.Dao.DeviceData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneDeviceDataWhere(WhereDelegate<DeviceDataColumns> where, out Bam.Protocol.Data.Common.DeviceData result)
		{
			Bam.Protocol.Data.Common.Dao.DeviceData.SetOneWhere(where, out Bam.Protocol.Data.Common.Dao.DeviceData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Common.DeviceData>();
            result = new DaoRepoData<Bam.Protocol.Data.Common.DeviceData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Common.DeviceData GetOneDeviceDataWhere(WhereDelegate<DeviceDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Common.DeviceData>();
			var data = (Bam.Protocol.Data.Common.DeviceData)Bam.Protocol.Data.Common.Dao.DeviceData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Common.DeviceData>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single DeviceData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a DeviceDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Common.DeviceData OneDeviceDataWhere(WhereDelegate<DeviceDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Common.DeviceData>();
            var data = (Bam.Protocol.Data.Common.DeviceData)Bam.Protocol.Data.Common.Dao.DeviceData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Common.DeviceData>(data, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Common.DeviceDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Common.DeviceDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Common.DeviceData> DeviceDatasWhere(WhereDelegate<DeviceDataColumns> where, OrderBy<DeviceDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Common.DeviceData>(Bam.Protocol.Data.Common.Dao.DeviceData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a DeviceDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Common.DeviceData> TopDeviceDatasWhere(int count, WhereDelegate<DeviceDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Common.DeviceData>(Bam.Protocol.Data.Common.Dao.DeviceData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Common.DeviceData> TopDeviceDatasWhere(int count, WhereDelegate<DeviceDataColumns> where, OrderBy<DeviceDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Common.DeviceData>(Bam.Protocol.Data.Common.Dao.DeviceData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of DeviceDatas
		/// </summary>
		public long CountDeviceDatas()
        {
            return Bam.Protocol.Data.Common.Dao.DeviceData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a DeviceDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceDataColumns and other values
		/// </param>
        public long CountDeviceDatasWhere(WhereDelegate<DeviceDataColumns> where)
        {
            return Bam.Protocol.Data.Common.Dao.DeviceData.Count(where, Database);
        }
        
        /*public async Task BatchQueryDeviceDatas(int batchSize, WhereDelegate<DeviceDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Common.DeviceData>> batchProcessor)
        {
            await Bam.Protocol.Data.Common.Dao.DeviceData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Common.DeviceData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllDeviceDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Common.DeviceData>> batchProcessor)
        {
            await Bam.Protocol.Data.Common.Dao.DeviceData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Common.DeviceData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneHostAddressDataWhere(WhereDelegate<HostAddressDataColumns> where)
		{
			Bam.Protocol.Data.Common.Dao.HostAddressData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneHostAddressDataWhere(WhereDelegate<HostAddressDataColumns> where, out Bam.Protocol.Data.Common.HostAddressData result)
		{
			Bam.Protocol.Data.Common.Dao.HostAddressData.SetOneWhere(where, out Bam.Protocol.Data.Common.Dao.HostAddressData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Common.HostAddressData>();
            result = new DaoRepoData<Bam.Protocol.Data.Common.HostAddressData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Common.HostAddressData GetOneHostAddressDataWhere(WhereDelegate<HostAddressDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Common.HostAddressData>();
			var data = (Bam.Protocol.Data.Common.HostAddressData)Bam.Protocol.Data.Common.Dao.HostAddressData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Common.HostAddressData>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single HostAddressData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a HostAddressDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between HostAddressDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Common.HostAddressData OneHostAddressDataWhere(WhereDelegate<HostAddressDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Common.HostAddressData>();
            var data = (Bam.Protocol.Data.Common.HostAddressData)Bam.Protocol.Data.Common.Dao.HostAddressData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Common.HostAddressData>(data, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Common.HostAddressDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Common.HostAddressDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Common.HostAddressData> HostAddressDatasWhere(WhereDelegate<HostAddressDataColumns> where, OrderBy<HostAddressDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Common.HostAddressData>(Bam.Protocol.Data.Common.Dao.HostAddressData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a HostAddressDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between HostAddressDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Common.HostAddressData> TopHostAddressDatasWhere(int count, WhereDelegate<HostAddressDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Common.HostAddressData>(Bam.Protocol.Data.Common.Dao.HostAddressData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Common.HostAddressData> TopHostAddressDatasWhere(int count, WhereDelegate<HostAddressDataColumns> where, OrderBy<HostAddressDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Common.HostAddressData>(Bam.Protocol.Data.Common.Dao.HostAddressData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of HostAddressDatas
		/// </summary>
		public long CountHostAddressDatas()
        {
            return Bam.Protocol.Data.Common.Dao.HostAddressData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a HostAddressDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between HostAddressDataColumns and other values
		/// </param>
        public long CountHostAddressDatasWhere(WhereDelegate<HostAddressDataColumns> where)
        {
            return Bam.Protocol.Data.Common.Dao.HostAddressData.Count(where, Database);
        }
        
        /*public async Task BatchQueryHostAddressDatas(int batchSize, WhereDelegate<HostAddressDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Common.HostAddressData>> batchProcessor)
        {
            await Bam.Protocol.Data.Common.Dao.HostAddressData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Common.HostAddressData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllHostAddressDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Common.HostAddressData>> batchProcessor)
        {
            await Bam.Protocol.Data.Common.Dao.HostAddressData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Common.HostAddressData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneMachineDataWhere(WhereDelegate<MachineDataColumns> where)
		{
			Bam.Protocol.Data.Common.Dao.MachineData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneMachineDataWhere(WhereDelegate<MachineDataColumns> where, out Bam.Protocol.Data.Common.MachineData result)
		{
			Bam.Protocol.Data.Common.Dao.MachineData.SetOneWhere(where, out Bam.Protocol.Data.Common.Dao.MachineData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Common.MachineData>();
            result = new DaoRepoData<Bam.Protocol.Data.Common.MachineData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Common.MachineData GetOneMachineDataWhere(WhereDelegate<MachineDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Common.MachineData>();
			var data = (Bam.Protocol.Data.Common.MachineData)Bam.Protocol.Data.Common.Dao.MachineData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Common.MachineData>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single MachineData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a MachineDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between MachineDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Common.MachineData OneMachineDataWhere(WhereDelegate<MachineDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Common.MachineData>();
            var data = (Bam.Protocol.Data.Common.MachineData)Bam.Protocol.Data.Common.Dao.MachineData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Common.MachineData>(data, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Common.MachineDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Common.MachineDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Common.MachineData> MachineDatasWhere(WhereDelegate<MachineDataColumns> where, OrderBy<MachineDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Common.MachineData>(Bam.Protocol.Data.Common.Dao.MachineData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a MachineDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between MachineDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Common.MachineData> TopMachineDatasWhere(int count, WhereDelegate<MachineDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Common.MachineData>(Bam.Protocol.Data.Common.Dao.MachineData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Common.MachineData> TopMachineDatasWhere(int count, WhereDelegate<MachineDataColumns> where, OrderBy<MachineDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Common.MachineData>(Bam.Protocol.Data.Common.Dao.MachineData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of MachineDatas
		/// </summary>
		public long CountMachineDatas()
        {
            return Bam.Protocol.Data.Common.Dao.MachineData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a MachineDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between MachineDataColumns and other values
		/// </param>
        public long CountMachineDatasWhere(WhereDelegate<MachineDataColumns> where)
        {
            return Bam.Protocol.Data.Common.Dao.MachineData.Count(where, Database);
        }
        
        /*public async Task BatchQueryMachineDatas(int batchSize, WhereDelegate<MachineDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Common.MachineData>> batchProcessor)
        {
            await Bam.Protocol.Data.Common.Dao.MachineData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Common.MachineData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllMachineDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Common.MachineData>> batchProcessor)
        {
            await Bam.Protocol.Data.Common.Dao.MachineData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Common.MachineData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneNicDataWhere(WhereDelegate<NicDataColumns> where)
		{
			Bam.Protocol.Data.Common.Dao.NicData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneNicDataWhere(WhereDelegate<NicDataColumns> where, out Bam.Protocol.Data.Common.NicData result)
		{
			Bam.Protocol.Data.Common.Dao.NicData.SetOneWhere(where, out Bam.Protocol.Data.Common.Dao.NicData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Common.NicData>();
            result = new DaoRepoData<Bam.Protocol.Data.Common.NicData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Common.NicData GetOneNicDataWhere(WhereDelegate<NicDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Common.NicData>();
			var data = (Bam.Protocol.Data.Common.NicData)Bam.Protocol.Data.Common.Dao.NicData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Common.NicData>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single NicData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a NicDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between NicDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Common.NicData OneNicDataWhere(WhereDelegate<NicDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Common.NicData>();
            var data = (Bam.Protocol.Data.Common.NicData)Bam.Protocol.Data.Common.Dao.NicData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Common.NicData>(data, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Common.NicDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Common.NicDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Common.NicData> NicDatasWhere(WhereDelegate<NicDataColumns> where, OrderBy<NicDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Common.NicData>(Bam.Protocol.Data.Common.Dao.NicData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a NicDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between NicDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Common.NicData> TopNicDatasWhere(int count, WhereDelegate<NicDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Common.NicData>(Bam.Protocol.Data.Common.Dao.NicData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Common.NicData> TopNicDatasWhere(int count, WhereDelegate<NicDataColumns> where, OrderBy<NicDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Common.NicData>(Bam.Protocol.Data.Common.Dao.NicData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of NicDatas
		/// </summary>
		public long CountNicDatas()
        {
            return Bam.Protocol.Data.Common.Dao.NicData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a NicDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between NicDataColumns and other values
		/// </param>
        public long CountNicDatasWhere(WhereDelegate<NicDataColumns> where)
        {
            return Bam.Protocol.Data.Common.Dao.NicData.Count(where, Database);
        }
        
        /*public async Task BatchQueryNicDatas(int batchSize, WhereDelegate<NicDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Common.NicData>> batchProcessor)
        {
            await Bam.Protocol.Data.Common.Dao.NicData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Common.NicData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllNicDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Common.NicData>> batchProcessor)
        {
            await Bam.Protocol.Data.Common.Dao.NicData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Common.NicData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneProcessDescriptorDataWhere(WhereDelegate<ProcessDescriptorDataColumns> where)
		{
			Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneProcessDescriptorDataWhere(WhereDelegate<ProcessDescriptorDataColumns> where, out Bam.Protocol.Data.Common.ProcessDescriptorData result)
		{
			Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.SetOneWhere(where, out Bam.Protocol.Data.Common.Dao.ProcessDescriptorData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Common.ProcessDescriptorData>();
            result = new DaoRepoData<Bam.Protocol.Data.Common.ProcessDescriptorData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Common.ProcessDescriptorData GetOneProcessDescriptorDataWhere(WhereDelegate<ProcessDescriptorDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Common.ProcessDescriptorData>();
			var data = (Bam.Protocol.Data.Common.ProcessDescriptorData)Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Common.ProcessDescriptorData>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single ProcessDescriptorData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ProcessDescriptorDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProcessDescriptorDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Common.ProcessDescriptorData OneProcessDescriptorDataWhere(WhereDelegate<ProcessDescriptorDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Common.ProcessDescriptorData>();
            var data = (Bam.Protocol.Data.Common.ProcessDescriptorData)Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            return new DaoRepoData<Bam.Protocol.Data.Common.ProcessDescriptorData>(data, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Common.ProcessDescriptorDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Common.ProcessDescriptorDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Common.ProcessDescriptorData> ProcessDescriptorDatasWhere(WhereDelegate<ProcessDescriptorDataColumns> where, OrderBy<ProcessDescriptorDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Common.ProcessDescriptorData>(Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a ProcessDescriptorDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProcessDescriptorDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Common.ProcessDescriptorData> TopProcessDescriptorDatasWhere(int count, WhereDelegate<ProcessDescriptorDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Common.ProcessDescriptorData>(Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Common.ProcessDescriptorData> TopProcessDescriptorDatasWhere(int count, WhereDelegate<ProcessDescriptorDataColumns> where, OrderBy<ProcessDescriptorDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Common.ProcessDescriptorData>(Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of ProcessDescriptorDatas
		/// </summary>
		public long CountProcessDescriptorDatas()
        {
            return Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ProcessDescriptorDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProcessDescriptorDataColumns and other values
		/// </param>
        public long CountProcessDescriptorDatasWhere(WhereDelegate<ProcessDescriptorDataColumns> where)
        {
            return Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.Count(where, Database);
        }
        
        /*public async Task BatchQueryProcessDescriptorDatas(int batchSize, WhereDelegate<ProcessDescriptorDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Common.ProcessDescriptorData>> batchProcessor)
        {
            await Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Common.ProcessDescriptorData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllProcessDescriptorDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Common.ProcessDescriptorData>> batchProcessor)
        {
            await Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Common.ProcessDescriptorData>(batch));
            }, Database);
        }


	}
}																								
