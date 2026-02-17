using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Common.Dao
{
    public class NicDataCollection: DaoCollection<NicDataColumns, NicData>
    { 
		public NicDataCollection(){}
		public NicDataCollection(IDatabase db, DataTable table, IDao dao = null!, string rc = null!) : base(db, table, dao, rc) { }
		public NicDataCollection(DataTable table, IDao dao = null!, string rc = null!) : base(table, dao, rc) { }
		public NicDataCollection(IQuery<NicDataColumns, NicData> q, Bam.Data.Dao dao = null!, string rc = null!) : base(q, dao, rc) { }
		public NicDataCollection(IDatabase db, IQuery<NicDataColumns, NicData> q, bool load) : base(db, q, load) { }
		public NicDataCollection(IQuery<NicDataColumns, NicData> q, bool load) : base(q, load) { }
    }
}