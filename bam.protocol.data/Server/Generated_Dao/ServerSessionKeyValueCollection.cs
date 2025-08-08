using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Server.Dao
{
    public class ServerSessionKeyValueCollection: DaoCollection<ServerSessionKeyValueColumns, ServerSessionKeyValue>
    { 
		public ServerSessionKeyValueCollection(){}
		public ServerSessionKeyValueCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public ServerSessionKeyValueCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public ServerSessionKeyValueCollection(IQuery<ServerSessionKeyValueColumns, ServerSessionKeyValue> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public ServerSessionKeyValueCollection(IDatabase db, IQuery<ServerSessionKeyValueColumns, ServerSessionKeyValue> q, bool load) : base(db, q, load) { }
		public ServerSessionKeyValueCollection(IQuery<ServerSessionKeyValueColumns, ServerSessionKeyValue> q, bool load) : base(q, load) { }
    }
}