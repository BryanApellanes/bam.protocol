using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class DeviceCollection: DaoCollection<DeviceColumns, Device>
    { 
		public DeviceCollection(){}
		public DeviceCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public DeviceCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public DeviceCollection(IQuery<DeviceColumns, Device> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public DeviceCollection(IDatabase db, IQuery<DeviceColumns, Device> q, bool load) : base(db, q, load) { }
		public DeviceCollection(IQuery<DeviceColumns, Device> q, bool load) : base(q, load) { }
    }
}