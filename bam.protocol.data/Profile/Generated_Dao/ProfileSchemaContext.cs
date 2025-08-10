/*
	This file was generated and should not be modified directly
*/
// model is SchemaDefinition
using System;
using System.Data;
using System.Data.Common;
using Bam;
using Bam.Data;
using Bam.Data.Qi;

namespace Bam.Protocol.Data.Profile.Dao
{
	// schema = ProfileSchema
    public static class ProfileSchemaContext
    {
		public static string ConnectionName
		{
			get
			{
				return "ProfileSchema";
			}
		}

		public static IDatabase Db
		{
			get
			{
				return Bam.Data.Db.For(ConnectionName);
			}
		}


	public class AdditionalPropertyQueryContext
	{
			public AdditionalPropertyCollection Where(WhereDelegate<AdditionalPropertyColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AdditionalProperty.Where(where, db);
			}
		   
			public AdditionalPropertyCollection Where(WhereDelegate<AdditionalPropertyColumns> where, OrderBy<AdditionalPropertyColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AdditionalProperty.Where(where, orderBy, db);
			}

			public AdditionalProperty OneWhere(WhereDelegate<AdditionalPropertyColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AdditionalProperty.OneWhere(where, db);
			}

			public static AdditionalProperty GetOneWhere(WhereDelegate<AdditionalPropertyColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AdditionalProperty.GetOneWhere(where, db);
			}
		
			public AdditionalProperty FirstOneWhere(WhereDelegate<AdditionalPropertyColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AdditionalProperty.FirstOneWhere(where, db);
			}

			public AdditionalPropertyCollection Top(int count, WhereDelegate<AdditionalPropertyColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AdditionalProperty.Top(count, where, db);
			}

			public AdditionalPropertyCollection Top(int count, WhereDelegate<AdditionalPropertyColumns> where, OrderBy<AdditionalPropertyColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AdditionalProperty.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<AdditionalPropertyColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AdditionalProperty.Count(where, db);
			}
	}

	static AdditionalPropertyQueryContext _additionalProperties;
	static object _additionalPropertiesLock = new object();
	public static AdditionalPropertyQueryContext AdditionalProperties
	{
		get
		{
			return _additionalPropertiesLock.DoubleCheckLock<AdditionalPropertyQueryContext>(ref _additionalProperties, () => new AdditionalPropertyQueryContext());
		}
	}
	public class AgentAdditionalPropertiesQueryContext
	{
			public AgentAdditionalPropertiesCollection Where(WhereDelegate<AgentAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.Where(where, db);
			}
		   
			public AgentAdditionalPropertiesCollection Where(WhereDelegate<AgentAdditionalPropertiesColumns> where, OrderBy<AgentAdditionalPropertiesColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.Where(where, orderBy, db);
			}

			public AgentAdditionalProperties OneWhere(WhereDelegate<AgentAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.OneWhere(where, db);
			}

			public static AgentAdditionalProperties GetOneWhere(WhereDelegate<AgentAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.GetOneWhere(where, db);
			}
		
			public AgentAdditionalProperties FirstOneWhere(WhereDelegate<AgentAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.FirstOneWhere(where, db);
			}

			public AgentAdditionalPropertiesCollection Top(int count, WhereDelegate<AgentAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.Top(count, where, db);
			}

			public AgentAdditionalPropertiesCollection Top(int count, WhereDelegate<AgentAdditionalPropertiesColumns> where, OrderBy<AgentAdditionalPropertiesColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<AgentAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AgentAdditionalProperties.Count(where, db);
			}
	}

	static AgentAdditionalPropertiesQueryContext _agentAdditionalProperties;
	static object _agentAdditionalPropertiesLock = new object();
	public static AgentAdditionalPropertiesQueryContext AgentAdditionalProperties
	{
		get
		{
			return _agentAdditionalPropertiesLock.DoubleCheckLock<AgentAdditionalPropertiesQueryContext>(ref _agentAdditionalProperties, () => new AgentAdditionalPropertiesQueryContext());
		}
	}
	public class AgentCertificateDataQueryContext
	{
			public AgentCertificateDataCollection Where(WhereDelegate<AgentCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AgentCertificateData.Where(where, db);
			}
		   
			public AgentCertificateDataCollection Where(WhereDelegate<AgentCertificateDataColumns> where, OrderBy<AgentCertificateDataColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AgentCertificateData.Where(where, orderBy, db);
			}

			public AgentCertificateData OneWhere(WhereDelegate<AgentCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AgentCertificateData.OneWhere(where, db);
			}

