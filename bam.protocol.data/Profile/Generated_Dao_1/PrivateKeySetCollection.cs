using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class PrivateKeySetCollection: DaoCollection<PrivateKeySetColumns, PrivateKeySet>
    { 
		public PrivateKeySetCollection(){}
		public PrivateKeySetCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public PrivateKeySetCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public PrivateKeySetCollection(IQuery<PrivateKeySetColumns, PrivateKeySet> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public PrivateKeySetCollection(IDatabase db, IQuery<PrivateKeySetColumns, PrivateKeySet> q, bool load) : base(db, q, load) { }
		public PrivateKeySetCollection(IQuery<PrivateKeySetColumns, PrivateKeySet> q, bool load) : base(q, load) { }
    }
}