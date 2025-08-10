using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class AgentAdditionalPropertiesCollection: DaoCollection<AgentAdditionalPropertiesColumns, AgentAdditionalProperties>
    { 
		public AgentAdditionalPropertiesCollection(){}
		public AgentAdditionalPropertiesCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public AgentAdditionalPropertiesCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public AgentAdditionalPropertiesCollection(IQuery<AgentAdditionalPropertiesColumns, AgentAdditionalProperties> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public AgentAdditionalPropertiesCollection(IDatabase db, IQuery<AgentAdditionalPropertiesColumns, AgentAdditionalProperties> q, bool load) : base(db, q, load) { }
		public AgentAdditionalPropertiesCollection(IQuery<AgentAdditionalPropertiesColumns, AgentAdditionalProperties> q, bool load) : base(q, load) { }
    }
}