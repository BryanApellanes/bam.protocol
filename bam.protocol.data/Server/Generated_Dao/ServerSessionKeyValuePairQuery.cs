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
    public class ServerSessionKeyValuePairQuery: Query<ServerSessionKeyValuePairColumns, ServerSessionKeyValuePair>
    { 
		public ServerSessionKeyValuePairQuery(){}
		public ServerSessionKeyValuePairQuery(WhereDelegate<ServerSessionKeyValuePairColumns> where, OrderBy<ServerSessionKeyValuePairColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public ServerSessionKeyValuePairQuery(Func<ServerSessionKeyValuePairColumns, QueryFilter<ServerSessionKeyValuePairColumns>> where, OrderBy<ServerSessionKeyValuePairColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public ServerSessionKeyValuePairQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static ServerSessionKeyValuePairQuery Where(WhereDelegate<ServerSessionKeyValuePairColumns> where)
        {
            return Where(where, null, null);
        }

        public static ServerSessionKeyValuePairQuery Where(WhereDelegate<ServerSessionKeyValuePairColumns> where, OrderBy<ServerSessionKeyValuePairColumns> orderBy = null, Database db = null)
        {
            return new ServerSessionKeyValuePairQuery(where, orderBy, db);
        }

		public ServerSessionKeyValuePairCollection Execute()
		{
			return new ServerSessionKeyValuePairCollection(this, true);
		}
    }
}