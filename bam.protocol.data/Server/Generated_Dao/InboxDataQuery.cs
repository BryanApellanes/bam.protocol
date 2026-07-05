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
    public class InboxDataQuery: Query<InboxDataColumns, InboxData>
    { 
		public InboxDataQuery(){}
		public InboxDataQuery(WhereDelegate<InboxDataColumns> where, OrderBy<InboxDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public InboxDataQuery(Func<InboxDataColumns, QueryFilter<InboxDataColumns>> where, OrderBy<InboxDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public InboxDataQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static InboxDataQuery Where(WhereDelegate<InboxDataColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static InboxDataQuery Where(WhereDelegate<InboxDataColumns> where, OrderBy<InboxDataColumns> orderBy = null!, Database db = null!)
        {
            return new InboxDataQuery(where, orderBy, db);
        }

		public InboxDataCollection Execute()
		{
			return new InboxDataCollection(this, true);
		}
    }
}