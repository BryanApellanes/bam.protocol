/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Server.Dao
{
    public class OutboxDataQuery: Query<OutboxDataColumns, OutboxData>
    { 
		public OutboxDataQuery(){}
		public OutboxDataQuery(WhereDelegate<OutboxDataColumns> where, OrderBy<OutboxDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public OutboxDataQuery(Func<OutboxDataColumns, QueryFilter<OutboxDataColumns>> where, OrderBy<OutboxDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public OutboxDataQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static OutboxDataQuery Where(WhereDelegate<OutboxDataColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static OutboxDataQuery Where(WhereDelegate<OutboxDataColumns> where, OrderBy<OutboxDataColumns> orderBy = null!, Database db = null!)
        {
            return new OutboxDataQuery(where, orderBy, db);
        }

		public OutboxDataCollection Execute()
		{
			return new OutboxDataCollection(this, true);
		}
    }
}