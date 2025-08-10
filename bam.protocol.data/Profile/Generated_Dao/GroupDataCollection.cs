using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class GroupDataCollection: DaoCollection<GroupDataColumns, GroupData>
    { 
		public GroupDataCollection(){}
		public GroupDataCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public GroupDataCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public GroupDataCollection(IQuery<GroupDataColumns, GroupData> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public GroupDataCollection(IDatabase db, IQuery<GroupDataColumns, GroupData> q, bool load) : base(db, q, load) { }
		public GroupDataCollection(IQuery<GroupDataColumns, GroupData> q, bool load) : base(q, load) { }
    }
}