			public static AgentCertificateData GetOneWhere(WhereDelegate<AgentCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AgentCertificateData.GetOneWhere(where, db);
			}
		
			public AgentCertificateData FirstOneWhere(WhereDelegate<AgentCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AgentCertificateData.FirstOneWhere(where, db);
			}

			public AgentCertificateDataCollection Top(int count, WhereDelegate<AgentCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AgentCertificateData.Top(count, where, db);
			}

			public AgentCertificateDataCollection Top(int count, WhereDelegate<AgentCertificateDataColumns> where, OrderBy<AgentCertificateDataColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AgentCertificateData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<AgentCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.AgentCertificateData.Count(where, db);
			}
	}

	static AgentCertificateDataQueryContext _agentCertificateDatas;
	static object _agentCertificateDatasLock = new object();
	public static AgentCertificateDataQueryContext AgentCertificateDatas
	{
		get
		{
			return _agentCertificateDatasLock.DoubleCheckLock<AgentCertificateDataQueryContext>(ref _agentCertificateDatas, () => new AgentCertificateDataQueryContext());
		}
	}
	public class CertificateDataQueryContext
	{
			public CertificateDataCollection Where(WhereDelegate<CertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.CertificateData.Where(where, db);
			}
		   
			public CertificateDataCollection Where(WhereDelegate<CertificateDataColumns> where, OrderBy<CertificateDataColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.CertificateData.Where(where, orderBy, db);
			}

			public CertificateData OneWhere(WhereDelegate<CertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.CertificateData.OneWhere(where, db);
			}

			public static CertificateData GetOneWhere(WhereDelegate<CertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.CertificateData.GetOneWhere(where, db);
			}
		
			public CertificateData FirstOneWhere(WhereDelegate<CertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.CertificateData.FirstOneWhere(where, db);
			}

			public CertificateDataCollection Top(int count, WhereDelegate<CertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.CertificateData.Top(count, where, db);
			}

			public CertificateDataCollection Top(int count, WhereDelegate<CertificateDataColumns> where, OrderBy<CertificateDataColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.CertificateData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<CertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.CertificateData.Count(where, db);
			}
	}

	static CertificateDataQueryContext _certificateDatas;
	static object _certificateDatasLock = new object();
	public static CertificateDataQueryContext CertificateDatas
	{
		get
		{
			return _certificateDatasLock.DoubleCheckLock<CertificateDataQueryContext>(ref _certificateDatas, () => new CertificateDataQueryContext());
		}
	}
	public class DeviceAdditionalPropertiesQueryContext
	{
			public DeviceAdditionalPropertiesCollection Where(WhereDelegate<DeviceAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.Where(where, db);
			}
		   
			public DeviceAdditionalPropertiesCollection Where(WhereDelegate<DeviceAdditionalPropertiesColumns> where, OrderBy<DeviceAdditionalPropertiesColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.Where(where, orderBy, db);
			}

			public DeviceAdditionalProperties OneWhere(WhereDelegate<DeviceAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.OneWhere(where, db);
			}

			public static DeviceAdditionalProperties GetOneWhere(WhereDelegate<DeviceAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.GetOneWhere(where, db);
			}
		
			public DeviceAdditionalProperties FirstOneWhere(WhereDelegate<DeviceAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.FirstOneWhere(where, db);
			}

			public DeviceAdditionalPropertiesCollection Top(int count, WhereDelegate<DeviceAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.Top(count, where, db);
			}

			public DeviceAdditionalPropertiesCollection Top(int count, WhereDelegate<DeviceAdditionalPropertiesColumns> where, OrderBy<DeviceAdditionalPropertiesColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<DeviceAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.DeviceAdditionalProperties.Count(where, db);
			}
	}

	static DeviceAdditionalPropertiesQueryContext _deviceAdditionalProperties;
	static object _deviceAdditionalPropertiesLock = new object();
	public static DeviceAdditionalPropertiesQueryContext DeviceAdditionalProperties
	{
		get
		{
			return _deviceAdditionalPropertiesLock.DoubleCheckLock<DeviceAdditionalPropertiesQueryContext>(ref _deviceAdditionalProperties, () => new DeviceAdditionalPropertiesQueryContext());
		}
	}
	public class DeviceCertificateDataQueryContext
	{
			public DeviceCertificateDataCollection Where(WhereDelegate<DeviceCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.Where(where, db);
			}
		   
			public DeviceCertificateDataCollection Where(WhereDelegate<DeviceCertificateDataColumns> where, OrderBy<DeviceCertificateDataColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.Where(where, orderBy, db);
			}

