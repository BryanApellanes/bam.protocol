/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Common.Dao
{
    public class ActorDataQuery: Query<ActorDataColumns, ActorData>
    { 
		public ActorDataQuery(){}
		public ActorDataQuery(WhereDelegate<ActorDataColumns> where, OrderBy<ActorDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public ActorDataQuery(Func<ActorDataColumns, QueryFilter<ActorDataColumns>> where, OrderBy<ActorDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public ActorDataQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static ActorDataQuery Where(WhereDelegate<ActorDataColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static ActorDataQuery Where(WhereDelegate<ActorDataColumns> where, OrderBy<ActorDataColumns> orderBy = null!, Database db = null!)
        {
            return new ActorDataQuery(where, orderBy, db);
        }

		public ActorDataCollection Execute()
		{
			return new ActorDataCollection(this, true);
		}
    }
}