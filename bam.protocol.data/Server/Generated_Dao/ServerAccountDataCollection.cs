using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Server.Dao
{
    public class ServerAccountDataCollection: DaoCollection<ServerAccountDataColumns, ServerAccountData>
    { 
		public ServerAccountDataCollection(){}
		public ServerAccountDataCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public ServerAccountDataCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public ServerAccountDataCollection(IQuery<ServerAccountDataColumns, ServerAccountData> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public ServerAccountDataCollection(IDatabase db, IQuery<ServerAccountDataColumns, ServerAccountData> q, bool load) : base(db, q, load) { }
		public ServerAccountDataCollection(IQuery<ServerAccountDataColumns, ServerAccountData> q, bool load) : base(q, load) { }
    }
}