			public DeviceCertificateData OneWhere(WhereDelegate<DeviceCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.OneWhere(where, db);
			}

			public static DeviceCertificateData GetOneWhere(WhereDelegate<DeviceCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.GetOneWhere(where, db);
			}
		
			public DeviceCertificateData FirstOneWhere(WhereDelegate<DeviceCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.FirstOneWhere(where, db);
			}

			public DeviceCertificateDataCollection Top(int count, WhereDelegate<DeviceCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.Top(count, where, db);
			}

			public DeviceCertificateDataCollection Top(int count, WhereDelegate<DeviceCertificateDataColumns> where, OrderBy<DeviceCertificateDataColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<DeviceCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.DeviceCertificateData.Count(where, db);
			}
	}

	static DeviceCertificateDataQueryContext _deviceCertificateDatas;
	static object _deviceCertificateDatasLock = new object();
	public static DeviceCertificateDataQueryContext DeviceCertificateDatas
	{
		get
		{
			return _deviceCertificateDatasLock.DoubleCheckLock<DeviceCertificateDataQueryContext>(ref _deviceCertificateDatas, () => new DeviceCertificateDataQueryContext());
		}
	}
	public class GroupAdditionalPropertiesQueryContext
	{
			public GroupAdditionalPropertiesCollection Where(WhereDelegate<GroupAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.Where(where, db);
			}
		   
			public GroupAdditionalPropertiesCollection Where(WhereDelegate<GroupAdditionalPropertiesColumns> where, OrderBy<GroupAdditionalPropertiesColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.Where(where, orderBy, db);
			}

			public GroupAdditionalProperties OneWhere(WhereDelegate<GroupAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.OneWhere(where, db);
			}

			public static GroupAdditionalProperties GetOneWhere(WhereDelegate<GroupAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.GetOneWhere(where, db);
			}
		
			public GroupAdditionalProperties FirstOneWhere(WhereDelegate<GroupAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.FirstOneWhere(where, db);
			}

			public GroupAdditionalPropertiesCollection Top(int count, WhereDelegate<GroupAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.Top(count, where, db);
			}

			public GroupAdditionalPropertiesCollection Top(int count, WhereDelegate<GroupAdditionalPropertiesColumns> where, OrderBy<GroupAdditionalPropertiesColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<GroupAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupAdditionalProperties.Count(where, db);
			}
	}

	static GroupAdditionalPropertiesQueryContext _groupAdditionalProperties;
	static object _groupAdditionalPropertiesLock = new object();
	public static GroupAdditionalPropertiesQueryContext GroupAdditionalProperties
	{
		get
		{
			return _groupAdditionalPropertiesLock.DoubleCheckLock<GroupAdditionalPropertiesQueryContext>(ref _groupAdditionalProperties, () => new GroupAdditionalPropertiesQueryContext());
		}
	}
	public class GroupDataQueryContext
	{
			public GroupDataCollection Where(WhereDelegate<GroupDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupData.Where(where, db);
			}
		   
			public GroupDataCollection Where(WhereDelegate<GroupDataColumns> where, OrderBy<GroupDataColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupData.Where(where, orderBy, db);
			}

			public GroupData OneWhere(WhereDelegate<GroupDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupData.OneWhere(where, db);
			}

			public static GroupData GetOneWhere(WhereDelegate<GroupDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupData.GetOneWhere(where, db);
			}
		
			public GroupData FirstOneWhere(WhereDelegate<GroupDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupData.FirstOneWhere(where, db);
			}

			public GroupDataCollection Top(int count, WhereDelegate<GroupDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupData.Top(count, where, db);
			}

			public GroupDataCollection Top(int count, WhereDelegate<GroupDataColumns> where, OrderBy<GroupDataColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<GroupDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupData.Count(where, db);
			}
	}

	static GroupDataQueryContext _groupDatas;
	static object _groupDatasLock = new object();
	public static GroupDataQueryContext GroupDatas
	{
		get
		{
			return _groupDatasLock.DoubleCheckLock<GroupDataQueryContext>(ref _groupDatas, () => new GroupDataQueryContext());
		}
	}
	public class PersonDataQueryContext
	{
			public PersonDataCollection Where(WhereDelegate<PersonDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonData.Where(where, db);
			}
		   
			public PersonDataCollection Where(WhereDelegate<PersonDataColumns> where, OrderBy<PersonDataColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonData.Where(where, orderBy, db);
			}

