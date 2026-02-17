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

namespace Bam.Protocol.Data.Common.Dao
{
	// schema = CommonSchema
    public static class CommonSchemaContext
    {
		public static string ConnectionName
		{
			get
			{
				return "CommonSchema";
			}
		}

		public static IDatabase Db
		{
			get
			{
				return Bam.Data.Db.For(ConnectionName);
			}
		}


	public class ActorDataQueryContext
	{
			public ActorDataCollection Where(WhereDelegate<ActorDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.ActorData.Where(where, db);
			}
		   
			public ActorDataCollection Where(WhereDelegate<ActorDataColumns> where, OrderBy<ActorDataColumns> orderBy = null!, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.ActorData.Where(where, orderBy, db);
			}

			public ActorData OneWhere(WhereDelegate<ActorDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.ActorData.OneWhere(where, db);
			}

			public static ActorData GetOneWhere(WhereDelegate<ActorDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.ActorData.GetOneWhere(where, db);
			}
		
			public ActorData FirstOneWhere(WhereDelegate<ActorDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.ActorData.FirstOneWhere(where, db);
			}

			public ActorDataCollection Top(int count, WhereDelegate<ActorDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.ActorData.Top(count, where, db);
			}

			public ActorDataCollection Top(int count, WhereDelegate<ActorDataColumns> where, OrderBy<ActorDataColumns> orderBy, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.ActorData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<ActorDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.ActorData.Count(where, db);
			}
	}

	static ActorDataQueryContext _actorDatas = null!;
	static object _actorDatasLock = new object();
	public static ActorDataQueryContext ActorDatas
	{
		get
		{
			return _actorDatasLock.DoubleCheckLock<ActorDataQueryContext>(ref _actorDatas, () => new ActorDataQueryContext());
		}
	}
	public class AgentDataQueryContext
	{
			public AgentDataCollection Where(WhereDelegate<AgentDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.AgentData.Where(where, db);
			}
		   
			public AgentDataCollection Where(WhereDelegate<AgentDataColumns> where, OrderBy<AgentDataColumns> orderBy = null!, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.AgentData.Where(where, orderBy, db);
			}

			public AgentData OneWhere(WhereDelegate<AgentDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.AgentData.OneWhere(where, db);
			}

			public static AgentData GetOneWhere(WhereDelegate<AgentDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.AgentData.GetOneWhere(where, db);
			}
		
			public AgentData FirstOneWhere(WhereDelegate<AgentDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.AgentData.FirstOneWhere(where, db);
			}

			public AgentDataCollection Top(int count, WhereDelegate<AgentDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.AgentData.Top(count, where, db);
			}

			public AgentDataCollection Top(int count, WhereDelegate<AgentDataColumns> where, OrderBy<AgentDataColumns> orderBy, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.AgentData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<AgentDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.AgentData.Count(where, db);
			}
	}

	static AgentDataQueryContext _agentDatas = null!;
	static object _agentDatasLock = new object();
	public static AgentDataQueryContext AgentDatas
	{
		get
		{
			return _agentDatasLock.DoubleCheckLock<AgentDataQueryContext>(ref _agentDatas, () => new AgentDataQueryContext());
		}
	}
	public class DeviceDataQueryContext
	{
			public DeviceDataCollection Where(WhereDelegate<DeviceDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.DeviceData.Where(where, db);
			}
		   
			public DeviceDataCollection Where(WhereDelegate<DeviceDataColumns> where, OrderBy<DeviceDataColumns> orderBy = null!, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.DeviceData.Where(where, orderBy, db);
			}

			public DeviceData OneWhere(WhereDelegate<DeviceDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.DeviceData.OneWhere(where, db);
			}

			public static DeviceData GetOneWhere(WhereDelegate<DeviceDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.DeviceData.GetOneWhere(where, db);
			}
		
			public DeviceData FirstOneWhere(WhereDelegate<DeviceDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.DeviceData.FirstOneWhere(where, db);
			}

			public DeviceDataCollection Top(int count, WhereDelegate<DeviceDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.DeviceData.Top(count, where, db);
			}

			public DeviceDataCollection Top(int count, WhereDelegate<DeviceDataColumns> where, OrderBy<DeviceDataColumns> orderBy, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.DeviceData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<DeviceDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.DeviceData.Count(where, db);
			}
	}

	static DeviceDataQueryContext _deviceDatas = null!;
	static object _deviceDatasLock = new object();
	public static DeviceDataQueryContext DeviceDatas
	{
		get
		{
			return _deviceDatasLock.DoubleCheckLock<DeviceDataQueryContext>(ref _deviceDatas, () => new DeviceDataQueryContext());
		}
	}
	public class HostAddressDataQueryContext
	{
			public HostAddressDataCollection Where(WhereDelegate<HostAddressDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.HostAddressData.Where(where, db);
			}
		   
			public HostAddressDataCollection Where(WhereDelegate<HostAddressDataColumns> where, OrderBy<HostAddressDataColumns> orderBy = null!, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.HostAddressData.Where(where, orderBy, db);
			}

			public HostAddressData OneWhere(WhereDelegate<HostAddressDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.HostAddressData.OneWhere(where, db);
			}

			public static HostAddressData GetOneWhere(WhereDelegate<HostAddressDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.HostAddressData.GetOneWhere(where, db);
			}
		
			public HostAddressData FirstOneWhere(WhereDelegate<HostAddressDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.HostAddressData.FirstOneWhere(where, db);
			}

