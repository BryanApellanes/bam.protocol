using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class ActorCollection: DaoCollection<ActorColumns, Actor>
    { 
		public ActorCollection(){}
		public ActorCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public ActorCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public ActorCollection(IQuery<ActorColumns, Actor> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public ActorCollection(IDatabase db, IQuery<ActorColumns, Actor> q, bool load) : base(db, q, load) { }
		public ActorCollection(IQuery<ActorColumns, Actor> q, bool load) : base(q, load) { }
    }
}