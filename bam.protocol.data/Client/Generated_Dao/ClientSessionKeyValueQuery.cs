/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Client.Dao
{
    public class ClientSessionKeyValueQuery: Query<ClientSessionKeyValueColumns, ClientSessionKeyValue>
    { 
		public ClientSessionKeyValueQuery(){}
		public ClientSessionKeyValueQuery(WhereDelegate<ClientSessionKeyValueColumns> where, OrderBy<ClientSessionKeyValueColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public ClientSessionKeyValueQuery(Func<ClientSessionKeyValueColumns, QueryFilter<ClientSessionKeyValueColumns>> where, OrderBy<ClientSessionKeyValueColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public ClientSessionKeyValueQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static ClientSessionKeyValueQuery Where(WhereDelegate<ClientSessionKeyValueColumns> where)
        {
            return Where(where, null, null);
        }

        public static ClientSessionKeyValueQuery Where(WhereDelegate<ClientSessionKeyValueColumns> where, OrderBy<ClientSessionKeyValueColumns> orderBy = null, Database db = null)
        {
            return new ClientSessionKeyValueQuery(where, orderBy, db);
        }

		public ClientSessionKeyValueCollection Execute()
		{
			return new ClientSessionKeyValueCollection(this, true);
		}
    }
}