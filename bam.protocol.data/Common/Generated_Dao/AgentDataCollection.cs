using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Common.Dao
{
    public class AgentDataCollection: DaoCollection<AgentDataColumns, AgentData>
    { 
		public AgentDataCollection(){}
		public AgentDataCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public AgentDataCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public AgentDataCollection(IQuery<AgentDataColumns, AgentData> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public AgentDataCollection(IDatabase db, IQuery<AgentDataColumns, AgentData> q, bool load) : base(db, q, load) { }
		public AgentDataCollection(IQuery<AgentDataColumns, AgentData> q, bool load) : base(q, load) { }
    }
}