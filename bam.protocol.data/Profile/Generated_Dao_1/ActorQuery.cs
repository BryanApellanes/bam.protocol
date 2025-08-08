/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class ActorQuery: Query<ActorColumns, Actor>
    { 
		public ActorQuery(){}
		public ActorQuery(WhereDelegate<ActorColumns> where, OrderBy<ActorColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public ActorQuery(Func<ActorColumns, QueryFilter<ActorColumns>> where, OrderBy<ActorColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public ActorQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static ActorQuery Where(WhereDelegate<ActorColumns> where)
        {
            return Where(where, null, null);
        }

        public static ActorQuery Where(WhereDelegate<ActorColumns> where, OrderBy<ActorColumns> orderBy = null, Database db = null)
        {
            return new ActorQuery(where, orderBy, db);
        }

		public ActorCollection Execute()
		{
			return new ActorCollection(this, true);
		}
    }
}