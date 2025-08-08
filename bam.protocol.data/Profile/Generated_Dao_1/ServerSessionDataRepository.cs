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
using Bam.Protocol.Data.Profile;

namespace Bam.Protocol.Data.Profile.Dao.Repository
{
	[Serializable]
	public partial class ServerSessionDataRepository: DaoRepository
	{
		public ServerSessionDataRepository()
		{
			SchemaName = "ServerSessionData";
			BaseNamespace = "Bam.Protocol.Data.Profile";

			
			AddType<Bam.Protocol.Data.Profile.ActorData>();
			
			
			AddType<DeviceData>();
			
			
			AddType<Bam.Protocol.Data.Profile.OrganizationData>();
			
			
			AddType<Bam.Protocol.Data.Profile.PersonData>();
			
			
			AddType<Bam.Protocol.Data.Private.PrivateKeySetData>();
			
			
			AddType<Bam.Protocol.Data.Profile.ProfileData>();
			
			
			AddType<Bam.Protocol.Data.Profile.PublicKeySetData>();
			

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
		public void SetOneActorWhere(WhereDelegate<ActorColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.Actor.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneActorWhere(WhereDelegate<ActorColumns> where, out Bam.Protocol.Data.Profile.ActorData result)
		{
			Bam.Protocol.Data.Profile.Dao.Actor.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.Actor daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.ActorData>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.ActorData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.ActorData GetOneActorWhere(WhereDelegate<ActorColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.ActorData>();
			var data = (Bam.Protocol.Data.Profile.ActorData)Bam.Protocol.Data.Profile.Dao.Actor.GetOneWhere(where, Database)?.CopyAs(wrapperType, this); 
            return new DaoRepoData<Bam.Protocol.Data.Profile.ActorData>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single Actor instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ActorColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ActorColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.ActorData OneActorWhere(WhereDelegate<ActorColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.ActorData>();
            var data = (Bam.Protocol.Data.Profile.ActorData)Bam.Protocol.Data.Profile.Dao.Actor.OneWhere(where, Database)?.CopyAs(wrapperType, this);
            return new DaoRepoData<Bam.Protocol.Data.Profile.ActorData>(data, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.ActorColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.ActorColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.ActorData> ActorsWhere(WhereDelegate<ActorColumns> where, OrderBy<ActorColumns> orderBy = null)
        {
            return Wrap<Bam.Protocol.Data.Profile.ActorData>(Bam.Protocol.Data.Profile.Dao.Actor.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a ActorColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ActorColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.ActorData> TopActorsWhere(int count, WhereDelegate<ActorColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.ActorData>(Bam.Protocol.Data.Profile.Dao.Actor.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.ActorData> TopActorsWhere(int count, WhereDelegate<ActorColumns> where, OrderBy<ActorColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.ActorData>(Bam.Protocol.Data.Profile.Dao.Actor.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of Actors
		/// </summary>
		public long CountActors()
        {
            return Bam.Protocol.Data.Profile.Dao.Actor.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ActorColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ActorColumns and other values
		/// </param>
        public long CountActorsWhere(WhereDelegate<ActorColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.Actor.Count(where, Database);
        }
        
        /*public async Task BatchQueryActors(int batchSize, WhereDelegate<ActorColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.Actor>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.Actor.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.Actor>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllActors(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.ActorData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.Actor.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.ActorData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneDeviceWhere(WhereDelegate<DeviceColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.Device.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneDeviceWhere(WhereDelegate<DeviceColumns> where, out DeviceData result)
		{
			Bam.Protocol.Data.Profile.Dao.Device.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.Device daoResult, Database);
			var data = daoResult.CopyAs<DeviceData>();
            result = new DaoRepoData<DeviceData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public DeviceData GetOneDeviceWhere(WhereDelegate<DeviceColumns> where)
		{
			Type wrapperType = GetWrapperType<DeviceData>();
			var data = (DeviceData)Bam.Protocol.Data.Profile.Dao.Device.GetOneWhere(where, Database)?.CopyAs(wrapperType, this); 
            return new DaoRepoData<DeviceData>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single Device instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a DeviceColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceColumns and other values
		/// </param>
		public DeviceData OneDeviceWhere(WhereDelegate<DeviceColumns> where)
        {
            Type wrapperType = GetWrapperType<DeviceData>();
            var data = (DeviceData)Bam.Protocol.Data.Profile.Dao.Device.OneWhere(where, Database)?.CopyAs(wrapperType, this);
            return new DaoRepoData<DeviceData>(data, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.DeviceColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.DeviceColumns and other values
		/// </param>
		public IEnumerable<DeviceData> DevicesWhere(WhereDelegate<DeviceColumns> where, OrderBy<DeviceColumns> orderBy = null)
        {
            return Wrap<DeviceData>(Bam.Protocol.Data.Profile.Dao.Device.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a DeviceColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceColumns and other values
		/// </param>
		public IEnumerable<DeviceData> TopDevicesWhere(int count, WhereDelegate<DeviceColumns> where)
        {
            return Wrap<DeviceData>(Bam.Protocol.Data.Profile.Dao.Device.Top(count, where, Database));
        }

        public IEnumerable<DeviceData> TopDevicesWhere(int count, WhereDelegate<DeviceColumns> where, OrderBy<DeviceColumns> orderBy)
        {
            return Wrap<DeviceData>(Bam.Protocol.Data.Profile.Dao.Device.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of Devices
		/// </summary>
		public long CountDevices()
        {
            return Bam.Protocol.Data.Profile.Dao.Device.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a DeviceColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceColumns and other values
		/// </param>
        public long CountDevicesWhere(WhereDelegate<DeviceColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.Device.Count(where, Database);
        }
        
        /*public async Task BatchQueryDevices(int batchSize, WhereDelegate<DeviceColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.Device>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.Device.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.Device>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllDevices(int batchSize, Action<IEnumerable<DeviceData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.Device.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<DeviceData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneOrganizationWhere(WhereDelegate<OrganizationColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.Organization.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneOrganizationWhere(WhereDelegate<OrganizationColumns> where, out Bam.Protocol.Data.Profile.OrganizationData result)
		{
			Bam.Protocol.Data.Profile.Dao.Organization.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.Organization daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.OrganizationData>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.OrganizationData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.OrganizationData GetOneOrganizationWhere(WhereDelegate<OrganizationColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.OrganizationData>();
			var data = (Bam.Protocol.Data.Profile.OrganizationData)Bam.Protocol.Data.Profile.Dao.Organization.GetOneWhere(where, Database)?.CopyAs(wrapperType, this); 
            return new DaoRepoData<Bam.Protocol.Data.Profile.OrganizationData>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single Organization instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a OrganizationColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.OrganizationData OneOrganizationWhere(WhereDelegate<OrganizationColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.OrganizationData>();
            var data = (Bam.Protocol.Data.Profile.OrganizationData)Bam.Protocol.Data.Profile.Dao.Organization.OneWhere(where, Database)?.CopyAs(wrapperType, this);
            return new DaoRepoData<Bam.Protocol.Data.Profile.OrganizationData>(data, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.OrganizationColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.OrganizationColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.OrganizationData> OrganizationsWhere(WhereDelegate<OrganizationColumns> where, OrderBy<OrganizationColumns> orderBy = null)
        {
            return Wrap<Bam.Protocol.Data.Profile.OrganizationData>(Bam.Protocol.Data.Profile.Dao.Organization.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a OrganizationColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.OrganizationData> TopOrganizationsWhere(int count, WhereDelegate<OrganizationColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.OrganizationData>(Bam.Protocol.Data.Profile.Dao.Organization.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.OrganizationData> TopOrganizationsWhere(int count, WhereDelegate<OrganizationColumns> where, OrderBy<OrganizationColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.OrganizationData>(Bam.Protocol.Data.Profile.Dao.Organization.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of Organizations
		/// </summary>
		public long CountOrganizations()
        {
            return Bam.Protocol.Data.Profile.Dao.Organization.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a OrganizationColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationColumns and other values
		/// </param>
        public long CountOrganizationsWhere(WhereDelegate<OrganizationColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.Organization.Count(where, Database);
        }
        
        /*public async Task BatchQueryOrganizations(int batchSize, WhereDelegate<OrganizationColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.Organization>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.Organization.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.Organization>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllOrganizations(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.OrganizationData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.Organization.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.OrganizationData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOnePersonWhere(WhereDelegate<PersonColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.Person.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOnePersonWhere(WhereDelegate<PersonColumns> where, out Bam.Protocol.Data.Profile.PersonData result)
		{
			Bam.Protocol.Data.Profile.Dao.Person.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.Person daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.PersonData>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.PersonData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.PersonData GetOnePersonWhere(WhereDelegate<PersonColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.PersonData>();
			var data = (Bam.Protocol.Data.Profile.PersonData)Bam.Protocol.Data.Profile.Dao.Person.GetOneWhere(where, Database)?.CopyAs(wrapperType, this); 
            return new DaoRepoData<Bam.Protocol.Data.Profile.PersonData>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single Person instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a PersonColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.PersonData OnePersonWhere(WhereDelegate<PersonColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.PersonData>();
            var data = (Bam.Protocol.Data.Profile.PersonData)Bam.Protocol.Data.Profile.Dao.Person.OneWhere(where, Database)?.CopyAs(wrapperType, this);
            return new DaoRepoData<Bam.Protocol.Data.Profile.PersonData>(data, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.PersonColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.PersonColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.PersonData> PersonsWhere(WhereDelegate<PersonColumns> where, OrderBy<PersonColumns> orderBy = null)
        {
            return Wrap<Bam.Protocol.Data.Profile.PersonData>(Bam.Protocol.Data.Profile.Dao.Person.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a PersonColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.PersonData> TopPersonsWhere(int count, WhereDelegate<PersonColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.PersonData>(Bam.Protocol.Data.Profile.Dao.Person.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.PersonData> TopPersonsWhere(int count, WhereDelegate<PersonColumns> where, OrderBy<PersonColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.PersonData>(Bam.Protocol.Data.Profile.Dao.Person.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of Persons
		/// </summary>
		public long CountPersons()
        {
            return Bam.Protocol.Data.Profile.Dao.Person.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a PersonColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonColumns and other values
		/// </param>
        public long CountPersonsWhere(WhereDelegate<PersonColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.Person.Count(where, Database);
        }
        
        /*public async Task BatchQueryPersons(int batchSize, WhereDelegate<PersonColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.Person>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.Person.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.Person>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllPersons(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.PersonData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.Person.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.PersonData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOnePrivateKeySetWhere(WhereDelegate<PrivateKeySetColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.PrivateKeySet.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOnePrivateKeySetWhere(WhereDelegate<PrivateKeySetColumns> where, out Bam.Protocol.Data.Private.PrivateKeySetData result)
		{
			Bam.Protocol.Data.Profile.Dao.PrivateKeySet.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.PrivateKeySet daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Private.PrivateKeySetData>();
            result = new DaoRepoData<Bam.Protocol.Data.Private.PrivateKeySetData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Private.PrivateKeySetData GetOnePrivateKeySetWhere(WhereDelegate<PrivateKeySetColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Private.PrivateKeySetData>();
			var data = (Bam.Protocol.Data.Private.PrivateKeySetData)Bam.Protocol.Data.Profile.Dao.PrivateKeySet.GetOneWhere(where, Database)?.CopyAs(wrapperType, this); 
            return new DaoRepoData<Bam.Protocol.Data.Private.PrivateKeySetData>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single PrivateKeySet instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a PrivateKeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PrivateKeySetColumns and other values
		/// </param>
		public Bam.Protocol.Data.Private.PrivateKeySetData OnePrivateKeySetWhere(WhereDelegate<PrivateKeySetColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Private.PrivateKeySetData>();
            var data = (Bam.Protocol.Data.Private.PrivateKeySetData)Bam.Protocol.Data.Profile.Dao.PrivateKeySet.OneWhere(where, Database)?.CopyAs(wrapperType, this);
            return new DaoRepoData<Bam.Protocol.Data.Private.PrivateKeySetData>(data, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.PrivateKeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.PrivateKeySetColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Private.PrivateKeySetData> PrivateKeySetsWhere(WhereDelegate<PrivateKeySetColumns> where, OrderBy<PrivateKeySetColumns> orderBy = null)
        {
            return Wrap<Bam.Protocol.Data.Private.PrivateKeySetData>(Bam.Protocol.Data.Profile.Dao.PrivateKeySet.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a PrivateKeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PrivateKeySetColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Private.PrivateKeySetData> TopPrivateKeySetsWhere(int count, WhereDelegate<PrivateKeySetColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Private.PrivateKeySetData>(Bam.Protocol.Data.Profile.Dao.PrivateKeySet.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Private.PrivateKeySetData> TopPrivateKeySetsWhere(int count, WhereDelegate<PrivateKeySetColumns> where, OrderBy<PrivateKeySetColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Private.PrivateKeySetData>(Bam.Protocol.Data.Profile.Dao.PrivateKeySet.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of PrivateKeySets
		/// </summary>
		public long CountPrivateKeySets()
        {
            return Bam.Protocol.Data.Profile.Dao.PrivateKeySet.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a PrivateKeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PrivateKeySetColumns and other values
		/// </param>
        public long CountPrivateKeySetsWhere(WhereDelegate<PrivateKeySetColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.PrivateKeySet.Count(where, Database);
        }
        
        /*public async Task BatchQueryPrivateKeySets(int batchSize, WhereDelegate<PrivateKeySetColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.PrivateKeySet>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.PrivateKeySet.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.PrivateKeySet>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllPrivateKeySets(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Private.PrivateKeySetData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.PrivateKeySet.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Private.PrivateKeySetData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneProfileWhere(WhereDelegate<ProfileColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.Profile.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneProfileWhere(WhereDelegate<ProfileColumns> where, out Bam.Protocol.Data.Profile.ProfileData result)
		{
			Bam.Protocol.Data.Profile.Dao.Profile.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.Profile daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.ProfileData>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.ProfileData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.ProfileData GetOneProfileWhere(WhereDelegate<ProfileColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.ProfileData>();
			var data = (Bam.Protocol.Data.Profile.ProfileData)Bam.Protocol.Data.Profile.Dao.Profile.GetOneWhere(where, Database)?.CopyAs(wrapperType, this); 
            return new DaoRepoData<Bam.Protocol.Data.Profile.ProfileData>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single Profile instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ProfileColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProfileColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.ProfileData OneProfileWhere(WhereDelegate<ProfileColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.ProfileData>();
            var data = (Bam.Protocol.Data.Profile.ProfileData)Bam.Protocol.Data.Profile.Dao.Profile.OneWhere(where, Database)?.CopyAs(wrapperType, this);
            return new DaoRepoData<Bam.Protocol.Data.Profile.ProfileData>(data, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.ProfileColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.ProfileColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.ProfileData> ProfilesWhere(WhereDelegate<ProfileColumns> where, OrderBy<ProfileColumns> orderBy = null)
        {
            return Wrap<Bam.Protocol.Data.Profile.ProfileData>(Bam.Protocol.Data.Profile.Dao.Profile.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a ProfileColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProfileColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.ProfileData> TopProfilesWhere(int count, WhereDelegate<ProfileColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.ProfileData>(Bam.Protocol.Data.Profile.Dao.Profile.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.ProfileData> TopProfilesWhere(int count, WhereDelegate<ProfileColumns> where, OrderBy<ProfileColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.ProfileData>(Bam.Protocol.Data.Profile.Dao.Profile.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of Profiles
		/// </summary>
		public long CountProfiles()
        {
            return Bam.Protocol.Data.Profile.Dao.Profile.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ProfileColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProfileColumns and other values
		/// </param>
        public long CountProfilesWhere(WhereDelegate<ProfileColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.Profile.Count(where, Database);
        }
        
        /*public async Task BatchQueryProfiles(int batchSize, WhereDelegate<ProfileColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.Profile>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.Profile.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.Profile>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllProfiles(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.ProfileData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.Profile.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.ProfileData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOnePublicKeySetWhere(WhereDelegate<PublicKeySetColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.PublicKeySet.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOnePublicKeySetWhere(WhereDelegate<PublicKeySetColumns> where, out Bam.Protocol.Data.Profile.PublicKeySetData result)
		{
			Bam.Protocol.Data.Profile.Dao.PublicKeySet.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.PublicKeySet daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.PublicKeySetData>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.PublicKeySetData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.PublicKeySetData GetOnePublicKeySetWhere(WhereDelegate<PublicKeySetColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.PublicKeySetData>();
			var data = (Bam.Protocol.Data.Profile.PublicKeySetData)Bam.Protocol.Data.Profile.Dao.PublicKeySet.GetOneWhere(where, Database)?.CopyAs(wrapperType, this); 
            return new DaoRepoData<Bam.Protocol.Data.Profile.PublicKeySetData>(data, this); 
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single PublicKeySet instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a PublicKeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PublicKeySetColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.PublicKeySetData OnePublicKeySetWhere(WhereDelegate<PublicKeySetColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.PublicKeySetData>();
            var data = (Bam.Protocol.Data.Profile.PublicKeySetData)Bam.Protocol.Data.Profile.Dao.PublicKeySet.OneWhere(where, Database)?.CopyAs(wrapperType, this);
            return new DaoRepoData<Bam.Protocol.Data.Profile.PublicKeySetData>(data, this);           
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.PublicKeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.PublicKeySetColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.PublicKeySetData> PublicKeySetsWhere(WhereDelegate<PublicKeySetColumns> where, OrderBy<PublicKeySetColumns> orderBy = null)
        {
            return Wrap<Bam.Protocol.Data.Profile.PublicKeySetData>(Bam.Protocol.Data.Profile.Dao.PublicKeySet.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a PublicKeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PublicKeySetColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.PublicKeySetData> TopPublicKeySetsWhere(int count, WhereDelegate<PublicKeySetColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.PublicKeySetData>(Bam.Protocol.Data.Profile.Dao.PublicKeySet.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.PublicKeySetData> TopPublicKeySetsWhere(int count, WhereDelegate<PublicKeySetColumns> where, OrderBy<PublicKeySetColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.PublicKeySetData>(Bam.Protocol.Data.Profile.Dao.PublicKeySet.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of PublicKeySets
		/// </summary>
		public long CountPublicKeySets()
        {
            return Bam.Protocol.Data.Profile.Dao.PublicKeySet.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a PublicKeySetColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PublicKeySetColumns and other values
		/// </param>
        public long CountPublicKeySetsWhere(WhereDelegate<PublicKeySetColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.PublicKeySet.Count(where, Database);
        }
        
        /*public async Task BatchQueryPublicKeySets(int batchSize, WhereDelegate<PublicKeySetColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.PublicKeySet>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.PublicKeySet.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.PublicKeySet>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllPublicKeySets(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.PublicKeySetData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.PublicKeySet.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.PublicKeySetData>(batch));
            }, Database);
        }


	}
}																								