			public PersonData OneWhere(WhereDelegate<PersonDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonData.OneWhere(where, db);
			}

			public static PersonData GetOneWhere(WhereDelegate<PersonDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonData.GetOneWhere(where, db);
			}
		
			public PersonData FirstOneWhere(WhereDelegate<PersonDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonData.FirstOneWhere(where, db);
			}

			public PersonDataCollection Top(int count, WhereDelegate<PersonDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonData.Top(count, where, db);
			}

			public PersonDataCollection Top(int count, WhereDelegate<PersonDataColumns> where, OrderBy<PersonDataColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<PersonDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonData.Count(where, db);
			}
	}

	static PersonDataQueryContext _personDatas;
	static object _personDatasLock = new object();
	public static PersonDataQueryContext PersonDatas
	{
		get
		{
			return _personDatasLock.DoubleCheckLock<PersonDataQueryContext>(ref _personDatas, () => new PersonDataQueryContext());
		}
	}
	public class OrganizationDataQueryContext
	{
			public OrganizationDataCollection Where(WhereDelegate<OrganizationDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationData.Where(where, db);
			}
		   
			public OrganizationDataCollection Where(WhereDelegate<OrganizationDataColumns> where, OrderBy<OrganizationDataColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationData.Where(where, orderBy, db);
			}

			public OrganizationData OneWhere(WhereDelegate<OrganizationDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationData.OneWhere(where, db);
			}

			public static OrganizationData GetOneWhere(WhereDelegate<OrganizationDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationData.GetOneWhere(where, db);
			}
		
			public OrganizationData FirstOneWhere(WhereDelegate<OrganizationDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationData.FirstOneWhere(where, db);
			}

			public OrganizationDataCollection Top(int count, WhereDelegate<OrganizationDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationData.Top(count, where, db);
			}

			public OrganizationDataCollection Top(int count, WhereDelegate<OrganizationDataColumns> where, OrderBy<OrganizationDataColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<OrganizationDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationData.Count(where, db);
			}
	}

	static OrganizationDataQueryContext _organizationDatas;
	static object _organizationDatasLock = new object();
	public static OrganizationDataQueryContext OrganizationDatas
	{
		get
		{
			return _organizationDatasLock.DoubleCheckLock<OrganizationDataQueryContext>(ref _organizationDatas, () => new OrganizationDataQueryContext());
		}
	}
	public class MailingAddressDataQueryContext
	{
			public MailingAddressDataCollection Where(WhereDelegate<MailingAddressDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.MailingAddressData.Where(where, db);
			}
		   
			public MailingAddressDataCollection Where(WhereDelegate<MailingAddressDataColumns> where, OrderBy<MailingAddressDataColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.MailingAddressData.Where(where, orderBy, db);
			}

			public MailingAddressData OneWhere(WhereDelegate<MailingAddressDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.MailingAddressData.OneWhere(where, db);
			}

			public static MailingAddressData GetOneWhere(WhereDelegate<MailingAddressDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.MailingAddressData.GetOneWhere(where, db);
			}
		
			public MailingAddressData FirstOneWhere(WhereDelegate<MailingAddressDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.MailingAddressData.FirstOneWhere(where, db);
			}

			public MailingAddressDataCollection Top(int count, WhereDelegate<MailingAddressDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.MailingAddressData.Top(count, where, db);
			}

			public MailingAddressDataCollection Top(int count, WhereDelegate<MailingAddressDataColumns> where, OrderBy<MailingAddressDataColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.MailingAddressData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<MailingAddressDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.MailingAddressData.Count(where, db);
			}
	}

	static MailingAddressDataQueryContext _mailingAddressDatas;
	static object _mailingAddressDatasLock = new object();
	public static MailingAddressDataQueryContext MailingAddressDatas
	{
		get
		{
			return _mailingAddressDatasLock.DoubleCheckLock<MailingAddressDataQueryContext>(ref _mailingAddressDatas, () => new MailingAddressDataQueryContext());
		}
	}
	public class OrganizationAdditionalPropertiesQueryContext
	{
			public OrganizationAdditionalPropertiesCollection Where(WhereDelegate<OrganizationAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.Where(where, db);
			}
		   
			public OrganizationAdditionalPropertiesCollection Where(WhereDelegate<OrganizationAdditionalPropertiesColumns> where, OrderBy<OrganizationAdditionalPropertiesColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.Where(where, orderBy, db);
			}

			public OrganizationAdditionalProperties OneWhere(WhereDelegate<OrganizationAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.OneWhere(where, db);
			}

