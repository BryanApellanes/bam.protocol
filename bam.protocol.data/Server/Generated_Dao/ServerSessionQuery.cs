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
    public class ServerSessionQuery: Query<ServerSessionColumns, ServerSession>
    { 
		public ServerSessionQuery(){}
		public ServerSessionQuery(WhereDelegate<ServerSessionColumns> where, OrderBy<ServerSessionColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public ServerSessionQuery(Func<ServerSessionColumns, QueryFilter<ServerSessionColumns>> where, OrderBy<ServerSessionColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public ServerSessionQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static ServerSessionQuery Where(WhereDelegate<ServerSessionColumns> where)
        {
            return Where(where, null, null);
        }

        public static ServerSessionQuery Where(WhereDelegate<ServerSessionColumns> where, OrderBy<ServerSessionColumns> orderBy = null, Database db = null)
        {
            return new ServerSessionQuery(where, orderBy, db);
        }

		public ServerSessionCollection Execute()
		{
			return new ServerSessionCollection(this, true);
		}
    }
}