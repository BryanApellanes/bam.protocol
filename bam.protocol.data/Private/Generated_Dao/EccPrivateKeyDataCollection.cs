using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Private.Dao
{
    public class EccPrivateKeyDataCollection: DaoCollection<EccPrivateKeyDataColumns, EccPrivateKeyData>
    { 
		public EccPrivateKeyDataCollection(){}
		public EccPrivateKeyDataCollection(IDatabase db, DataTable table, IDao dao = null!, string rc = null!) : base(db, table, dao, rc) { }
		public EccPrivateKeyDataCollection(DataTable table, IDao dao = null!, string rc = null!) : base(table, dao, rc) { }
		public EccPrivateKeyDataCollection(IQuery<EccPrivateKeyDataColumns, EccPrivateKeyData> q, Bam.Data.Dao dao = null!, string rc = null!) : base(q, dao, rc) { }
		public EccPrivateKeyDataCollection(IDatabase db, IQuery<EccPrivateKeyDataColumns, EccPrivateKeyData> q, bool load) : base(db, q, load) { }
		public EccPrivateKeyDataCollection(IQuery<EccPrivateKeyDataColumns, EccPrivateKeyData> q, bool load) : base(q, load) { }
    }
}