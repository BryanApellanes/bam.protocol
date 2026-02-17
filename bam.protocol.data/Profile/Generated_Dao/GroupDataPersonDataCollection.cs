using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class GroupDataPersonDataCollection: DaoCollection<GroupDataPersonDataColumns, GroupDataPersonData>
    { 
		public GroupDataPersonDataCollection(){}
		public GroupDataPersonDataCollection(IDatabase db, DataTable table, IDao dao = null!, string rc = null!) : base(db, table, dao, rc) { }
		public GroupDataPersonDataCollection(DataTable table, IDao dao = null!, string rc = null!) : base(table, dao, rc) { }
		public GroupDataPersonDataCollection(IQuery<GroupDataPersonDataColumns, GroupDataPersonData> q, Bam.Data.Dao dao = null!, string rc = null!) : base(q, dao, rc) { }
		public GroupDataPersonDataCollection(IDatabase db, IQuery<GroupDataPersonDataColumns, GroupDataPersonData> q, bool load) : base(db, q, load) { }
		public GroupDataPersonDataCollection(IQuery<GroupDataPersonDataColumns, GroupDataPersonData> q, bool load) : base(q, load) { }
    }
}