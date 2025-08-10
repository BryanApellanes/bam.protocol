using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Common.Dao
{
    public class ActorDataCollection: DaoCollection<ActorDataColumns, ActorData>
    { 
		public ActorDataCollection(){}
		public ActorDataCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public ActorDataCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public ActorDataCollection(IQuery<ActorDataColumns, ActorData> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public ActorDataCollection(IDatabase db, IQuery<ActorDataColumns, ActorData> q, bool load) : base(db, q, load) { }
		public ActorDataCollection(IQuery<ActorDataColumns, ActorData> q, bool load) : base(q, load) { }
    }
}