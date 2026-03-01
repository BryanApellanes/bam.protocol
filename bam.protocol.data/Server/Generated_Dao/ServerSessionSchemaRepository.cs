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
using Bam.Protocol.Data.Server;

namespace Bam.Protocol.Data.Server.Dao.Repository
{
	[Serializable]
	public partial class ServerSessionSchemaRepository: DaoRepository
	{
		public ServerSessionSchemaRepository()
		{
			SchemaName = "ServerSessionSchema";
			BaseNamespace = "Bam.Protocol.Data.Server";

			
			AddType<Bam.Protocol.Data.Server.AccountData>();
			
			
			AddType<Bam.Protocol.Data.Server.InboxData>();
			
			
			AddType<Bam.Protocol.Data.Server.OutboxData>();
			
			
			AddType<Bam.Protocol.Data.Server.ServerAccountData>();
			
			
			AddType<Bam.Protocol.Data.Server.ServerSession>();
			
			
			AddType<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>();
			

			DaoAssembly = typeof(ServerSessionSchemaRepository).Assembly;
		}

		object _addLock = new object();
        public override void AddType(Type type)
        {
            lock (_addLock)
            {
                base.AddType(type);
                DaoAssembly = typeof(ServerSessionSchemaRepository).Assembly;
            }
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneAccountDataWhere(WhereDelegate<AccountDataColumns> where)
		{
			Bam.Protocol.Data.Server.Dao.AccountData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneAccountDataWhere(WhereDelegate<AccountDataColumns> where, out Bam.Protocol.Data.Server.AccountData result)
		{
			Bam.Protocol.Data.Server.Dao.AccountData.SetOneWhere(where, out Bam.Protocol.Data.Server.Dao.AccountData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Server.AccountData>()!;
            result = new DaoRepoData<Bam.Protocol.Data.Server.AccountData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Server.AccountData GetOneAccountDataWhere(WhereDelegate<AccountDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Server.AccountData>();
			var data = (Bam.Protocol.Data.Server.AccountData)Bam.Protocol.Data.Server.Dao.AccountData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null;
            return new DaoRepoData<Bam.Protocol.Data.Server.AccountData>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single AccountData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a AccountDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AccountDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Server.AccountData OneAccountDataWhere(WhereDelegate<AccountDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Server.AccountData>();
            var data = (Bam.Protocol.Data.Server.AccountData)Bam.Protocol.Data.Server.Dao.AccountData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null;
            return new DaoRepoData<Bam.Protocol.Data.Server.AccountData>(data!, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Server.AccountDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Server.AccountDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Server.AccountData> AccountDatasWhere(WhereDelegate<AccountDataColumns> where, OrderBy<AccountDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Server.AccountData>(Bam.Protocol.Data.Server.Dao.AccountData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a AccountDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AccountDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Server.AccountData> TopAccountDatasWhere(int count, WhereDelegate<AccountDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Server.AccountData>(Bam.Protocol.Data.Server.Dao.AccountData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Server.AccountData> TopAccountDatasWhere(int count, WhereDelegate<AccountDataColumns> where, OrderBy<AccountDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Server.AccountData>(Bam.Protocol.Data.Server.Dao.AccountData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of AccountDatas
		/// </summary>
		public long CountAccountDatas()
        {
            return Bam.Protocol.Data.Server.Dao.AccountData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a AccountDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AccountDataColumns and other values
		/// </param>
        public long CountAccountDatasWhere(WhereDelegate<AccountDataColumns> where)
        {
            return Bam.Protocol.Data.Server.Dao.AccountData.Count(where, Database);
        }
        
        /*public async Task BatchQueryAccountDatas(int batchSize, WhereDelegate<AccountDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Server.AccountData>> batchProcessor)
        {
            await Bam.Protocol.Data.Server.Dao.AccountData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Server.AccountData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllAccountDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Server.AccountData>> batchProcessor)
        {
            await Bam.Protocol.Data.Server.Dao.AccountData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Server.AccountData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneInboxDataWhere(WhereDelegate<InboxDataColumns> where)
		{
			Bam.Protocol.Data.Server.Dao.InboxData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneInboxDataWhere(WhereDelegate<InboxDataColumns> where, out Bam.Protocol.Data.Server.InboxData result)
		{
			Bam.Protocol.Data.Server.Dao.InboxData.SetOneWhere(where, out Bam.Protocol.Data.Server.Dao.InboxData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Server.InboxData>()!;
            result = new DaoRepoData<Bam.Protocol.Data.Server.InboxData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Server.InboxData GetOneInboxDataWhere(WhereDelegate<InboxDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Server.InboxData>();
			var data = (Bam.Protocol.Data.Server.InboxData)Bam.Protocol.Data.Server.Dao.InboxData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null;
            return new DaoRepoData<Bam.Protocol.Data.Server.InboxData>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single InboxData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a InboxDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between InboxDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Server.InboxData OneInboxDataWhere(WhereDelegate<InboxDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Server.InboxData>();
            var data = (Bam.Protocol.Data.Server.InboxData)Bam.Protocol.Data.Server.Dao.InboxData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null;
            return new DaoRepoData<Bam.Protocol.Data.Server.InboxData>(data!, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Server.InboxDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Server.InboxDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Server.InboxData> InboxDatasWhere(WhereDelegate<InboxDataColumns> where, OrderBy<InboxDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Server.InboxData>(Bam.Protocol.Data.Server.Dao.InboxData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a InboxDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between InboxDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Server.InboxData> TopInboxDatasWhere(int count, WhereDelegate<InboxDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Server.InboxData>(Bam.Protocol.Data.Server.Dao.InboxData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Server.InboxData> TopInboxDatasWhere(int count, WhereDelegate<InboxDataColumns> where, OrderBy<InboxDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Server.InboxData>(Bam.Protocol.Data.Server.Dao.InboxData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of InboxDatas
		/// </summary>
		public long CountInboxDatas()
        {
            return Bam.Protocol.Data.Server.Dao.InboxData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a InboxDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between InboxDataColumns and other values
		/// </param>
        public long CountInboxDatasWhere(WhereDelegate<InboxDataColumns> where)
        {
            return Bam.Protocol.Data.Server.Dao.InboxData.Count(where, Database);
        }
        
        /*public async Task BatchQueryInboxDatas(int batchSize, WhereDelegate<InboxDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Server.InboxData>> batchProcessor)
        {
            await Bam.Protocol.Data.Server.Dao.InboxData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Server.InboxData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllInboxDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Server.InboxData>> batchProcessor)
        {
            await Bam.Protocol.Data.Server.Dao.InboxData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Server.InboxData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneOutboxDataWhere(WhereDelegate<OutboxDataColumns> where)
		{
			Bam.Protocol.Data.Server.Dao.OutboxData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneOutboxDataWhere(WhereDelegate<OutboxDataColumns> where, out Bam.Protocol.Data.Server.OutboxData result)
		{
			Bam.Protocol.Data.Server.Dao.OutboxData.SetOneWhere(where, out Bam.Protocol.Data.Server.Dao.OutboxData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Server.OutboxData>()!;
            result = new DaoRepoData<Bam.Protocol.Data.Server.OutboxData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Server.OutboxData GetOneOutboxDataWhere(WhereDelegate<OutboxDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Server.OutboxData>();
			var data = (Bam.Protocol.Data.Server.OutboxData)Bam.Protocol.Data.Server.Dao.OutboxData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null;
            return new DaoRepoData<Bam.Protocol.Data.Server.OutboxData>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single OutboxData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a OutboxDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OutboxDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Server.OutboxData OneOutboxDataWhere(WhereDelegate<OutboxDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Server.OutboxData>();
            var data = (Bam.Protocol.Data.Server.OutboxData)Bam.Protocol.Data.Server.Dao.OutboxData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null;
            return new DaoRepoData<Bam.Protocol.Data.Server.OutboxData>(data!, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Server.OutboxDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Server.OutboxDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Server.OutboxData> OutboxDatasWhere(WhereDelegate<OutboxDataColumns> where, OrderBy<OutboxDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Server.OutboxData>(Bam.Protocol.Data.Server.Dao.OutboxData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a OutboxDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OutboxDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Server.OutboxData> TopOutboxDatasWhere(int count, WhereDelegate<OutboxDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Server.OutboxData>(Bam.Protocol.Data.Server.Dao.OutboxData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Server.OutboxData> TopOutboxDatasWhere(int count, WhereDelegate<OutboxDataColumns> where, OrderBy<OutboxDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Server.OutboxData>(Bam.Protocol.Data.Server.Dao.OutboxData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of OutboxDatas
		/// </summary>
		public long CountOutboxDatas()
        {
            return Bam.Protocol.Data.Server.Dao.OutboxData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a OutboxDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OutboxDataColumns and other values
		/// </param>
        public long CountOutboxDatasWhere(WhereDelegate<OutboxDataColumns> where)
        {
            return Bam.Protocol.Data.Server.Dao.OutboxData.Count(where, Database);
        }
        
        /*public async Task BatchQueryOutboxDatas(int batchSize, WhereDelegate<OutboxDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Server.OutboxData>> batchProcessor)
        {
            await Bam.Protocol.Data.Server.Dao.OutboxData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Server.OutboxData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllOutboxDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Server.OutboxData>> batchProcessor)
        {
            await Bam.Protocol.Data.Server.Dao.OutboxData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Server.OutboxData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneServerAccountDataWhere(WhereDelegate<ServerAccountDataColumns> where)
		{
			Bam.Protocol.Data.Server.Dao.ServerAccountData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneServerAccountDataWhere(WhereDelegate<ServerAccountDataColumns> where, out Bam.Protocol.Data.Server.ServerAccountData result)
		{
			Bam.Protocol.Data.Server.Dao.ServerAccountData.SetOneWhere(where, out Bam.Protocol.Data.Server.Dao.ServerAccountData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Server.ServerAccountData>()!;
            result = new DaoRepoData<Bam.Protocol.Data.Server.ServerAccountData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Server.ServerAccountData GetOneServerAccountDataWhere(WhereDelegate<ServerAccountDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Server.ServerAccountData>();
			var data = (Bam.Protocol.Data.Server.ServerAccountData)Bam.Protocol.Data.Server.Dao.ServerAccountData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null;
            return new DaoRepoData<Bam.Protocol.Data.Server.ServerAccountData>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single ServerAccountData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ServerAccountDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ServerAccountDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Server.ServerAccountData OneServerAccountDataWhere(WhereDelegate<ServerAccountDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Server.ServerAccountData>();
            var data = (Bam.Protocol.Data.Server.ServerAccountData)Bam.Protocol.Data.Server.Dao.ServerAccountData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null;
            return new DaoRepoData<Bam.Protocol.Data.Server.ServerAccountData>(data!, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Server.ServerAccountDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Server.ServerAccountDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Server.ServerAccountData> ServerAccountDatasWhere(WhereDelegate<ServerAccountDataColumns> where, OrderBy<ServerAccountDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Server.ServerAccountData>(Bam.Protocol.Data.Server.Dao.ServerAccountData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a ServerAccountDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ServerAccountDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Server.ServerAccountData> TopServerAccountDatasWhere(int count, WhereDelegate<ServerAccountDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Server.ServerAccountData>(Bam.Protocol.Data.Server.Dao.ServerAccountData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Server.ServerAccountData> TopServerAccountDatasWhere(int count, WhereDelegate<ServerAccountDataColumns> where, OrderBy<ServerAccountDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Server.ServerAccountData>(Bam.Protocol.Data.Server.Dao.ServerAccountData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of ServerAccountDatas
		/// </summary>
		public long CountServerAccountDatas()
        {
            return Bam.Protocol.Data.Server.Dao.ServerAccountData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ServerAccountDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ServerAccountDataColumns and other values
		/// </param>
        public long CountServerAccountDatasWhere(WhereDelegate<ServerAccountDataColumns> where)
        {
            return Bam.Protocol.Data.Server.Dao.ServerAccountData.Count(where, Database);
        }
        
        /*public async Task BatchQueryServerAccountDatas(int batchSize, WhereDelegate<ServerAccountDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Server.ServerAccountData>> batchProcessor)
        {
            await Bam.Protocol.Data.Server.Dao.ServerAccountData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Server.ServerAccountData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllServerAccountDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Server.ServerAccountData>> batchProcessor)
        {
            await Bam.Protocol.Data.Server.Dao.ServerAccountData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Server.ServerAccountData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneServerSessionWhere(WhereDelegate<ServerSessionColumns> where)
		{
			Bam.Protocol.Data.Server.Dao.ServerSession.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneServerSessionWhere(WhereDelegate<ServerSessionColumns> where, out Bam.Protocol.Data.Server.ServerSession result)
		{
			Bam.Protocol.Data.Server.Dao.ServerSession.SetOneWhere(where, out Bam.Protocol.Data.Server.Dao.ServerSession daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Server.ServerSession>()!;
            result = new DaoRepoData<Bam.Protocol.Data.Server.ServerSession>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Server.ServerSession GetOneServerSessionWhere(WhereDelegate<ServerSessionColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Server.ServerSession>();
			var data = (Bam.Protocol.Data.Server.ServerSession)Bam.Protocol.Data.Server.Dao.ServerSession.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null;
            return new DaoRepoData<Bam.Protocol.Data.Server.ServerSession>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single ServerSession instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ServerSessionColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ServerSessionColumns and other values
		/// </param>
		public Bam.Protocol.Data.Server.ServerSession OneServerSessionWhere(WhereDelegate<ServerSessionColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Server.ServerSession>();
            var data = (Bam.Protocol.Data.Server.ServerSession)Bam.Protocol.Data.Server.Dao.ServerSession.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null;
            return new DaoRepoData<Bam.Protocol.Data.Server.ServerSession>(data!, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Server.ServerSessionColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Server.ServerSessionColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Server.ServerSession> ServerSessionsWhere(WhereDelegate<ServerSessionColumns> where, OrderBy<ServerSessionColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Server.ServerSession>(Bam.Protocol.Data.Server.Dao.ServerSession.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a ServerSessionColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ServerSessionColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Server.ServerSession> TopServerSessionsWhere(int count, WhereDelegate<ServerSessionColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Server.ServerSession>(Bam.Protocol.Data.Server.Dao.ServerSession.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Server.ServerSession> TopServerSessionsWhere(int count, WhereDelegate<ServerSessionColumns> where, OrderBy<ServerSessionColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Server.ServerSession>(Bam.Protocol.Data.Server.Dao.ServerSession.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of ServerSessions
		/// </summary>
		public long CountServerSessions()
        {
            return Bam.Protocol.Data.Server.Dao.ServerSession.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ServerSessionColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ServerSessionColumns and other values
		/// </param>
        public long CountServerSessionsWhere(WhereDelegate<ServerSessionColumns> where)
        {
            return Bam.Protocol.Data.Server.Dao.ServerSession.Count(where, Database);
        }
        
        /*public async Task BatchQueryServerSessions(int batchSize, WhereDelegate<ServerSessionColumns> where, Action<IEnumerable<Bam.Protocol.Data.Server.ServerSession>> batchProcessor)
        {
            await Bam.Protocol.Data.Server.Dao.ServerSession.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Server.ServerSession>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllServerSessions(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Server.ServerSession>> batchProcessor)
        {
            await Bam.Protocol.Data.Server.Dao.ServerSession.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Server.ServerSession>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneServerSessionKeyValuePairWhere(WhereDelegate<ServerSessionKeyValuePairColumns> where)
		{
			Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneServerSessionKeyValuePairWhere(WhereDelegate<ServerSessionKeyValuePairColumns> where, out Bam.Protocol.Data.Server.ServerSessionKeyValuePair result)
		{
			Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.SetOneWhere(where, out Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>()!;
            result = new DaoRepoData<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Server.ServerSessionKeyValuePair GetOneServerSessionKeyValuePairWhere(WhereDelegate<ServerSessionKeyValuePairColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>();
			var data = (Bam.Protocol.Data.Server.ServerSessionKeyValuePair)Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null;
            return new DaoRepoData<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single ServerSessionKeyValuePair instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ServerSessionKeyValuePairColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ServerSessionKeyValuePairColumns and other values
		/// </param>
		public Bam.Protocol.Data.Server.ServerSessionKeyValuePair OneServerSessionKeyValuePairWhere(WhereDelegate<ServerSessionKeyValuePairColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>();
            var data = (Bam.Protocol.Data.Server.ServerSessionKeyValuePair)Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null;
            return new DaoRepoData<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>(data!, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Server.ServerSessionKeyValuePairColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Server.ServerSessionKeyValuePairColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Server.ServerSessionKeyValuePair> ServerSessionKeyValuePairsWhere(WhereDelegate<ServerSessionKeyValuePairColumns> where, OrderBy<ServerSessionKeyValuePairColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>(Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a ServerSessionKeyValuePairColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ServerSessionKeyValuePairColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Server.ServerSessionKeyValuePair> TopServerSessionKeyValuePairsWhere(int count, WhereDelegate<ServerSessionKeyValuePairColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>(Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Server.ServerSessionKeyValuePair> TopServerSessionKeyValuePairsWhere(int count, WhereDelegate<ServerSessionKeyValuePairColumns> where, OrderBy<ServerSessionKeyValuePairColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>(Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of ServerSessionKeyValuePairs
		/// </summary>
		public long CountServerSessionKeyValuePairs()
        {
            return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ServerSessionKeyValuePairColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ServerSessionKeyValuePairColumns and other values
		/// </param>
        public long CountServerSessionKeyValuePairsWhere(WhereDelegate<ServerSessionKeyValuePairColumns> where)
        {
            return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.Count(where, Database);
        }
        
        /*public async Task BatchQueryServerSessionKeyValuePairs(int batchSize, WhereDelegate<ServerSessionKeyValuePairColumns> where, Action<IEnumerable<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>> batchProcessor)
        {
            await Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllServerSessionKeyValuePairs(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>> batchProcessor)
        {
            await Bam.Protocol.Data.Server.Dao.ServerSessionKeyValuePair.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>(batch));
            }, Database);
        }


	}
}																								
