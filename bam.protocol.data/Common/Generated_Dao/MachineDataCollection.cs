using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Common.Dao
{
    public class MachineDataCollection: DaoCollection<MachineDataColumns, MachineData>
    { 
		public MachineDataCollection(){}
		public MachineDataCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public MachineDataCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public MachineDataCollection(IQuery<MachineDataColumns, MachineData> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public MachineDataCollection(IDatabase db, IQuery<MachineDataColumns, MachineData> q, bool load) : base(db, q, load) { }
		public MachineDataCollection(IQuery<MachineDataColumns, MachineData> q, bool load) : base(q, load) { }
    }
}