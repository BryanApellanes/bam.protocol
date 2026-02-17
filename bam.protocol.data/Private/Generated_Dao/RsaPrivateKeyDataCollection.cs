using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Private.Dao
{
    public class RsaPrivateKeyDataCollection: DaoCollection<RsaPrivateKeyDataColumns, RsaPrivateKeyData>
    { 
		public RsaPrivateKeyDataCollection(){}
		public RsaPrivateKeyDataCollection(IDatabase db, DataTable table, IDao dao = null!, string rc = null!) : base(db, table, dao, rc) { }
		public RsaPrivateKeyDataCollection(DataTable table, IDao dao = null!, string rc = null!) : base(table, dao, rc) { }
		public RsaPrivateKeyDataCollection(IQuery<RsaPrivateKeyDataColumns, RsaPrivateKeyData> q, Bam.Data.Dao dao = null!, string rc = null!) : base(q, dao, rc) { }
		public RsaPrivateKeyDataCollection(IDatabase db, IQuery<RsaPrivateKeyDataColumns, RsaPrivateKeyData> q, bool load) : base(db, q, load) { }
		public RsaPrivateKeyDataCollection(IQuery<RsaPrivateKeyDataColumns, RsaPrivateKeyData> q, bool load) : base(q, load) { }
    }
}