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
    public class ServerSessionKeyValueQuery: Query<ServerSessionKeyValueColumns, ServerSessionKeyValue>
    { 
		public ServerSessionKeyValueQuery(){}
		public ServerSessionKeyValueQuery(WhereDelegate<ServerSessionKeyValueColumns> where, OrderBy<ServerSessionKeyValueColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public ServerSessionKeyValueQuery(Func<ServerSessionKeyValueColumns, QueryFilter<ServerSessionKeyValueColumns>> where, OrderBy<ServerSessionKeyValueColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public ServerSessionKeyValueQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static ServerSessionKeyValueQuery Where(WhereDelegate<ServerSessionKeyValueColumns> where)
        {
            return Where(where, null, null);
        }

        public static ServerSessionKeyValueQuery Where(WhereDelegate<ServerSessionKeyValueColumns> where, OrderBy<ServerSessionKeyValueColumns> orderBy = null, Database db = null)
        {
            return new ServerSessionKeyValueQuery(where, orderBy, db);
        }

		public ServerSessionKeyValueCollection Execute()
		{
			return new ServerSessionKeyValueCollection(this, true);
		}
    }
}