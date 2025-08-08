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
	// schema = ServerSessionData
    public static class ServerSessionDataContext
    {
		public static string ConnectionName
		{
			get
			{
				return "ServerSessionData";
			}
		}

		public static IDatabase Db
		{
			get
			{
				return Bam.Data.Db.For(ConnectionName);
			}
		}


	public class ActorQueryContext
	{
			public ActorCollection Where(WhereDelegate<ActorColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Actor.Where(where, db);
			}
		   
			public ActorCollection Where(WhereDelegate<ActorColumns> where, OrderBy<ActorColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Actor.Where(where, orderBy, db);
			}

			public Actor OneWhere(WhereDelegate<ActorColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Actor.OneWhere(where, db);
			}

			public static Actor GetOneWhere(WhereDelegate<ActorColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Actor.GetOneWhere(where, db);
			}
		
			public Actor FirstOneWhere(WhereDelegate<ActorColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Actor.FirstOneWhere(where, db);
			}

			public ActorCollection Top(int count, WhereDelegate<ActorColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Actor.Top(count, where, db);
			}

			public ActorCollection Top(int count, WhereDelegate<ActorColumns> where, OrderBy<ActorColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Actor.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<ActorColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Actor.Count(where, db);
			}
	}

	static ActorQueryContext _actors;
	static object _actorsLock = new object();
	public static ActorQueryContext Actors
	{
		get
		{
			return _actorsLock.DoubleCheckLock<ActorQueryContext>(ref _actors, () => new ActorQueryContext());
		}
	}
	public class DeviceQueryContext
	{
			public DeviceCollection Where(WhereDelegate<DeviceColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Device.Where(where, db);
			}
		   
			public DeviceCollection Where(WhereDelegate<DeviceColumns> where, OrderBy<DeviceColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Device.Where(where, orderBy, db);
			}

			public Device OneWhere(WhereDelegate<DeviceColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Device.OneWhere(where, db);
			}

			public static Device GetOneWhere(WhereDelegate<DeviceColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Device.GetOneWhere(where, db);
			}
		
			public Device FirstOneWhere(WhereDelegate<DeviceColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Device.FirstOneWhere(where, db);
			}

			public DeviceCollection Top(int count, WhereDelegate<DeviceColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Device.Top(count, where, db);
			}

			public DeviceCollection Top(int count, WhereDelegate<DeviceColumns> where, OrderBy<DeviceColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Device.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<DeviceColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Device.Count(where, db);
			}
	}

	static DeviceQueryContext _devices;
	static object _devicesLock = new object();
	public static DeviceQueryContext Devices
	{
		get
		{
			return _devicesLock.DoubleCheckLock<DeviceQueryContext>(ref _devices, () => new DeviceQueryContext());
		}
	}
	public class OrganizationQueryContext
	{
			public OrganizationCollection Where(WhereDelegate<OrganizationColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Organization.Where(where, db);
			}
		   
			public OrganizationCollection Where(WhereDelegate<OrganizationColumns> where, OrderBy<OrganizationColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Organization.Where(where, orderBy, db);
			}

			public Organization OneWhere(WhereDelegate<OrganizationColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Organization.OneWhere(where, db);
			}

			public static Organization GetOneWhere(WhereDelegate<OrganizationColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Organization.GetOneWhere(where, db);
			}
		
			public Organization FirstOneWhere(WhereDelegate<OrganizationColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Organization.FirstOneWhere(where, db);
			}

			public OrganizationCollection Top(int count, WhereDelegate<OrganizationColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Organization.Top(count, where, db);
			}

			public OrganizationCollection Top(int count, WhereDelegate<OrganizationColumns> where, OrderBy<OrganizationColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Organization.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<OrganizationColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Organization.Count(where, db);
			}
	}

	static OrganizationQueryContext _organizations;
	static object _organizationsLock = new object();
	public static OrganizationQueryContext Organizations
	{
		get
		{
			return _organizationsLock.DoubleCheckLock<OrganizationQueryContext>(ref _organizations, () => new OrganizationQueryContext());
		}
	}
	public class PersonQueryContext
	{
			public PersonCollection Where(WhereDelegate<PersonColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Person.Where(where, db);
			}
		   
			public PersonCollection Where(WhereDelegate<PersonColumns> where, OrderBy<PersonColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Person.Where(where, orderBy, db);
			}

			public Person OneWhere(WhereDelegate<PersonColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Person.OneWhere(where, db);
			}

			public static Person GetOneWhere(WhereDelegate<PersonColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Person.GetOneWhere(where, db);
			}
		
			public Person FirstOneWhere(WhereDelegate<PersonColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Person.FirstOneWhere(where, db);
			}

			public PersonCollection Top(int count, WhereDelegate<PersonColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Person.Top(count, where, db);
			}

			public PersonCollection Top(int count, WhereDelegate<PersonColumns> where, OrderBy<PersonColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Person.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<PersonColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Person.Count(where, db);
			}
	}

	static PersonQueryContext _persons;
	static object _personsLock = new object();
	public static PersonQueryContext Persons
	{
		get
		{
			return _personsLock.DoubleCheckLock<PersonQueryContext>(ref _persons, () => new PersonQueryContext());
		}
	}
	public class PrivateKeySetQueryContext
	{
			public PrivateKeySetCollection Where(WhereDelegate<PrivateKeySetColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PrivateKeySet.Where(where, db);
			}
		   
			public PrivateKeySetCollection Where(WhereDelegate<PrivateKeySetColumns> where, OrderBy<PrivateKeySetColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PrivateKeySet.Where(where, orderBy, db);
			}

			public PrivateKeySet OneWhere(WhereDelegate<PrivateKeySetColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PrivateKeySet.OneWhere(where, db);
			}

			public static PrivateKeySet GetOneWhere(WhereDelegate<PrivateKeySetColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PrivateKeySet.GetOneWhere(where, db);
			}
		
			public PrivateKeySet FirstOneWhere(WhereDelegate<PrivateKeySetColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PrivateKeySet.FirstOneWhere(where, db);
			}

			public PrivateKeySetCollection Top(int count, WhereDelegate<PrivateKeySetColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PrivateKeySet.Top(count, where, db);
			}

			public PrivateKeySetCollection Top(int count, WhereDelegate<PrivateKeySetColumns> where, OrderBy<PrivateKeySetColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PrivateKeySet.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<PrivateKeySetColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PrivateKeySet.Count(where, db);
			}
	}

	static PrivateKeySetQueryContext _privateKeySets;
	static object _privateKeySetsLock = new object();
	public static PrivateKeySetQueryContext PrivateKeySets
	{
		get
		{
			return _privateKeySetsLock.DoubleCheckLock<PrivateKeySetQueryContext>(ref _privateKeySets, () => new PrivateKeySetQueryContext());
		}
	}
	public class ProfileQueryContext
	{
			public ProfileCollection Where(WhereDelegate<ProfileColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Profile.Where(where, db);
			}
		   
			public ProfileCollection Where(WhereDelegate<ProfileColumns> where, OrderBy<ProfileColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Profile.Where(where, orderBy, db);
			}

			public Profile OneWhere(WhereDelegate<ProfileColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Profile.OneWhere(where, db);
			}

			public static Profile GetOneWhere(WhereDelegate<ProfileColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Profile.GetOneWhere(where, db);
			}
		
			public Profile FirstOneWhere(WhereDelegate<ProfileColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Profile.FirstOneWhere(where, db);
			}

			public ProfileCollection Top(int count, WhereDelegate<ProfileColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Profile.Top(count, where, db);
			}

			public ProfileCollection Top(int count, WhereDelegate<ProfileColumns> where, OrderBy<ProfileColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Profile.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<ProfileColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.Profile.Count(where, db);
			}
	}

	static ProfileQueryContext _profiles;
	static object _profilesLock = new object();
	public static ProfileQueryContext Profiles
	{
		get
		{
			return _profilesLock.DoubleCheckLock<ProfileQueryContext>(ref _profiles, () => new ProfileQueryContext());
		}
	}
	public class PublicKeySetQueryContext
	{
			public PublicKeySetCollection Where(WhereDelegate<PublicKeySetColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PublicKeySet.Where(where, db);
			}
		   
			public PublicKeySetCollection Where(WhereDelegate<PublicKeySetColumns> where, OrderBy<PublicKeySetColumns> orderBy = null, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PublicKeySet.Where(where, orderBy, db);
			}

			public PublicKeySet OneWhere(WhereDelegate<PublicKeySetColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PublicKeySet.OneWhere(where, db);
			}

			public static PublicKeySet GetOneWhere(WhereDelegate<PublicKeySetColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PublicKeySet.GetOneWhere(where, db);
			}
		
			public PublicKeySet FirstOneWhere(WhereDelegate<PublicKeySetColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PublicKeySet.FirstOneWhere(where, db);
			}

			public PublicKeySetCollection Top(int count, WhereDelegate<PublicKeySetColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PublicKeySet.Top(count, where, db);
			}

			public PublicKeySetCollection Top(int count, WhereDelegate<PublicKeySetColumns> where, OrderBy<PublicKeySetColumns> orderBy, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PublicKeySet.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<PublicKeySetColumns> where, Database db = null)
			{
				return Bam.Protocol.Data.Profile.Dao.PublicKeySet.Count(where, db);
			}
	}

	static PublicKeySetQueryContext _publicKeySets;
	static object _publicKeySetsLock = new object();
	public static PublicKeySetQueryContext PublicKeySets
	{
		get
		{
			return _publicKeySetsLock.DoubleCheckLock<PublicKeySetQueryContext>(ref _publicKeySets, () => new PublicKeySetQueryContext());
		}
	}
    }
}																								