			public HostAddressDataCollection Top(int count, WhereDelegate<HostAddressDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.HostAddressData.Top(count, where, db);
			}

			public HostAddressDataCollection Top(int count, WhereDelegate<HostAddressDataColumns> where, OrderBy<HostAddressDataColumns> orderBy, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.HostAddressData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<HostAddressDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.HostAddressData.Count(where, db);
			}
	}

	static HostAddressDataQueryContext _hostAddressDatas = null!;
	static object _hostAddressDatasLock = new object();
	public static HostAddressDataQueryContext HostAddressDatas
	{
		get
		{
			return _hostAddressDatasLock.DoubleCheckLock<HostAddressDataQueryContext>(ref _hostAddressDatas, () => new HostAddressDataQueryContext());
		}
	}
	public class NicDataQueryContext
	{
			public NicDataCollection Where(WhereDelegate<NicDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.NicData.Where(where, db);
			}
		   
			public NicDataCollection Where(WhereDelegate<NicDataColumns> where, OrderBy<NicDataColumns> orderBy = null!, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.NicData.Where(where, orderBy, db);
			}

			public NicData OneWhere(WhereDelegate<NicDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.NicData.OneWhere(where, db);
			}

			public static NicData GetOneWhere(WhereDelegate<NicDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.NicData.GetOneWhere(where, db);
			}
		
			public NicData FirstOneWhere(WhereDelegate<NicDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.NicData.FirstOneWhere(where, db);
			}

			public NicDataCollection Top(int count, WhereDelegate<NicDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.NicData.Top(count, where, db);
			}

			public NicDataCollection Top(int count, WhereDelegate<NicDataColumns> where, OrderBy<NicDataColumns> orderBy, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.NicData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<NicDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.NicData.Count(where, db);
			}
	}

	static NicDataQueryContext _nicDatas = null!;
	static object _nicDatasLock = new object();
	public static NicDataQueryContext NicDatas
	{
		get
		{
			return _nicDatasLock.DoubleCheckLock<NicDataQueryContext>(ref _nicDatas, () => new NicDataQueryContext());
		}
	}
	public class MachineDataQueryContext
	{
			public MachineDataCollection Where(WhereDelegate<MachineDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.MachineData.Where(where, db);
			}
		   
			public MachineDataCollection Where(WhereDelegate<MachineDataColumns> where, OrderBy<MachineDataColumns> orderBy = null!, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.MachineData.Where(where, orderBy, db);
			}

			public MachineData OneWhere(WhereDelegate<MachineDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.MachineData.OneWhere(where, db);
			}

			public static MachineData GetOneWhere(WhereDelegate<MachineDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.MachineData.GetOneWhere(where, db);
			}
		
			public MachineData FirstOneWhere(WhereDelegate<MachineDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.MachineData.FirstOneWhere(where, db);
			}

			public MachineDataCollection Top(int count, WhereDelegate<MachineDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.MachineData.Top(count, where, db);
			}

			public MachineDataCollection Top(int count, WhereDelegate<MachineDataColumns> where, OrderBy<MachineDataColumns> orderBy, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.MachineData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<MachineDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.MachineData.Count(where, db);
			}
	}

	static MachineDataQueryContext _machineDatas = null!;
	static object _machineDatasLock = new object();
	public static MachineDataQueryContext MachineDatas
	{
		get
		{
			return _machineDatasLock.DoubleCheckLock<MachineDataQueryContext>(ref _machineDatas, () => new MachineDataQueryContext());
		}
	}
	public class ProcessDescriptorDataQueryContext
	{
			public ProcessDescriptorDataCollection Where(WhereDelegate<ProcessDescriptorDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.Where(where, db);
			}
		   
			public ProcessDescriptorDataCollection Where(WhereDelegate<ProcessDescriptorDataColumns> where, OrderBy<ProcessDescriptorDataColumns> orderBy = null!, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.Where(where, orderBy, db);
			}

			public ProcessDescriptorData OneWhere(WhereDelegate<ProcessDescriptorDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.OneWhere(where, db);
			}

			public static ProcessDescriptorData GetOneWhere(WhereDelegate<ProcessDescriptorDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.GetOneWhere(where, db);
			}
		
			public ProcessDescriptorData FirstOneWhere(WhereDelegate<ProcessDescriptorDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.FirstOneWhere(where, db);
			}

			public ProcessDescriptorDataCollection Top(int count, WhereDelegate<ProcessDescriptorDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.Top(count, where, db);
			}

			public ProcessDescriptorDataCollection Top(int count, WhereDelegate<ProcessDescriptorDataColumns> where, OrderBy<ProcessDescriptorDataColumns> orderBy, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.Top(count, where, orderBy, db);
			}

			public long Count(WhereDelegate<ProcessDescriptorDataColumns> where, Database db = null!)
			{
				return Bam.Protocol.Data.Common.Dao.ProcessDescriptorData.Count(where, db);
			}
	}

	static ProcessDescriptorDataQueryContext _processDescriptorDatas = null!;
	static object _processDescriptorDatasLock = new object();
	public static ProcessDescriptorDataQueryContext ProcessDescriptorDatas
	{
		get
		{
			return _processDescriptorDatasLock.DoubleCheckLock<ProcessDescriptorDataQueryContext>(ref _processDescriptorDatas, () => new ProcessDescriptorDataQueryContext());
		}
	}
    }
}																								
