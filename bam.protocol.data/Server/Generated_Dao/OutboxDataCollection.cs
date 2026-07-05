using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Server.Dao
{
    public class OutboxDataCollection: DaoCollection<OutboxDataColumns, OutboxData>
    { 
		public OutboxDataCollection(){}
		public OutboxDataCollection(IDatabase db, DataTable table, IDao dao = null!, string rc = null!) : base(db, table, dao, rc) { }
		public OutboxDataCollection(DataTable table, IDao dao = null!, string rc = null!) : base(table, dao, rc) { }
		public OutboxDataCollection(IQuery<OutboxDataColumns, OutboxData> q, Bam.Data.Dao dao = null!, string rc = null!) : base(q, dao, rc) { }
		public OutboxDataCollection(IDatabase db, IQuery<OutboxDataColumns, OutboxData> q, bool load) : base(db, q, load) { }
		public OutboxDataCollection(IQuery<OutboxDataColumns, OutboxData> q, bool load) : base(q, load) { }
    }
}