using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class PublicKeySetDataCollection: DaoCollection<PublicKeySetDataColumns, PublicKeySetData>
    { 
		public PublicKeySetDataCollection(){}
		public PublicKeySetDataCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public PublicKeySetDataCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public PublicKeySetDataCollection(IQuery<PublicKeySetDataColumns, PublicKeySetData> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public PublicKeySetDataCollection(IDatabase db, IQuery<PublicKeySetDataColumns, PublicKeySetData> q, bool load) : base(db, q, load) { }
		public PublicKeySetDataCollection(IQuery<PublicKeySetDataColumns, PublicKeySetData> q, bool load) : base(q, load) { }
    }
}