using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Client.Dao
{
    public class ClientSessionKeyValueCollection: DaoCollection<ClientSessionKeyValueColumns, ClientSessionKeyValue>
    { 
		public ClientSessionKeyValueCollection(){}
		public ClientSessionKeyValueCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public ClientSessionKeyValueCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public ClientSessionKeyValueCollection(IQuery<ClientSessionKeyValueColumns, ClientSessionKeyValue> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public ClientSessionKeyValueCollection(IDatabase db, IQuery<ClientSessionKeyValueColumns, ClientSessionKeyValue> q, bool load) : base(db, q, load) { }
		public ClientSessionKeyValueCollection(IQuery<ClientSessionKeyValueColumns, ClientSessionKeyValue> q, bool load) : base(q, load) { }
    }
}