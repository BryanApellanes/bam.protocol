using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Client.Dao
{
    public class ClientSessionDataCollection: DaoCollection<ClientSessionDataColumns, ClientSessionData>
    { 
		public ClientSessionDataCollection(){}
		public ClientSessionDataCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public ClientSessionDataCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public ClientSessionDataCollection(IQuery<ClientSessionDataColumns, ClientSessionData> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public ClientSessionDataCollection(IDatabase db, IQuery<ClientSessionDataColumns, ClientSessionData> q, bool load) : base(db, q, load) { }
		public ClientSessionDataCollection(IQuery<ClientSessionDataColumns, ClientSessionData> q, bool load) : base(q, load) { }
    }
}