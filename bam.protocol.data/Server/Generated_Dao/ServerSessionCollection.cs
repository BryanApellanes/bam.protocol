using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Server.Dao
{
    public class ServerSessionCollection: DaoCollection<ServerSessionColumns, ServerSession>
    { 
		public ServerSessionCollection(){}
		public ServerSessionCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public ServerSessionCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public ServerSessionCollection(IQuery<ServerSessionColumns, ServerSession> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public ServerSessionCollection(IDatabase db, IQuery<ServerSessionColumns, ServerSession> q, bool load) : base(db, q, load) { }
		public ServerSessionCollection(IQuery<ServerSessionColumns, ServerSession> q, bool load) : base(q, load) { }
    }
}