using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Private.Dao
{
    public class PrivateKeySetDataCollection: DaoCollection<PrivateKeySetDataColumns, PrivateKeySetData>
    { 
		public PrivateKeySetDataCollection(){}
		public PrivateKeySetDataCollection(IDatabase db, DataTable table, IDao dao = null!, string rc = null!) : base(db, table, dao, rc) { }
		public PrivateKeySetDataCollection(DataTable table, IDao dao = null!, string rc = null!) : base(table, dao, rc) { }
		public PrivateKeySetDataCollection(IQuery<PrivateKeySetDataColumns, PrivateKeySetData> q, Bam.Data.Dao dao = null!, string rc = null!) : base(q, dao, rc) { }
		public PrivateKeySetDataCollection(IDatabase db, IQuery<PrivateKeySetDataColumns, PrivateKeySetData> q, bool load) : base(db, q, load) { }
		public PrivateKeySetDataCollection(IQuery<PrivateKeySetDataColumns, PrivateKeySetData> q, bool load) : base(q, load) { }
    }
}