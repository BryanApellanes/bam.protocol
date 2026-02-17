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
	public partial class ProfileSchemaRepository: DaoInheritanceRepository
	{
		public ProfileSchemaRepository()
		{
			SchemaName = "ProfileSchema";
			BaseNamespace = "Bam.Protocol.Data.Profile";

			
			AddType<Bam.Protocol.Data.Profile.AdditionalProperty>();
			
			
			AddType<Bam.Protocol.Data.Profile.AgentAdditionalProperties>();
			
			
			AddType<Bam.Protocol.Data.Profile.AgentCertificateData>();
			
			
			AddType<Bam.Protocol.Data.Profile.CertificateData>();
			
			
			AddType<Bam.Protocol.Data.Profile.DeviceAdditionalProperties>();
			
			
			AddType<Bam.Protocol.Data.Profile.DeviceCertificateData>();
			
			
			AddType<Bam.Protocol.Data.Profile.GroupAdditionalProperties>();
			
			
			AddType<Bam.Protocol.Data.Profile.GroupData>();
			
			
			AddType<Bam.Protocol.Data.Profile.MailingAddressData>();
			
			
			AddType<Bam.Protocol.Data.Profile.OrganizationAdditionalProperties>();
			
			
			AddType<Bam.Protocol.Data.Profile.OrganizationCertificateData>();
			
			
			AddType<Bam.Protocol.Data.Profile.OrganizationData>();
			
			
			AddType<Bam.Protocol.Data.Profile.OrganizationMailingAddress>();
			
			
			AddType<Bam.Protocol.Data.Profile.PersonAdditionalProperties>();
			
			
			AddType<Bam.Protocol.Data.Profile.PersonCertificateData>();
			
			
			AddType<Bam.Protocol.Data.Profile.PersonData>();
			
			
			AddType<Bam.Protocol.Data.Profile.PersonMailingAddressData>();
			
			
			AddType<Bam.Protocol.Data.Profile.ProfileAdditionalProperties>();
			
			
			AddType<Bam.Protocol.Data.Profile.ProfileData>();
			
			
			AddType<Bam.Protocol.Data.Profile.PublicKeySetData>();
			

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
		public void SetOneAdditionalPropertyWhere(WhereDelegate<AdditionalPropertyColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.AdditionalProperty.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneAdditionalPropertyWhere(WhereDelegate<AdditionalPropertyColumns> where, out Bam.Protocol.Data.Profile.AdditionalProperty result)
		{
			Bam.Protocol.Data.Profile.Dao.AdditionalProperty.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.AdditionalProperty daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.AdditionalProperty>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.AdditionalProperty>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.AdditionalProperty GetOneAdditionalPropertyWhere(WhereDelegate<AdditionalPropertyColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.AdditionalProperty>();
			var data = (Bam.Protocol.Data.Profile.AdditionalProperty)Bam.Protocol.Data.Profile.Dao.AdditionalProperty.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.AdditionalProperty>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single AdditionalProperty instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a AdditionalPropertyColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AdditionalPropertyColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.AdditionalProperty OneAdditionalPropertyWhere(WhereDelegate<AdditionalPropertyColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.AdditionalProperty>();
            var data = (Bam.Protocol.Data.Profile.AdditionalProperty)Bam.Protocol.Data.Profile.Dao.AdditionalProperty.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.AdditionalProperty>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.AdditionalPropertyColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.AdditionalPropertyColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.AdditionalProperty> AdditionalPropertiesWhere(WhereDelegate<AdditionalPropertyColumns> where, OrderBy<AdditionalPropertyColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.AdditionalProperty>(Bam.Protocol.Data.Profile.Dao.AdditionalProperty.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a AdditionalPropertyColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AdditionalPropertyColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.AdditionalProperty> TopAdditionalPropertiesWhere(int count, WhereDelegate<AdditionalPropertyColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.AdditionalProperty>(Bam.Protocol.Data.Profile.Dao.AdditionalProperty.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.AdditionalProperty> TopAdditionalPropertiesWhere(int count, WhereDelegate<AdditionalPropertyColumns> where, OrderBy<AdditionalPropertyColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.AdditionalProperty>(Bam.Protocol.Data.Profile.Dao.AdditionalProperty.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of AdditionalProperties
		/// </summary>
		public long CountAdditionalProperties()
        {
            return Bam.Protocol.Data.Profile.Dao.AdditionalProperty.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a AdditionalPropertyColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AdditionalPropertyColumns and other values
		/// </param>
        public long CountAdditionalPropertiesWhere(WhereDelegate<AdditionalPropertyColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.AdditionalProperty.Count(where, Database);
        }
        
        /*public async Task BatchQueryAdditionalProperties(int batchSize, WhereDelegate<AdditionalPropertyColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.AdditionalProperty>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.AdditionalProperty.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.AdditionalProperty>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllAdditionalProperties(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.AdditionalProperty>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.AdditionalProperty.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.AdditionalProperty>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneAgentAdditionalPropertiesWhere(WhereDelegate<AgentAdditionalPropertiesColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneAgentAdditionalPropertiesWhere(WhereDelegate<AgentAdditionalPropertiesColumns> where, out Bam.Protocol.Data.Profile.AgentAdditionalProperties result)
		{
			Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.AgentAdditionalProperties>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.AgentAdditionalProperties>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.AgentAdditionalProperties GetOneAgentAdditionalPropertiesWhere(WhereDelegate<AgentAdditionalPropertiesColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.AgentAdditionalProperties>();
			var data = (Bam.Protocol.Data.Profile.AgentAdditionalProperties)Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.AgentAdditionalProperties>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single AgentAdditionalProperties instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a AgentAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AgentAdditionalPropertiesColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.AgentAdditionalProperties OneAgentAdditionalPropertiesWhere(WhereDelegate<AgentAdditionalPropertiesColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.AgentAdditionalProperties>();
            var data = (Bam.Protocol.Data.Profile.AgentAdditionalProperties)Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.AgentAdditionalProperties>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.AgentAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.AgentAdditionalPropertiesColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.AgentAdditionalProperties> AgentAdditionalPropertiesWhere(WhereDelegate<AgentAdditionalPropertiesColumns> where, OrderBy<AgentAdditionalPropertiesColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.AgentAdditionalProperties>(Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a AgentAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AgentAdditionalPropertiesColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.AgentAdditionalProperties> TopAgentAdditionalPropertiesWhere(int count, WhereDelegate<AgentAdditionalPropertiesColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.AgentAdditionalProperties>(Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.AgentAdditionalProperties> TopAgentAdditionalPropertiesWhere(int count, WhereDelegate<AgentAdditionalPropertiesColumns> where, OrderBy<AgentAdditionalPropertiesColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.AgentAdditionalProperties>(Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of AgentAdditionalProperties
		/// </summary>
		public long CountAgentAdditionalProperties()
        {
            return Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a AgentAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AgentAdditionalPropertiesColumns and other values
		/// </param>
        public long CountAgentAdditionalPropertiesWhere(WhereDelegate<AgentAdditionalPropertiesColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.Count(where, Database);
        }
        
        /*public async Task BatchQueryAgentAdditionalProperties(int batchSize, WhereDelegate<AgentAdditionalPropertiesColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.AgentAdditionalProperties>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.AgentAdditionalProperties>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllAgentAdditionalProperties(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.AgentAdditionalProperties>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.AgentAdditionalProperties>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneAgentCertificateDataWhere(WhereDelegate<AgentCertificateDataColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.AgentCertificateData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneAgentCertificateDataWhere(WhereDelegate<AgentCertificateDataColumns> where, out Bam.Protocol.Data.Profile.AgentCertificateData result)
		{
			Bam.Protocol.Data.Profile.Dao.AgentCertificateData.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.AgentCertificateData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.AgentCertificateData>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.AgentCertificateData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.AgentCertificateData GetOneAgentCertificateDataWhere(WhereDelegate<AgentCertificateDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.AgentCertificateData>();
			var data = (Bam.Protocol.Data.Profile.AgentCertificateData)Bam.Protocol.Data.Profile.Dao.AgentCertificateData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.AgentCertificateData>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single AgentCertificateData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a AgentCertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AgentCertificateDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.AgentCertificateData OneAgentCertificateDataWhere(WhereDelegate<AgentCertificateDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.AgentCertificateData>();
            var data = (Bam.Protocol.Data.Profile.AgentCertificateData)Bam.Protocol.Data.Profile.Dao.AgentCertificateData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.AgentCertificateData>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.AgentCertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.AgentCertificateDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.AgentCertificateData> AgentCertificateDatasWhere(WhereDelegate<AgentCertificateDataColumns> where, OrderBy<AgentCertificateDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.AgentCertificateData>(Bam.Protocol.Data.Profile.Dao.AgentCertificateData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a AgentCertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AgentCertificateDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.AgentCertificateData> TopAgentCertificateDatasWhere(int count, WhereDelegate<AgentCertificateDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.AgentCertificateData>(Bam.Protocol.Data.Profile.Dao.AgentCertificateData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.AgentCertificateData> TopAgentCertificateDatasWhere(int count, WhereDelegate<AgentCertificateDataColumns> where, OrderBy<AgentCertificateDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.AgentCertificateData>(Bam.Protocol.Data.Profile.Dao.AgentCertificateData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of AgentCertificateDatas
		/// </summary>
		public long CountAgentCertificateDatas()
        {
            return Bam.Protocol.Data.Profile.Dao.AgentCertificateData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a AgentCertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between AgentCertificateDataColumns and other values
		/// </param>
        public long CountAgentCertificateDatasWhere(WhereDelegate<AgentCertificateDataColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.AgentCertificateData.Count(where, Database);
        }
        
        /*public async Task BatchQueryAgentCertificateDatas(int batchSize, WhereDelegate<AgentCertificateDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.AgentCertificateData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.AgentCertificateData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.AgentCertificateData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllAgentCertificateDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.AgentCertificateData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.AgentCertificateData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.AgentCertificateData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneCertificateDataWhere(WhereDelegate<CertificateDataColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.CertificateData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneCertificateDataWhere(WhereDelegate<CertificateDataColumns> where, out Bam.Protocol.Data.Profile.CertificateData result)
		{
			Bam.Protocol.Data.Profile.Dao.CertificateData.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.CertificateData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.CertificateData>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.CertificateData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.CertificateData GetOneCertificateDataWhere(WhereDelegate<CertificateDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.CertificateData>();
			var data = (Bam.Protocol.Data.Profile.CertificateData)Bam.Protocol.Data.Profile.Dao.CertificateData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.CertificateData>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single CertificateData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a CertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between CertificateDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.CertificateData OneCertificateDataWhere(WhereDelegate<CertificateDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.CertificateData>();
            var data = (Bam.Protocol.Data.Profile.CertificateData)Bam.Protocol.Data.Profile.Dao.CertificateData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.CertificateData>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.CertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.CertificateDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.CertificateData> CertificateDatasWhere(WhereDelegate<CertificateDataColumns> where, OrderBy<CertificateDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.CertificateData>(Bam.Protocol.Data.Profile.Dao.CertificateData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a CertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between CertificateDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.CertificateData> TopCertificateDatasWhere(int count, WhereDelegate<CertificateDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.CertificateData>(Bam.Protocol.Data.Profile.Dao.CertificateData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.CertificateData> TopCertificateDatasWhere(int count, WhereDelegate<CertificateDataColumns> where, OrderBy<CertificateDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.CertificateData>(Bam.Protocol.Data.Profile.Dao.CertificateData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of CertificateDatas
		/// </summary>
		public long CountCertificateDatas()
        {
            return Bam.Protocol.Data.Profile.Dao.CertificateData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a CertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between CertificateDataColumns and other values
		/// </param>
        public long CountCertificateDatasWhere(WhereDelegate<CertificateDataColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.CertificateData.Count(where, Database);
        }
        
        /*public async Task BatchQueryCertificateDatas(int batchSize, WhereDelegate<CertificateDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.CertificateData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.CertificateData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.CertificateData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllCertificateDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.CertificateData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.CertificateData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.CertificateData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneDeviceAdditionalPropertiesWhere(WhereDelegate<DeviceAdditionalPropertiesColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneDeviceAdditionalPropertiesWhere(WhereDelegate<DeviceAdditionalPropertiesColumns> where, out Bam.Protocol.Data.Profile.DeviceAdditionalProperties result)
		{
			Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.DeviceAdditionalProperties>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.DeviceAdditionalProperties>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.DeviceAdditionalProperties GetOneDeviceAdditionalPropertiesWhere(WhereDelegate<DeviceAdditionalPropertiesColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.DeviceAdditionalProperties>();
			var data = (Bam.Protocol.Data.Profile.DeviceAdditionalProperties)Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.DeviceAdditionalProperties>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single DeviceAdditionalProperties instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a DeviceAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceAdditionalPropertiesColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.DeviceAdditionalProperties OneDeviceAdditionalPropertiesWhere(WhereDelegate<DeviceAdditionalPropertiesColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.DeviceAdditionalProperties>();
            var data = (Bam.Protocol.Data.Profile.DeviceAdditionalProperties)Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.DeviceAdditionalProperties>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.DeviceAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.DeviceAdditionalPropertiesColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.DeviceAdditionalProperties> DeviceAdditionalPropertiesWhere(WhereDelegate<DeviceAdditionalPropertiesColumns> where, OrderBy<DeviceAdditionalPropertiesColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.DeviceAdditionalProperties>(Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a DeviceAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceAdditionalPropertiesColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.DeviceAdditionalProperties> TopDeviceAdditionalPropertiesWhere(int count, WhereDelegate<DeviceAdditionalPropertiesColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.DeviceAdditionalProperties>(Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.DeviceAdditionalProperties> TopDeviceAdditionalPropertiesWhere(int count, WhereDelegate<DeviceAdditionalPropertiesColumns> where, OrderBy<DeviceAdditionalPropertiesColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.DeviceAdditionalProperties>(Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of DeviceAdditionalProperties
		/// </summary>
		public long CountDeviceAdditionalProperties()
        {
            return Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a DeviceAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceAdditionalPropertiesColumns and other values
		/// </param>
        public long CountDeviceAdditionalPropertiesWhere(WhereDelegate<DeviceAdditionalPropertiesColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.Count(where, Database);
        }
        
        /*public async Task BatchQueryDeviceAdditionalProperties(int batchSize, WhereDelegate<DeviceAdditionalPropertiesColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.DeviceAdditionalProperties>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.DeviceAdditionalProperties>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllDeviceAdditionalProperties(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.DeviceAdditionalProperties>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.DeviceAdditionalProperties>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneDeviceCertificateDataWhere(WhereDelegate<DeviceCertificateDataColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneDeviceCertificateDataWhere(WhereDelegate<DeviceCertificateDataColumns> where, out Bam.Protocol.Data.Profile.DeviceCertificateData result)
		{
			Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.DeviceCertificateData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.DeviceCertificateData>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.DeviceCertificateData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.DeviceCertificateData GetOneDeviceCertificateDataWhere(WhereDelegate<DeviceCertificateDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.DeviceCertificateData>();
			var data = (Bam.Protocol.Data.Profile.DeviceCertificateData)Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.DeviceCertificateData>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single DeviceCertificateData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a DeviceCertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceCertificateDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.DeviceCertificateData OneDeviceCertificateDataWhere(WhereDelegate<DeviceCertificateDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.DeviceCertificateData>();
            var data = (Bam.Protocol.Data.Profile.DeviceCertificateData)Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.DeviceCertificateData>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.DeviceCertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.DeviceCertificateDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.DeviceCertificateData> DeviceCertificateDatasWhere(WhereDelegate<DeviceCertificateDataColumns> where, OrderBy<DeviceCertificateDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.DeviceCertificateData>(Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a DeviceCertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceCertificateDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.DeviceCertificateData> TopDeviceCertificateDatasWhere(int count, WhereDelegate<DeviceCertificateDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.DeviceCertificateData>(Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.DeviceCertificateData> TopDeviceCertificateDatasWhere(int count, WhereDelegate<DeviceCertificateDataColumns> where, OrderBy<DeviceCertificateDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.DeviceCertificateData>(Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of DeviceCertificateDatas
		/// </summary>
		public long CountDeviceCertificateDatas()
        {
            return Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a DeviceCertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between DeviceCertificateDataColumns and other values
		/// </param>
        public long CountDeviceCertificateDatasWhere(WhereDelegate<DeviceCertificateDataColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.Count(where, Database);
        }
        
        /*public async Task BatchQueryDeviceCertificateDatas(int batchSize, WhereDelegate<DeviceCertificateDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.DeviceCertificateData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.DeviceCertificateData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllDeviceCertificateDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.DeviceCertificateData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.DeviceCertificateData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneGroupAdditionalPropertiesWhere(WhereDelegate<GroupAdditionalPropertiesColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneGroupAdditionalPropertiesWhere(WhereDelegate<GroupAdditionalPropertiesColumns> where, out Bam.Protocol.Data.Profile.GroupAdditionalProperties result)
		{
			Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.GroupAdditionalProperties>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.GroupAdditionalProperties>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.GroupAdditionalProperties GetOneGroupAdditionalPropertiesWhere(WhereDelegate<GroupAdditionalPropertiesColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.GroupAdditionalProperties>();
			var data = (Bam.Protocol.Data.Profile.GroupAdditionalProperties)Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.GroupAdditionalProperties>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single GroupAdditionalProperties instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a GroupAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between GroupAdditionalPropertiesColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.GroupAdditionalProperties OneGroupAdditionalPropertiesWhere(WhereDelegate<GroupAdditionalPropertiesColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.GroupAdditionalProperties>();
            var data = (Bam.Protocol.Data.Profile.GroupAdditionalProperties)Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.GroupAdditionalProperties>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.GroupAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.GroupAdditionalPropertiesColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.GroupAdditionalProperties> GroupAdditionalPropertiesWhere(WhereDelegate<GroupAdditionalPropertiesColumns> where, OrderBy<GroupAdditionalPropertiesColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.GroupAdditionalProperties>(Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a GroupAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between GroupAdditionalPropertiesColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.GroupAdditionalProperties> TopGroupAdditionalPropertiesWhere(int count, WhereDelegate<GroupAdditionalPropertiesColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.GroupAdditionalProperties>(Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.GroupAdditionalProperties> TopGroupAdditionalPropertiesWhere(int count, WhereDelegate<GroupAdditionalPropertiesColumns> where, OrderBy<GroupAdditionalPropertiesColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.GroupAdditionalProperties>(Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of GroupAdditionalProperties
		/// </summary>
		public long CountGroupAdditionalProperties()
        {
            return Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a GroupAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between GroupAdditionalPropertiesColumns and other values
		/// </param>
        public long CountGroupAdditionalPropertiesWhere(WhereDelegate<GroupAdditionalPropertiesColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.Count(where, Database);
        }
        
        /*public async Task BatchQueryGroupAdditionalProperties(int batchSize, WhereDelegate<GroupAdditionalPropertiesColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.GroupAdditionalProperties>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.GroupAdditionalProperties>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllGroupAdditionalProperties(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.GroupAdditionalProperties>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.GroupAdditionalProperties>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneGroupDataWhere(WhereDelegate<GroupDataColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.GroupData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneGroupDataWhere(WhereDelegate<GroupDataColumns> where, out Bam.Protocol.Data.Profile.GroupData result)
		{
			Bam.Protocol.Data.Profile.Dao.GroupData.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.GroupData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.GroupData>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.GroupData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.GroupData GetOneGroupDataWhere(WhereDelegate<GroupDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.GroupData>();
			var data = (Bam.Protocol.Data.Profile.GroupData)Bam.Protocol.Data.Profile.Dao.GroupData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.GroupData>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single GroupData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a GroupDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between GroupDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.GroupData OneGroupDataWhere(WhereDelegate<GroupDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.GroupData>();
            var data = (Bam.Protocol.Data.Profile.GroupData)Bam.Protocol.Data.Profile.Dao.GroupData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.GroupData>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.GroupDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.GroupDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.GroupData> GroupDatasWhere(WhereDelegate<GroupDataColumns> where, OrderBy<GroupDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.GroupData>(Bam.Protocol.Data.Profile.Dao.GroupData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a GroupDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between GroupDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.GroupData> TopGroupDatasWhere(int count, WhereDelegate<GroupDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.GroupData>(Bam.Protocol.Data.Profile.Dao.GroupData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.GroupData> TopGroupDatasWhere(int count, WhereDelegate<GroupDataColumns> where, OrderBy<GroupDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.GroupData>(Bam.Protocol.Data.Profile.Dao.GroupData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of GroupDatas
		/// </summary>
		public long CountGroupDatas()
        {
            return Bam.Protocol.Data.Profile.Dao.GroupData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a GroupDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between GroupDataColumns and other values
		/// </param>
        public long CountGroupDatasWhere(WhereDelegate<GroupDataColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.GroupData.Count(where, Database);
        }
        
        /*public async Task BatchQueryGroupDatas(int batchSize, WhereDelegate<GroupDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.GroupData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.GroupData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.GroupData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllGroupDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.GroupData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.GroupData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.GroupData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneMailingAddressDataWhere(WhereDelegate<MailingAddressDataColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.MailingAddressData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneMailingAddressDataWhere(WhereDelegate<MailingAddressDataColumns> where, out Bam.Protocol.Data.Profile.MailingAddressData result)
		{
			Bam.Protocol.Data.Profile.Dao.MailingAddressData.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.MailingAddressData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.MailingAddressData>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.MailingAddressData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.MailingAddressData GetOneMailingAddressDataWhere(WhereDelegate<MailingAddressDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.MailingAddressData>();
			var data = (Bam.Protocol.Data.Profile.MailingAddressData)Bam.Protocol.Data.Profile.Dao.MailingAddressData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.MailingAddressData>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single MailingAddressData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a MailingAddressDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between MailingAddressDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.MailingAddressData OneMailingAddressDataWhere(WhereDelegate<MailingAddressDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.MailingAddressData>();
            var data = (Bam.Protocol.Data.Profile.MailingAddressData)Bam.Protocol.Data.Profile.Dao.MailingAddressData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.MailingAddressData>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.MailingAddressDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.MailingAddressDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.MailingAddressData> MailingAddressDatasWhere(WhereDelegate<MailingAddressDataColumns> where, OrderBy<MailingAddressDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.MailingAddressData>(Bam.Protocol.Data.Profile.Dao.MailingAddressData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a MailingAddressDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between MailingAddressDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.MailingAddressData> TopMailingAddressDatasWhere(int count, WhereDelegate<MailingAddressDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.MailingAddressData>(Bam.Protocol.Data.Profile.Dao.MailingAddressData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.MailingAddressData> TopMailingAddressDatasWhere(int count, WhereDelegate<MailingAddressDataColumns> where, OrderBy<MailingAddressDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.MailingAddressData>(Bam.Protocol.Data.Profile.Dao.MailingAddressData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of MailingAddressDatas
		/// </summary>
		public long CountMailingAddressDatas()
        {
            return Bam.Protocol.Data.Profile.Dao.MailingAddressData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a MailingAddressDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between MailingAddressDataColumns and other values
		/// </param>
        public long CountMailingAddressDatasWhere(WhereDelegate<MailingAddressDataColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.MailingAddressData.Count(where, Database);
        }
        
        /*public async Task BatchQueryMailingAddressDatas(int batchSize, WhereDelegate<MailingAddressDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.MailingAddressData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.MailingAddressData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.MailingAddressData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllMailingAddressDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.MailingAddressData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.MailingAddressData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.MailingAddressData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneOrganizationAdditionalPropertiesWhere(WhereDelegate<OrganizationAdditionalPropertiesColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneOrganizationAdditionalPropertiesWhere(WhereDelegate<OrganizationAdditionalPropertiesColumns> where, out Bam.Protocol.Data.Profile.OrganizationAdditionalProperties result)
		{
			Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.OrganizationAdditionalProperties>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.OrganizationAdditionalProperties>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.OrganizationAdditionalProperties GetOneOrganizationAdditionalPropertiesWhere(WhereDelegate<OrganizationAdditionalPropertiesColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.OrganizationAdditionalProperties>();
			var data = (Bam.Protocol.Data.Profile.OrganizationAdditionalProperties)Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.OrganizationAdditionalProperties>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single OrganizationAdditionalProperties instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a OrganizationAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationAdditionalPropertiesColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.OrganizationAdditionalProperties OneOrganizationAdditionalPropertiesWhere(WhereDelegate<OrganizationAdditionalPropertiesColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.OrganizationAdditionalProperties>();
            var data = (Bam.Protocol.Data.Profile.OrganizationAdditionalProperties)Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.OrganizationAdditionalProperties>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.OrganizationAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.OrganizationAdditionalPropertiesColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.OrganizationAdditionalProperties> OrganizationAdditionalPropertiesWhere(WhereDelegate<OrganizationAdditionalPropertiesColumns> where, OrderBy<OrganizationAdditionalPropertiesColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.OrganizationAdditionalProperties>(Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a OrganizationAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationAdditionalPropertiesColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.OrganizationAdditionalProperties> TopOrganizationAdditionalPropertiesWhere(int count, WhereDelegate<OrganizationAdditionalPropertiesColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.OrganizationAdditionalProperties>(Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.OrganizationAdditionalProperties> TopOrganizationAdditionalPropertiesWhere(int count, WhereDelegate<OrganizationAdditionalPropertiesColumns> where, OrderBy<OrganizationAdditionalPropertiesColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.OrganizationAdditionalProperties>(Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of OrganizationAdditionalProperties
		/// </summary>
		public long CountOrganizationAdditionalProperties()
        {
            return Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a OrganizationAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationAdditionalPropertiesColumns and other values
		/// </param>
        public long CountOrganizationAdditionalPropertiesWhere(WhereDelegate<OrganizationAdditionalPropertiesColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.Count(where, Database);
        }
        
        /*public async Task BatchQueryOrganizationAdditionalProperties(int batchSize, WhereDelegate<OrganizationAdditionalPropertiesColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.OrganizationAdditionalProperties>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.OrganizationAdditionalProperties>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllOrganizationAdditionalProperties(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.OrganizationAdditionalProperties>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.OrganizationAdditionalProperties>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneOrganizationCertificateDataWhere(WhereDelegate<OrganizationCertificateDataColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneOrganizationCertificateDataWhere(WhereDelegate<OrganizationCertificateDataColumns> where, out Bam.Protocol.Data.Profile.OrganizationCertificateData result)
		{
			Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.OrganizationCertificateData>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.OrganizationCertificateData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.OrganizationCertificateData GetOneOrganizationCertificateDataWhere(WhereDelegate<OrganizationCertificateDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.OrganizationCertificateData>();
			var data = (Bam.Protocol.Data.Profile.OrganizationCertificateData)Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.OrganizationCertificateData>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single OrganizationCertificateData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a OrganizationCertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationCertificateDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.OrganizationCertificateData OneOrganizationCertificateDataWhere(WhereDelegate<OrganizationCertificateDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.OrganizationCertificateData>();
            var data = (Bam.Protocol.Data.Profile.OrganizationCertificateData)Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.OrganizationCertificateData>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.OrganizationCertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.OrganizationCertificateDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.OrganizationCertificateData> OrganizationCertificateDatasWhere(WhereDelegate<OrganizationCertificateDataColumns> where, OrderBy<OrganizationCertificateDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.OrganizationCertificateData>(Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a OrganizationCertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationCertificateDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.OrganizationCertificateData> TopOrganizationCertificateDatasWhere(int count, WhereDelegate<OrganizationCertificateDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.OrganizationCertificateData>(Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.OrganizationCertificateData> TopOrganizationCertificateDatasWhere(int count, WhereDelegate<OrganizationCertificateDataColumns> where, OrderBy<OrganizationCertificateDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.OrganizationCertificateData>(Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of OrganizationCertificateDatas
		/// </summary>
		public long CountOrganizationCertificateDatas()
        {
            return Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a OrganizationCertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationCertificateDataColumns and other values
		/// </param>
        public long CountOrganizationCertificateDatasWhere(WhereDelegate<OrganizationCertificateDataColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.Count(where, Database);
        }
        
        /*public async Task BatchQueryOrganizationCertificateDatas(int batchSize, WhereDelegate<OrganizationCertificateDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.OrganizationCertificateData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.OrganizationCertificateData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllOrganizationCertificateDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.OrganizationCertificateData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.OrganizationCertificateData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneOrganizationDataWhere(WhereDelegate<OrganizationDataColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.OrganizationData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneOrganizationDataWhere(WhereDelegate<OrganizationDataColumns> where, out Bam.Protocol.Data.Profile.OrganizationData result)
		{
			Bam.Protocol.Data.Profile.Dao.OrganizationData.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.OrganizationData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.OrganizationData>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.OrganizationData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.OrganizationData GetOneOrganizationDataWhere(WhereDelegate<OrganizationDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.OrganizationData>();
			var data = (Bam.Protocol.Data.Profile.OrganizationData)Bam.Protocol.Data.Profile.Dao.OrganizationData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.OrganizationData>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single OrganizationData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a OrganizationDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.OrganizationData OneOrganizationDataWhere(WhereDelegate<OrganizationDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.OrganizationData>();
            var data = (Bam.Protocol.Data.Profile.OrganizationData)Bam.Protocol.Data.Profile.Dao.OrganizationData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.OrganizationData>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.OrganizationDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.OrganizationDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.OrganizationData> OrganizationDatasWhere(WhereDelegate<OrganizationDataColumns> where, OrderBy<OrganizationDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.OrganizationData>(Bam.Protocol.Data.Profile.Dao.OrganizationData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a OrganizationDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.OrganizationData> TopOrganizationDatasWhere(int count, WhereDelegate<OrganizationDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.OrganizationData>(Bam.Protocol.Data.Profile.Dao.OrganizationData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.OrganizationData> TopOrganizationDatasWhere(int count, WhereDelegate<OrganizationDataColumns> where, OrderBy<OrganizationDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.OrganizationData>(Bam.Protocol.Data.Profile.Dao.OrganizationData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of OrganizationDatas
		/// </summary>
		public long CountOrganizationDatas()
        {
            return Bam.Protocol.Data.Profile.Dao.OrganizationData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a OrganizationDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationDataColumns and other values
		/// </param>
        public long CountOrganizationDatasWhere(WhereDelegate<OrganizationDataColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.OrganizationData.Count(where, Database);
        }
        
        /*public async Task BatchQueryOrganizationDatas(int batchSize, WhereDelegate<OrganizationDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.OrganizationData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.OrganizationData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.OrganizationData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllOrganizationDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.OrganizationData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.OrganizationData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.OrganizationData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneOrganizationMailingAddressWhere(WhereDelegate<OrganizationMailingAddressColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneOrganizationMailingAddressWhere(WhereDelegate<OrganizationMailingAddressColumns> where, out Bam.Protocol.Data.Profile.OrganizationMailingAddress result)
		{
			Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.OrganizationMailingAddress>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.OrganizationMailingAddress>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.OrganizationMailingAddress GetOneOrganizationMailingAddressWhere(WhereDelegate<OrganizationMailingAddressColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.OrganizationMailingAddress>();
			var data = (Bam.Protocol.Data.Profile.OrganizationMailingAddress)Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.OrganizationMailingAddress>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single OrganizationMailingAddress instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a OrganizationMailingAddressColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationMailingAddressColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.OrganizationMailingAddress OneOrganizationMailingAddressWhere(WhereDelegate<OrganizationMailingAddressColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.OrganizationMailingAddress>();
            var data = (Bam.Protocol.Data.Profile.OrganizationMailingAddress)Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.OrganizationMailingAddress>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.OrganizationMailingAddressColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.OrganizationMailingAddressColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.OrganizationMailingAddress> OrganizationMailingAddressesWhere(WhereDelegate<OrganizationMailingAddressColumns> where, OrderBy<OrganizationMailingAddressColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.OrganizationMailingAddress>(Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a OrganizationMailingAddressColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationMailingAddressColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.OrganizationMailingAddress> TopOrganizationMailingAddressesWhere(int count, WhereDelegate<OrganizationMailingAddressColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.OrganizationMailingAddress>(Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.OrganizationMailingAddress> TopOrganizationMailingAddressesWhere(int count, WhereDelegate<OrganizationMailingAddressColumns> where, OrderBy<OrganizationMailingAddressColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.OrganizationMailingAddress>(Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of OrganizationMailingAddresses
		/// </summary>
		public long CountOrganizationMailingAddresses()
        {
            return Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a OrganizationMailingAddressColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between OrganizationMailingAddressColumns and other values
		/// </param>
        public long CountOrganizationMailingAddressesWhere(WhereDelegate<OrganizationMailingAddressColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.Count(where, Database);
        }
        
        /*public async Task BatchQueryOrganizationMailingAddresses(int batchSize, WhereDelegate<OrganizationMailingAddressColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.OrganizationMailingAddress>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.OrganizationMailingAddress>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllOrganizationMailingAddresses(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.OrganizationMailingAddress>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.OrganizationMailingAddress>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOnePersonAdditionalPropertiesWhere(WhereDelegate<PersonAdditionalPropertiesColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOnePersonAdditionalPropertiesWhere(WhereDelegate<PersonAdditionalPropertiesColumns> where, out Bam.Protocol.Data.Profile.PersonAdditionalProperties result)
		{
			Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.PersonAdditionalProperties>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.PersonAdditionalProperties>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.PersonAdditionalProperties GetOnePersonAdditionalPropertiesWhere(WhereDelegate<PersonAdditionalPropertiesColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.PersonAdditionalProperties>();
			var data = (Bam.Protocol.Data.Profile.PersonAdditionalProperties)Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.PersonAdditionalProperties>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single PersonAdditionalProperties instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a PersonAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonAdditionalPropertiesColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.PersonAdditionalProperties OnePersonAdditionalPropertiesWhere(WhereDelegate<PersonAdditionalPropertiesColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.PersonAdditionalProperties>();
            var data = (Bam.Protocol.Data.Profile.PersonAdditionalProperties)Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.PersonAdditionalProperties>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.PersonAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.PersonAdditionalPropertiesColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.PersonAdditionalProperties> PersonAdditionalPropertiesWhere(WhereDelegate<PersonAdditionalPropertiesColumns> where, OrderBy<PersonAdditionalPropertiesColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.PersonAdditionalProperties>(Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a PersonAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonAdditionalPropertiesColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.PersonAdditionalProperties> TopPersonAdditionalPropertiesWhere(int count, WhereDelegate<PersonAdditionalPropertiesColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.PersonAdditionalProperties>(Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.PersonAdditionalProperties> TopPersonAdditionalPropertiesWhere(int count, WhereDelegate<PersonAdditionalPropertiesColumns> where, OrderBy<PersonAdditionalPropertiesColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.PersonAdditionalProperties>(Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of PersonAdditionalProperties
		/// </summary>
		public long CountPersonAdditionalProperties()
        {
            return Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a PersonAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonAdditionalPropertiesColumns and other values
		/// </param>
        public long CountPersonAdditionalPropertiesWhere(WhereDelegate<PersonAdditionalPropertiesColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.Count(where, Database);
        }
        
        /*public async Task BatchQueryPersonAdditionalProperties(int batchSize, WhereDelegate<PersonAdditionalPropertiesColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.PersonAdditionalProperties>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.PersonAdditionalProperties>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllPersonAdditionalProperties(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.PersonAdditionalProperties>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.PersonAdditionalProperties>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOnePersonCertificateDataWhere(WhereDelegate<PersonCertificateDataColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.PersonCertificateData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOnePersonCertificateDataWhere(WhereDelegate<PersonCertificateDataColumns> where, out Bam.Protocol.Data.Profile.PersonCertificateData result)
		{
			Bam.Protocol.Data.Profile.Dao.PersonCertificateData.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.PersonCertificateData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.PersonCertificateData>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.PersonCertificateData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.PersonCertificateData GetOnePersonCertificateDataWhere(WhereDelegate<PersonCertificateDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.PersonCertificateData>();
			var data = (Bam.Protocol.Data.Profile.PersonCertificateData)Bam.Protocol.Data.Profile.Dao.PersonCertificateData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.PersonCertificateData>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single PersonCertificateData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a PersonCertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonCertificateDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.PersonCertificateData OnePersonCertificateDataWhere(WhereDelegate<PersonCertificateDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.PersonCertificateData>();
            var data = (Bam.Protocol.Data.Profile.PersonCertificateData)Bam.Protocol.Data.Profile.Dao.PersonCertificateData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.PersonCertificateData>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.PersonCertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.PersonCertificateDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.PersonCertificateData> PersonCertificateDatasWhere(WhereDelegate<PersonCertificateDataColumns> where, OrderBy<PersonCertificateDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.PersonCertificateData>(Bam.Protocol.Data.Profile.Dao.PersonCertificateData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a PersonCertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonCertificateDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.PersonCertificateData> TopPersonCertificateDatasWhere(int count, WhereDelegate<PersonCertificateDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.PersonCertificateData>(Bam.Protocol.Data.Profile.Dao.PersonCertificateData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.PersonCertificateData> TopPersonCertificateDatasWhere(int count, WhereDelegate<PersonCertificateDataColumns> where, OrderBy<PersonCertificateDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.PersonCertificateData>(Bam.Protocol.Data.Profile.Dao.PersonCertificateData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of PersonCertificateDatas
		/// </summary>
		public long CountPersonCertificateDatas()
        {
            return Bam.Protocol.Data.Profile.Dao.PersonCertificateData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a PersonCertificateDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonCertificateDataColumns and other values
		/// </param>
        public long CountPersonCertificateDatasWhere(WhereDelegate<PersonCertificateDataColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.PersonCertificateData.Count(where, Database);
        }
        
        /*public async Task BatchQueryPersonCertificateDatas(int batchSize, WhereDelegate<PersonCertificateDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.PersonCertificateData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.PersonCertificateData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.PersonCertificateData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllPersonCertificateDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.PersonCertificateData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.PersonCertificateData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.PersonCertificateData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOnePersonDataWhere(WhereDelegate<PersonDataColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.PersonData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOnePersonDataWhere(WhereDelegate<PersonDataColumns> where, out Bam.Protocol.Data.Profile.PersonData result)
		{
			Bam.Protocol.Data.Profile.Dao.PersonData.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.PersonData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.PersonData>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.PersonData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.PersonData GetOnePersonDataWhere(WhereDelegate<PersonDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.PersonData>();
			var data = (Bam.Protocol.Data.Profile.PersonData)Bam.Protocol.Data.Profile.Dao.PersonData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.PersonData>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single PersonData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a PersonDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.PersonData OnePersonDataWhere(WhereDelegate<PersonDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.PersonData>();
            var data = (Bam.Protocol.Data.Profile.PersonData)Bam.Protocol.Data.Profile.Dao.PersonData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.PersonData>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.PersonDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.PersonDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.PersonData> PersonDatasWhere(WhereDelegate<PersonDataColumns> where, OrderBy<PersonDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.PersonData>(Bam.Protocol.Data.Profile.Dao.PersonData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a PersonDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.PersonData> TopPersonDatasWhere(int count, WhereDelegate<PersonDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.PersonData>(Bam.Protocol.Data.Profile.Dao.PersonData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.PersonData> TopPersonDatasWhere(int count, WhereDelegate<PersonDataColumns> where, OrderBy<PersonDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.PersonData>(Bam.Protocol.Data.Profile.Dao.PersonData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of PersonDatas
		/// </summary>
		public long CountPersonDatas()
        {
            return Bam.Protocol.Data.Profile.Dao.PersonData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a PersonDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonDataColumns and other values
		/// </param>
        public long CountPersonDatasWhere(WhereDelegate<PersonDataColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.PersonData.Count(where, Database);
        }
        
        /*public async Task BatchQueryPersonDatas(int batchSize, WhereDelegate<PersonDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.PersonData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.PersonData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.PersonData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllPersonDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.PersonData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.PersonData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.PersonData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOnePersonMailingAddressDataWhere(WhereDelegate<PersonMailingAddressDataColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOnePersonMailingAddressDataWhere(WhereDelegate<PersonMailingAddressDataColumns> where, out Bam.Protocol.Data.Profile.PersonMailingAddressData result)
		{
			Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.PersonMailingAddressData>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.PersonMailingAddressData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.PersonMailingAddressData GetOnePersonMailingAddressDataWhere(WhereDelegate<PersonMailingAddressDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.PersonMailingAddressData>();
			var data = (Bam.Protocol.Data.Profile.PersonMailingAddressData)Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.PersonMailingAddressData>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single PersonMailingAddressData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a PersonMailingAddressDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonMailingAddressDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.PersonMailingAddressData OnePersonMailingAddressDataWhere(WhereDelegate<PersonMailingAddressDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.PersonMailingAddressData>();
            var data = (Bam.Protocol.Data.Profile.PersonMailingAddressData)Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.PersonMailingAddressData>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.PersonMailingAddressDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.PersonMailingAddressDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.PersonMailingAddressData> PersonMailingAddressDatasWhere(WhereDelegate<PersonMailingAddressDataColumns> where, OrderBy<PersonMailingAddressDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.PersonMailingAddressData>(Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a PersonMailingAddressDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonMailingAddressDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.PersonMailingAddressData> TopPersonMailingAddressDatasWhere(int count, WhereDelegate<PersonMailingAddressDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.PersonMailingAddressData>(Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.PersonMailingAddressData> TopPersonMailingAddressDatasWhere(int count, WhereDelegate<PersonMailingAddressDataColumns> where, OrderBy<PersonMailingAddressDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.PersonMailingAddressData>(Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of PersonMailingAddressDatas
		/// </summary>
		public long CountPersonMailingAddressDatas()
        {
            return Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a PersonMailingAddressDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PersonMailingAddressDataColumns and other values
		/// </param>
        public long CountPersonMailingAddressDatasWhere(WhereDelegate<PersonMailingAddressDataColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.Count(where, Database);
        }
        
        /*public async Task BatchQueryPersonMailingAddressDatas(int batchSize, WhereDelegate<PersonMailingAddressDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.PersonMailingAddressData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.PersonMailingAddressData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllPersonMailingAddressDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.PersonMailingAddressData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.PersonMailingAddressData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneProfileAdditionalPropertiesWhere(WhereDelegate<ProfileAdditionalPropertiesColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneProfileAdditionalPropertiesWhere(WhereDelegate<ProfileAdditionalPropertiesColumns> where, out Bam.Protocol.Data.Profile.ProfileAdditionalProperties result)
		{
			Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.ProfileAdditionalProperties>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.ProfileAdditionalProperties>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.ProfileAdditionalProperties GetOneProfileAdditionalPropertiesWhere(WhereDelegate<ProfileAdditionalPropertiesColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.ProfileAdditionalProperties>();
			var data = (Bam.Protocol.Data.Profile.ProfileAdditionalProperties)Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.ProfileAdditionalProperties>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single ProfileAdditionalProperties instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ProfileAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProfileAdditionalPropertiesColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.ProfileAdditionalProperties OneProfileAdditionalPropertiesWhere(WhereDelegate<ProfileAdditionalPropertiesColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.ProfileAdditionalProperties>();
            var data = (Bam.Protocol.Data.Profile.ProfileAdditionalProperties)Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.ProfileAdditionalProperties>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.ProfileAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.ProfileAdditionalPropertiesColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.ProfileAdditionalProperties> ProfileAdditionalPropertiesWhere(WhereDelegate<ProfileAdditionalPropertiesColumns> where, OrderBy<ProfileAdditionalPropertiesColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.ProfileAdditionalProperties>(Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a ProfileAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProfileAdditionalPropertiesColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.ProfileAdditionalProperties> TopProfileAdditionalPropertiesWhere(int count, WhereDelegate<ProfileAdditionalPropertiesColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.ProfileAdditionalProperties>(Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.ProfileAdditionalProperties> TopProfileAdditionalPropertiesWhere(int count, WhereDelegate<ProfileAdditionalPropertiesColumns> where, OrderBy<ProfileAdditionalPropertiesColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.ProfileAdditionalProperties>(Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of ProfileAdditionalProperties
		/// </summary>
		public long CountProfileAdditionalProperties()
        {
            return Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ProfileAdditionalPropertiesColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProfileAdditionalPropertiesColumns and other values
		/// </param>
        public long CountProfileAdditionalPropertiesWhere(WhereDelegate<ProfileAdditionalPropertiesColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.Count(where, Database);
        }
        
        /*public async Task BatchQueryProfileAdditionalProperties(int batchSize, WhereDelegate<ProfileAdditionalPropertiesColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.ProfileAdditionalProperties>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.ProfileAdditionalProperties>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllProfileAdditionalProperties(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.ProfileAdditionalProperties>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.ProfileAdditionalProperties>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneProfileDataWhere(WhereDelegate<ProfileDataColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.ProfileData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOneProfileDataWhere(WhereDelegate<ProfileDataColumns> where, out Bam.Protocol.Data.Profile.ProfileData result)
		{
			Bam.Protocol.Data.Profile.Dao.ProfileData.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.ProfileData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.ProfileData>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.ProfileData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.ProfileData GetOneProfileDataWhere(WhereDelegate<ProfileDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.ProfileData>();
			var data = (Bam.Protocol.Data.Profile.ProfileData)Bam.Protocol.Data.Profile.Dao.ProfileData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.ProfileData>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single ProfileData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ProfileDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProfileDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.ProfileData OneProfileDataWhere(WhereDelegate<ProfileDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.ProfileData>();
            var data = (Bam.Protocol.Data.Profile.ProfileData)Bam.Protocol.Data.Profile.Dao.ProfileData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.ProfileData>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.ProfileDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.ProfileDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.ProfileData> ProfileDatasWhere(WhereDelegate<ProfileDataColumns> where, OrderBy<ProfileDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.ProfileData>(Bam.Protocol.Data.Profile.Dao.ProfileData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a ProfileDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProfileDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.ProfileData> TopProfileDatasWhere(int count, WhereDelegate<ProfileDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.ProfileData>(Bam.Protocol.Data.Profile.Dao.ProfileData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.ProfileData> TopProfileDatasWhere(int count, WhereDelegate<ProfileDataColumns> where, OrderBy<ProfileDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.ProfileData>(Bam.Protocol.Data.Profile.Dao.ProfileData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of ProfileDatas
		/// </summary>
		public long CountProfileDatas()
        {
            return Bam.Protocol.Data.Profile.Dao.ProfileData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a ProfileDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between ProfileDataColumns and other values
		/// </param>
        public long CountProfileDatasWhere(WhereDelegate<ProfileDataColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.ProfileData.Count(where, Database);
        }
        
        /*public async Task BatchQueryProfileDatas(int batchSize, WhereDelegate<ProfileDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.ProfileData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.ProfileData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.ProfileData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllProfileDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.ProfileData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.ProfileData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.ProfileData>(batch));
            }, Database);
        }

		
		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOnePublicKeySetDataWhere(WhereDelegate<PublicKeySetDataColumns> where)
		{
			Bam.Protocol.Data.Profile.Dao.PublicKeySetData.SetOneWhere(where, Database);
		}

		/// <summary>
		/// Set one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		public void SetOnePublicKeySetDataWhere(WhereDelegate<PublicKeySetDataColumns> where, out Bam.Protocol.Data.Profile.PublicKeySetData result)
		{
			Bam.Protocol.Data.Profile.Dao.PublicKeySetData.SetOneWhere(where, out Bam.Protocol.Data.Profile.Dao.PublicKeySetData daoResult, Database);
			var data = daoResult.CopyAs<Bam.Protocol.Data.Profile.PublicKeySetData>();
            result = new DaoRepoData<Bam.Protocol.Data.Profile.PublicKeySetData>(data, this);
		}

		/// <summary>
		/// Get one entry matching the specified filter.  If none exists 
		/// one is created; success depends on the nullability
		/// of the specified columns.
		/// </summary>
		/// <param name="where"></param>
		public Bam.Protocol.Data.Profile.PublicKeySetData GetOnePublicKeySetDataWhere(WhereDelegate<PublicKeySetDataColumns> where)
		{
			Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.PublicKeySetData>();
			var data = (Bam.Protocol.Data.Profile.PublicKeySetData)Bam.Protocol.Data.Profile.Dao.PublicKeySetData.GetOneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.PublicKeySetData>(data, this);
        }

		/// <summary>
		/// Execute a query that should return only one result.  If no result is found null is returned.  If more
		/// than one result is returned a MultipleEntriesFoundException is thrown.  This method is most commonly used to retrieve a
		/// single PublicKeySetData instance by its Id/Key value
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a PublicKeySetDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PublicKeySetDataColumns and other values
		/// </param>
		public Bam.Protocol.Data.Profile.PublicKeySetData OnePublicKeySetDataWhere(WhereDelegate<PublicKeySetDataColumns> where)
        {
            Type wrapperType = GetWrapperType<Bam.Protocol.Data.Profile.PublicKeySetData>();
            var data = (Bam.Protocol.Data.Profile.PublicKeySetData)Bam.Protocol.Data.Profile.Dao.PublicKeySetData.OneWhere(where, Database)?.CopyAs(wrapperType, this)!;
            if (data == null) return null!;
            return new DaoRepoData<Bam.Protocol.Data.Profile.PublicKeySetData>(data, this);
        }

		/// <summary>
		/// Execute a query and return the results. 
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a Bam.Protocol.Data.Profile.PublicKeySetDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between Bam.Protocol.Data.Profile.PublicKeySetDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.PublicKeySetData> PublicKeySetDatasWhere(WhereDelegate<PublicKeySetDataColumns> where, OrderBy<PublicKeySetDataColumns> orderBy = null!)
        {
            return Wrap<Bam.Protocol.Data.Profile.PublicKeySetData>(Bam.Protocol.Data.Profile.Dao.PublicKeySetData.Where(where, orderBy, Database));
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
		/// <param name="where">A WhereDelegate that receives a PublicKeySetDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PublicKeySetDataColumns and other values
		/// </param>
		public IEnumerable<Bam.Protocol.Data.Profile.PublicKeySetData> TopPublicKeySetDatasWhere(int count, WhereDelegate<PublicKeySetDataColumns> where)
        {
            return Wrap<Bam.Protocol.Data.Profile.PublicKeySetData>(Bam.Protocol.Data.Profile.Dao.PublicKeySetData.Top(count, where, Database));
        }

        public IEnumerable<Bam.Protocol.Data.Profile.PublicKeySetData> TopPublicKeySetDatasWhere(int count, WhereDelegate<PublicKeySetDataColumns> where, OrderBy<PublicKeySetDataColumns> orderBy)
        {
            return Wrap<Bam.Protocol.Data.Profile.PublicKeySetData>(Bam.Protocol.Data.Profile.Dao.PublicKeySetData.Top(count, where, orderBy, Database));
        }
                                
		/// <summary>
		/// Return the count of PublicKeySetDatas
		/// </summary>
		public long CountPublicKeySetDatas()
        {
            return Bam.Protocol.Data.Profile.Dao.PublicKeySetData.Count(Database);
        }

		/// <summary>
		/// Execute a query and return the number of results
		/// </summary>
		/// <param name="where">A WhereDelegate that receives a PublicKeySetDataColumns 
		/// and returns a IQueryFilter which is the result of any comparisons
		/// between PublicKeySetDataColumns and other values
		/// </param>
        public long CountPublicKeySetDatasWhere(WhereDelegate<PublicKeySetDataColumns> where)
        {
            return Bam.Protocol.Data.Profile.Dao.PublicKeySetData.Count(where, Database);
        }
        
        /*public async Task BatchQueryPublicKeySetDatas(int batchSize, WhereDelegate<PublicKeySetDataColumns> where, Action<IEnumerable<Bam.Protocol.Data.Profile.PublicKeySetData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.PublicKeySetData.BatchQuery(batchSize, where, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.PublicKeySetData>(batch));
            }, Database);
        }*/
		
        public async Task BatchAllPublicKeySetDatas(int batchSize, Action<IEnumerable<Bam.Protocol.Data.Profile.PublicKeySetData>> batchProcessor)
        {
            await Bam.Protocol.Data.Profile.Dao.PublicKeySetData.BatchAll(batchSize, (batch) =>
            {
				batchProcessor(Wrap<Bam.Protocol.Data.Profile.PublicKeySetData>(batch));
            }, Database);
        }


	}
}																								
