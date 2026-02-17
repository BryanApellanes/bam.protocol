using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class GroupAdditionalPropertiesCollection: DaoCollection<GroupAdditionalPropertiesColumns, GroupAdditionalProperties>
    { 
		public GroupAdditionalPropertiesCollection(){}
		public GroupAdditionalPropertiesCollection(IDatabase db, DataTable table, IDao dao = null!, string rc = null!) : base(db, table, dao, rc) { }
		public GroupAdditionalPropertiesCollection(DataTable table, IDao dao = null!, string rc = null!) : base(table, dao, rc) { }
		public GroupAdditionalPropertiesCollection(IQuery<GroupAdditionalPropertiesColumns, GroupAdditionalProperties> q, Bam.Data.Dao dao = null!, string rc = null!) : base(q, dao, rc) { }
		public GroupAdditionalPropertiesCollection(IDatabase db, IQuery<GroupAdditionalPropertiesColumns, GroupAdditionalProperties> q, bool load) : base(db, q, load) { }
		public GroupAdditionalPropertiesCollection(IQuery<GroupAdditionalPropertiesColumns, GroupAdditionalProperties> q, bool load) : base(q, load) { }
    }
}