			public static OrganizationAdditionalProperties GetOneWhere(WhereDelegate<OrganizationAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.GetOneWhere(where, db);
			}
		
			public OrganizationAdditionalProperties FirstOneWhere(WhereDelegate<OrganizationAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.FirstOneWhere(where, db);
			}

			public OrganizationAdditionalPropertiesCollection Top(int count, WhereDelegate<OrganizationAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.Top(count, where, db);
			}

			public OrganizationAdditionalPropertiesCollection Top(int count, WhereDelegate<OrganizationAdditionalPropertiesColumns> where, OrderBy<OrganizationAdditionalPropertiesColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<OrganizationAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationAdditionalProperties.Count(where, db);
			}
	}

	static OrganizationAdditionalPropertiesQueryContext _organizationAdditionalProperties;
	static object _organizationAdditionalPropertiesLock = new object();
	public static OrganizationAdditionalPropertiesQueryContext OrganizationAdditionalProperties
	{
		get
		{
			return _organizationAdditionalPropertiesLock.DoubleCheckLock<OrganizationAdditionalPropertiesQueryContext>(ref _organizationAdditionalProperties, () => new OrganizationAdditionalPropertiesQueryContext());
		}
	}
	public class OrganizationCertificateDataQueryContext
	{
			public OrganizationCertificateDataCollection Where(WhereDelegate<OrganizationCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.Where(where, db);
			}
		   
			public OrganizationCertificateDataCollection Where(WhereDelegate<OrganizationCertificateDataColumns> where, OrderBy<OrganizationCertificateDataColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.Where(where, orderBy, db);
			}

			public OrganizationCertificateData OneWhere(WhereDelegate<OrganizationCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.OneWhere(where, db);
			}

			public static OrganizationCertificateData GetOneWhere(WhereDelegate<OrganizationCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.GetOneWhere(where, db);
			}
		
			public OrganizationCertificateData FirstOneWhere(WhereDelegate<OrganizationCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.FirstOneWhere(where, db);
			}

			public OrganizationCertificateDataCollection Top(int count, WhereDelegate<OrganizationCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.Top(count, where, db);
			}

			public OrganizationCertificateDataCollection Top(int count, WhereDelegate<OrganizationCertificateDataColumns> where, OrderBy<OrganizationCertificateDataColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<OrganizationCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationCertificateData.Count(where, db);
			}
	}

	static OrganizationCertificateDataQueryContext _organizationCertificateDatas;
	static object _organizationCertificateDatasLock = new object();
	public static OrganizationCertificateDataQueryContext OrganizationCertificateDatas
	{
		get
		{
			return _organizationCertificateDatasLock.DoubleCheckLock<OrganizationCertificateDataQueryContext>(ref _organizationCertificateDatas, () => new OrganizationCertificateDataQueryContext());
		}
	}
	public class OrganizationMailingAddressQueryContext
	{
			public OrganizationMailingAddressCollection Where(WhereDelegate<OrganizationMailingAddressColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.Where(where, db);
			}
		   
			public OrganizationMailingAddressCollection Where(WhereDelegate<OrganizationMailingAddressColumns> where, OrderBy<OrganizationMailingAddressColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.Where(where, orderBy, db);
			}

			public OrganizationMailingAddress OneWhere(WhereDelegate<OrganizationMailingAddressColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.OneWhere(where, db);
			}

			public static OrganizationMailingAddress GetOneWhere(WhereDelegate<OrganizationMailingAddressColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.GetOneWhere(where, db);
			}
		
			public OrganizationMailingAddress FirstOneWhere(WhereDelegate<OrganizationMailingAddressColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.FirstOneWhere(where, db);
			}

			public OrganizationMailingAddressCollection Top(int count, WhereDelegate<OrganizationMailingAddressColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.Top(count, where, db);
			}

			public OrganizationMailingAddressCollection Top(int count, WhereDelegate<OrganizationMailingAddressColumns> where, OrderBy<OrganizationMailingAddressColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<OrganizationMailingAddressColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.OrganizationMailingAddress.Count(where, db);
			}
	}

	static OrganizationMailingAddressQueryContext _organizationMailingAddresses;
	static object _organizationMailingAddressesLock = new object();
	public static OrganizationMailingAddressQueryContext OrganizationMailingAddresses
	{
		get
		{
			return _organizationMailingAddressesLock.DoubleCheckLock<OrganizationMailingAddressQueryContext>(ref _organizationMailingAddresses, () => new OrganizationMailingAddressQueryContext());
		}
	}
	public class PersonAdditionalPropertiesQueryContext
	{
			public PersonAdditionalPropertiesCollection Where(WhereDelegate<PersonAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.Where(where, db);
			}
		   
