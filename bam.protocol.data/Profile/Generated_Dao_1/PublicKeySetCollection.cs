using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class PublicKeySetCollection: DaoCollection<PublicKeySetColumns, PublicKeySet>
    { 
		public PublicKeySetCollection(){}
		public PublicKeySetCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public PublicKeySetCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public PublicKeySetCollection(IQuery<PublicKeySetColumns, PublicKeySet> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public PublicKeySetCollection(IDatabase db, IQuery<PublicKeySetColumns, PublicKeySet> q, bool load) : base(db, q, load) { }
		public PublicKeySetCollection(IQuery<PublicKeySetColumns, PublicKeySet> q, bool load) : base(q, load) { }
    }
}