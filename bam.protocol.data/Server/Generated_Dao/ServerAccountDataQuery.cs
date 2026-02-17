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
    public class ServerAccountDataQuery: Query<ServerAccountDataColumns, ServerAccountData>
    { 
		public ServerAccountDataQuery(){}
		public ServerAccountDataQuery(WhereDelegate<ServerAccountDataColumns> where, OrderBy<ServerAccountDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public ServerAccountDataQuery(Func<ServerAccountDataColumns, QueryFilter<ServerAccountDataColumns>> where, OrderBy<ServerAccountDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public ServerAccountDataQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static ServerAccountDataQuery Where(WhereDelegate<ServerAccountDataColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static ServerAccountDataQuery Where(WhereDelegate<ServerAccountDataColumns> where, OrderBy<ServerAccountDataColumns> orderBy = null!, Database db = null!)
        {
            return new ServerAccountDataQuery(where, orderBy, db);
        }

		public ServerAccountDataCollection Execute()
		{
			return new ServerAccountDataCollection(this, true);
		}
    }
}