			public PersonAdditionalPropertiesCollection Where(WhereDelegate<PersonAdditionalPropertiesColumns> where, OrderBy<PersonAdditionalPropertiesColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.Where(where, orderBy, db);
			}

			public PersonAdditionalProperties OneWhere(WhereDelegate<PersonAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.OneWhere(where, db);
			}

			public static PersonAdditionalProperties GetOneWhere(WhereDelegate<PersonAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.GetOneWhere(where, db);
			}
		
			public PersonAdditionalProperties FirstOneWhere(WhereDelegate<PersonAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.FirstOneWhere(where, db);
			}

			public PersonAdditionalPropertiesCollection Top(int count, WhereDelegate<PersonAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.Top(count, where, db);
			}

			public PersonAdditionalPropertiesCollection Top(int count, WhereDelegate<PersonAdditionalPropertiesColumns> where, OrderBy<PersonAdditionalPropertiesColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<PersonAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonAdditionalProperties.Count(where, db);
			}
	}

	static PersonAdditionalPropertiesQueryContext _personAdditionalProperties;
	static object _personAdditionalPropertiesLock = new object();
	public static PersonAdditionalPropertiesQueryContext PersonAdditionalProperties
	{
		get
		{
			return _personAdditionalPropertiesLock.DoubleCheckLock<PersonAdditionalPropertiesQueryContext>(ref _personAdditionalProperties, () => new PersonAdditionalPropertiesQueryContext());
		}
	}
	public class PersonCertificateDataQueryContext
	{
			public PersonCertificateDataCollection Where(WhereDelegate<PersonCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonCertificateData.Where(where, db);
			}
		   
			public PersonCertificateDataCollection Where(WhereDelegate<PersonCertificateDataColumns> where, OrderBy<PersonCertificateDataColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonCertificateData.Where(where, orderBy, db);
			}

			public PersonCertificateData OneWhere(WhereDelegate<PersonCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonCertificateData.OneWhere(where, db);
			}

			public static PersonCertificateData GetOneWhere(WhereDelegate<PersonCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonCertificateData.GetOneWhere(where, db);
			}
		
			public PersonCertificateData FirstOneWhere(WhereDelegate<PersonCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonCertificateData.FirstOneWhere(where, db);
			}

			public PersonCertificateDataCollection Top(int count, WhereDelegate<PersonCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonCertificateData.Top(count, where, db);
			}

			public PersonCertificateDataCollection Top(int count, WhereDelegate<PersonCertificateDataColumns> where, OrderBy<PersonCertificateDataColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonCertificateData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<PersonCertificateDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonCertificateData.Count(where, db);
			}
	}

	static PersonCertificateDataQueryContext _personCertificateDatas;
	static object _personCertificateDatasLock = new object();
	public static PersonCertificateDataQueryContext PersonCertificateDatas
	{
		get
		{
			return _personCertificateDatasLock.DoubleCheckLock<PersonCertificateDataQueryContext>(ref _personCertificateDatas, () => new PersonCertificateDataQueryContext());
		}
	}
	public class PersonMailingAddressDataQueryContext
	{
			public PersonMailingAddressDataCollection Where(WhereDelegate<PersonMailingAddressDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.Where(where, db);
			}
		   
			public PersonMailingAddressDataCollection Where(WhereDelegate<PersonMailingAddressDataColumns> where, OrderBy<PersonMailingAddressDataColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.Where(where, orderBy, db);
			}

			public PersonMailingAddressData OneWhere(WhereDelegate<PersonMailingAddressDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.OneWhere(where, db);
			}

			public static PersonMailingAddressData GetOneWhere(WhereDelegate<PersonMailingAddressDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.GetOneWhere(where, db);
			}
		
			public PersonMailingAddressData FirstOneWhere(WhereDelegate<PersonMailingAddressDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.FirstOneWhere(where, db);
			}

			public PersonMailingAddressDataCollection Top(int count, WhereDelegate<PersonMailingAddressDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.Top(count, where, db);
			}

			public PersonMailingAddressDataCollection Top(int count, WhereDelegate<PersonMailingAddressDataColumns> where, OrderBy<PersonMailingAddressDataColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<PersonMailingAddressDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonMailingAddressData.Count(where, db);
			}
	}

