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
    public class ClientSessionDataQuery: Query<ClientSessionDataColumns, ClientSessionData>
    { 
		public ClientSessionDataQuery(){}
		public ClientSessionDataQuery(WhereDelegate<ClientSessionDataColumns> where, OrderBy<ClientSessionDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public ClientSessionDataQuery(Func<ClientSessionDataColumns, QueryFilter<ClientSessionDataColumns>> where, OrderBy<ClientSessionDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public ClientSessionDataQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static ClientSessionDataQuery Where(WhereDelegate<ClientSessionDataColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static ClientSessionDataQuery Where(WhereDelegate<ClientSessionDataColumns> where, OrderBy<ClientSessionDataColumns> orderBy = null!, Database db = null!)
        {
            return new ClientSessionDataQuery(where, orderBy, db);
        }

		public ClientSessionDataCollection Execute()
		{
			return new ClientSessionDataCollection(this, true);
		}
    }
}