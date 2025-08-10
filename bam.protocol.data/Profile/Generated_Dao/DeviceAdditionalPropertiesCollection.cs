using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class DeviceAdditionalPropertiesCollection: DaoCollection<DeviceAdditionalPropertiesColumns, DeviceAdditionalProperties>
    { 
		public DeviceAdditionalPropertiesCollection(){}
		public DeviceAdditionalPropertiesCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public DeviceAdditionalPropertiesCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public DeviceAdditionalPropertiesCollection(IQuery<DeviceAdditionalPropertiesColumns, DeviceAdditionalProperties> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public DeviceAdditionalPropertiesCollection(IDatabase db, IQuery<DeviceAdditionalPropertiesColumns, DeviceAdditionalProperties> q, bool load) : base(db, q, load) { }
		public DeviceAdditionalPropertiesCollection(IQuery<DeviceAdditionalPropertiesColumns, DeviceAdditionalProperties> q, bool load) : base(q, load) { }
    }
}