	static PersonMailingAddressDataQueryContext _personMailingAddressDatas;
	static object _personMailingAddressDatasLock = new object();
	public static PersonMailingAddressDataQueryContext PersonMailingAddressDatas
	{
		get
		{
			return _personMailingAddressDatasLock.DoubleCheckLock<PersonMailingAddressDataQueryContext>(ref _personMailingAddressDatas, () => new PersonMailingAddressDataQueryContext());
		}
	}
	public class ProfileAccountDataQueryContext
	{
			public ProfileAccountDataCollection Where(WhereDelegate<ProfileAccountDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.ProfileAccountData.Where(where, db);
			}
		   
			public ProfileAccountDataCollection Where(WhereDelegate<ProfileAccountDataColumns> where, OrderBy<ProfileAccountDataColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.ProfileAccountData.Where(where, orderBy, db);
			}

			public ProfileAccountData OneWhere(WhereDelegate<ProfileAccountDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.ProfileAccountData.OneWhere(where, db);
			}

			public static ProfileAccountData GetOneWhere(WhereDelegate<ProfileAccountDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.ProfileAccountData.GetOneWhere(where, db);
			}
		
			public ProfileAccountData FirstOneWhere(WhereDelegate<ProfileAccountDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.ProfileAccountData.FirstOneWhere(where, db);
			}

			public ProfileAccountDataCollection Top(int count, WhereDelegate<ProfileAccountDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.ProfileAccountData.Top(count, where, db);
			}

			public ProfileAccountDataCollection Top(int count, WhereDelegate<ProfileAccountDataColumns> where, OrderBy<ProfileAccountDataColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.ProfileAccountData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<ProfileAccountDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.ProfileAccountData.Count(where, db);
			}
	}

	static ProfileAccountDataQueryContext _profileAccountDatas;
	static object _profileAccountDatasLock = new object();
	public static ProfileAccountDataQueryContext ProfileAccountDatas
	{
		get
		{
			return _profileAccountDatasLock.DoubleCheckLock<ProfileAccountDataQueryContext>(ref _profileAccountDatas, () => new ProfileAccountDataQueryContext());
		}
	}
	public class ProfileAdditionalPropertiesQueryContext
	{
			public ProfileAdditionalPropertiesCollection Where(WhereDelegate<ProfileAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.Where(where, db);
			}
		   
			public ProfileAdditionalPropertiesCollection Where(WhereDelegate<ProfileAdditionalPropertiesColumns> where, OrderBy<ProfileAdditionalPropertiesColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.Where(where, orderBy, db);
			}

			public ProfileAdditionalProperties OneWhere(WhereDelegate<ProfileAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.OneWhere(where, db);
			}

			public static ProfileAdditionalProperties GetOneWhere(WhereDelegate<ProfileAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.GetOneWhere(where, db);
			}
		
			public ProfileAdditionalProperties FirstOneWhere(WhereDelegate<ProfileAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.FirstOneWhere(where, db);
			}

			public ProfileAdditionalPropertiesCollection Top(int count, WhereDelegate<ProfileAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.Top(count, where, db);
			}

			public ProfileAdditionalPropertiesCollection Top(int count, WhereDelegate<ProfileAdditionalPropertiesColumns> where, OrderBy<ProfileAdditionalPropertiesColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<ProfileAdditionalPropertiesColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.ProfileAdditionalProperties.Count(where, db);
			}
	}

	static ProfileAdditionalPropertiesQueryContext _profileAdditionalProperties;
	static object _profileAdditionalPropertiesLock = new object();
	public static ProfileAdditionalPropertiesQueryContext ProfileAdditionalProperties
	{
		get
		{
			return _profileAdditionalPropertiesLock.DoubleCheckLock<ProfileAdditionalPropertiesQueryContext>(ref _profileAdditionalProperties, () => new ProfileAdditionalPropertiesQueryContext());
		}
	}
	public class PublicKeySetDataQueryContext
	{
			public PublicKeySetDataCollection Where(WhereDelegate<PublicKeySetDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PublicKeySetData.Where(where, db);
			}
		   
			public PublicKeySetDataCollection Where(WhereDelegate<PublicKeySetDataColumns> where, OrderBy<PublicKeySetDataColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PublicKeySetData.Where(where, orderBy, db);
			}

			public PublicKeySetData OneWhere(WhereDelegate<PublicKeySetDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PublicKeySetData.OneWhere(where, db);
			}

			public static PublicKeySetData GetOneWhere(WhereDelegate<PublicKeySetDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PublicKeySetData.GetOneWhere(where, db);
			}
		
			public PublicKeySetData FirstOneWhere(WhereDelegate<PublicKeySetDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PublicKeySetData.FirstOneWhere(where, db);
			}

