using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Common.Dao
{
    public class DeviceDataCollection: DaoCollection<DeviceDataColumns, DeviceData>
    { 
		public DeviceDataCollection(){}
		public DeviceDataCollection(IDatabase db, DataTable table, IDao dao = null!, string rc = null!) : base(db, table, dao, rc) { }
		public DeviceDataCollection(DataTable table, IDao dao = null!, string rc = null!) : base(table, dao, rc) { }
		public DeviceDataCollection(IQuery<DeviceDataColumns, DeviceData> q, Bam.Data.Dao dao = null!, string rc = null!) : base(q, dao, rc) { }
		public DeviceDataCollection(IDatabase db, IQuery<DeviceDataColumns, DeviceData> q, bool load) : base(db, q, load) { }
		public DeviceDataCollection(IQuery<DeviceDataColumns, DeviceData> q, bool load) : base(q, load) { }
    }
}