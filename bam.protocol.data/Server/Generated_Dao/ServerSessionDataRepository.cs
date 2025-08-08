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
	public partial class ServerSessionDataRepository: DaoRepository
	{
		public ServerSessionDataRepository()
		{
			SchemaName = "ServerSessionData";
			BaseNamespace = "Bam.Protocol.Data.Server";

			
			AddType<Bam.Protocol.Data.Server.ServerSession>();
			
			
			AddType<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>();
			

			DaoAssembly = typeof(ServerSessionDataRepository).Assembly;
		}

		object _addLock = new object();
        public override void AddType(Type type)
        {
            lock (_addLock)
            {
                base.AddType(type);
                DaoAssembly = typeof(ServerSessionDataRepository).Assembly;
            }
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
			var data = daoResult.CopyAs<Bam.Protocol.Data.Server.ServerSession>();
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
			var data = (Bam.Protocol.Data.Server.ServerSession)Bam.Protocol.Data.Server.Dao.ServerSession.GetOneWhere(where, Database)?.CopyAs(wrapperType, this); 
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
            var data = (Bam.Protocol.Data.Server.ServerSession)Bam.Protocol.Data.Server.Dao.ServerSession.OneWhere(where, Database)?.CopyAs(wrapperType, this);
            return new DaoRepoData<Bam.Protocol.Data.Server.ServerSession>(data, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Server.ServerSessionColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Server.ServerSessionColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Server.ServerSession> ServerSessionsWhere(WhereDelegate<ServerSessionColumns> where, OrderBy<ServerSessionColumns> orderBy = null)
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
		public void SetOneServerSessionKeyValueWhere(WhereDelegate<ServerSessionKeyValueColumns> where)
		{
			Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneServerSessionKeyValueWhere(WhereDelegate<ServerSessionKeyValueColumns> where, out Bam.Protocol.Data.Server.ServerSessionKeyValuePair result)
		{
			Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.SetOneWhere(where, out Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>();
            result = new DaoRepoData<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Server.ServerSessionKeyValuePair GetOneServerSessionKeyValueWhere(WhereDelegate<ServerSessionKeyValueColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>();
			var data = (Bam.Protocol.Data.Server.ServerSessionKeyValuePair)Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.GetOneWhere(where, Database)?.CopyAs(wrapperType, this); 
            return new DaoRepoData<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single ServerSessionKeyValue instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ServerSessionKeyValueColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ServerSessionKeyValueColumns and other values
		/// </param>
		public Bam.Protocol.Data.Server.ServerSessionKeyValuePair OneServerSessionKeyValueWhere(WhereDelegate<ServerSessionKeyValueColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>();
            var data = (Bam.Protocol.Data.Server.ServerSessionKeyValuePair)Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.OneWhere(where, Database)?.CopyAs(wrapperType, this);
            return new DaoRepoData<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>(data, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Server.ServerSessionKeyValueColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Server.ServerSessionKeyValueColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Server.ServerSessionKeyValuePair> ServerSessionKeyValuesWhere(WhereDelegate<ServerSessionKeyValueColumns> where, OrderBy<ServerSessionKeyValueColumns> orderBy = null)
        {
            return Wrap<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>(Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a ServerSessionKeyValueColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ServerSessionKeyValueColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Server.ServerSessionKeyValuePair> TopServerSessionKeyValuesWhere(int count, WhereDelegate<ServerSessionKeyValueColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>(Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Server.ServerSessionKeyValuePair> TopServerSessionKeyValuesWhere(int count, WhereDelegate<ServerSessionKeyValueColumns> where, OrderBy<ServerSessionKeyValueColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>(Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of ServerSessionKeyValues
		/// </summary>
		public long CountServerSessionKeyValues()
        {
            return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ServerSessionKeyValueColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ServerSessionKeyValueColumns and other values
		/// </param>
        public long CountServerSessionKeyValuesWhere(WhereDelegate<ServerSessionKeyValueColumns> where)
        {
            return Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.Count(where, Database);
        }
        
        /*public async Task BatchQueryServerSessionKeyValues(int batchSize, WhereDelegate<ServerSessionKeyValueColumns> where, Action<IEnumerable<Bam.Protocol.Data.Server.ServerSessionKeyValue>> batchProcessor)
        {
            await Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Server.ServerSessionKeyValue>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllServerSessionKeyValues(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>> batchProcessor)
        {
            await Bam.Protocol.Data.Server.Dao.ServerSessionKeyValue.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Server.ServerSessionKeyValuePair>(batch));
            }, Database);
        }


	}
}																								