			public PublicKeySetDataCollection Top(int count, WhereDelegate<PublicKeySetDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PublicKeySetData.Top(count, where, db);
			}

			public PublicKeySetDataCollection Top(int count, WhereDelegate<PublicKeySetDataColumns> where, OrderBy<PublicKeySetDataColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PublicKeySetData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<PublicKeySetDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PublicKeySetData.Count(where, db);
			}
	}

	static PublicKeySetDataQueryContext _publicKeySetDatas;
	static object _publicKeySetDatasLock = new object();
	public static PublicKeySetDataQueryContext PublicKeySetDatas
	{
		get
		{
			return _publicKeySetDatasLock.DoubleCheckLock<PublicKeySetDataQueryContext>(ref _publicKeySetDatas, () => new PublicKeySetDataQueryContext());
		}
	}
	public class GroupDataPersonDataQueryContext
	{
			public GroupDataPersonDataCollection Where(WhereDelegate<GroupDataPersonDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupDataPersonData.Where(where, db);
			}
		   
			public GroupDataPersonDataCollection Where(WhereDelegate<GroupDataPersonDataColumns> where, OrderBy<GroupDataPersonDataColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupDataPersonData.Where(where, orderBy, db);
			}

			public GroupDataPersonData OneWhere(WhereDelegate<GroupDataPersonDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupDataPersonData.OneWhere(where, db);
			}

			public static GroupDataPersonData GetOneWhere(WhereDelegate<GroupDataPersonDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupDataPersonData.GetOneWhere(where, db);
			}
		
			public GroupDataPersonData FirstOneWhere(WhereDelegate<GroupDataPersonDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupDataPersonData.FirstOneWhere(where, db);
			}

			public GroupDataPersonDataCollection Top(int count, WhereDelegate<GroupDataPersonDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupDataPersonData.Top(count, where, db);
			}

			public GroupDataPersonDataCollection Top(int count, WhereDelegate<GroupDataPersonDataColumns> where, OrderBy<GroupDataPersonDataColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupDataPersonData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<GroupDataPersonDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.GroupDataPersonData.Count(where, db);
			}
	}

	static GroupDataPersonDataQueryContext _groupDataPersonDatas;
	static object _groupDataPersonDatasLock = new object();
	public static GroupDataPersonDataQueryContext GroupDataPersonDatas
	{
		get
		{
			return _groupDataPersonDatasLock.DoubleCheckLock<GroupDataPersonDataQueryContext>(ref _groupDataPersonDatas, () => new GroupDataPersonDataQueryContext());
		}
	}
	public class PersonDataOrganizationDataQueryContext
	{
			public PersonDataOrganizationDataCollection Where(WhereDelegate<PersonDataOrganizationDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonDataOrganizationData.Where(where, db);
			}
		   
			public PersonDataOrganizationDataCollection Where(WhereDelegate<PersonDataOrganizationDataColumns> where, OrderBy<PersonDataOrganizationDataColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonDataOrganizationData.Where(where, orderBy, db);
			}

			public PersonDataOrganizationData OneWhere(WhereDelegate<PersonDataOrganizationDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonDataOrganizationData.OneWhere(where, db);
			}

			public static PersonDataOrganizationData GetOneWhere(WhereDelegate<PersonDataOrganizationDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonDataOrganizationData.GetOneWhere(where, db);
			}
		
			public PersonDataOrganizationData FirstOneWhere(WhereDelegate<PersonDataOrganizationDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonDataOrganizationData.FirstOneWhere(where, db);
			}

			public PersonDataOrganizationDataCollection Top(int count, WhereDelegate<PersonDataOrganizationDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonDataOrganizationData.Top(count, where, db);
			}

			public PersonDataOrganizationDataCollection Top(int count, WhereDelegate<PersonDataOrganizationDataColumns> where, OrderBy<PersonDataOrganizationDataColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonDataOrganizationData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<PersonDataOrganizationDataColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PersonDataOrganizationData.Count(where, db);
			}
	}

	static PersonDataOrganizationDataQueryContext _personDataOrganizationDatas;
	static object _personDataOrganizationDatasLock = new object();
	public static PersonDataOrganizationDataQueryContext PersonDataOrganizationDatas
	{
		get
		{
			return _personDataOrganizationDatasLock.DoubleCheckLock<PersonDataOrganizationDataQueryContext>(ref _personDataOrganizationDatas, () => new PersonDataOrganizationDataQueryContext());
		}
	}
    }
}																								
