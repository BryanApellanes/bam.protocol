using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Server.Dao
{
    public class ServerSessionKeyValuePairCollection: DaoCollection<ServerSessionKeyValuePairColumns, ServerSessionKeyValuePair>
    { 
		public ServerSessionKeyValuePairCollection(){}
		public ServerSessionKeyValuePairCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public ServerSessionKeyValuePairCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public ServerSessionKeyValuePairCollection(IQuery<ServerSessionKeyValuePairColumns, ServerSessionKeyValuePair> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public ServerSessionKeyValuePairCollection(IDatabase db, IQuery<ServerSessionKeyValuePairColumns, ServerSessionKeyValuePair> q, bool load) : base(db, q, load) { }
		public ServerSessionKeyValuePairCollection(IQuery<ServerSessionKeyValuePairColumns, ServerSessionKeyValuePair> q, bool load) : base(q, load) { }
    }
}