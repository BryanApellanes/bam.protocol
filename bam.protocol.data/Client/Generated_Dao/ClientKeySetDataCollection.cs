using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Client.Dao
{
    public class ClientKeySetDataCollection: DaoCollection<ClientKeySetDataColumns, ClientKeySetData>
    { 
		public ClientKeySetDataCollection(){}
		public ClientKeySetDataCollection(IDatabase db, DataTable table, IDao dao = null!, string rc = null!) : base(db, table, dao, rc) { }
		public ClientKeySetDataCollection(DataTable table, IDao dao = null!, string rc = null!) : base(table, dao, rc) { }
		public ClientKeySetDataCollection(IQuery<ClientKeySetDataColumns, ClientKeySetData> q, Bam.Data.Dao dao = null!, string rc = null!) : base(q, dao, rc) { }
		public ClientKeySetDataCollection(IDatabase db, IQuery<ClientKeySetDataColumns, ClientKeySetData> q, bool load) : base(db, q, load) { }
		public ClientKeySetDataCollection(IQuery<ClientKeySetDataColumns, ClientKeySetData> q, bool load) : base(q, load) { }
    }
}