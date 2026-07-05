using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Server.Dao
{
    public class InboxDataCollection: DaoCollection<InboxDataColumns, InboxData>
    { 
		public InboxDataCollection(){}
		public InboxDataCollection(IDatabase db, DataTable table, IDao dao = null!, string rc = null!) : base(db, table, dao, rc) { }
		public InboxDataCollection(DataTable table, IDao dao = null!, string rc = null!) : base(table, dao, rc) { }
		public InboxDataCollection(IQuery<InboxDataColumns, InboxData> q, Bam.Data.Dao dao = null!, string rc = null!) : base(q, dao, rc) { }
		public InboxDataCollection(IDatabase db, IQuery<InboxDataColumns, InboxData> q, bool load) : base(db, q, load) { }
		public InboxDataCollection(IQuery<InboxDataColumns, InboxData> q, bool load) : base(q, load) { }
    }
}