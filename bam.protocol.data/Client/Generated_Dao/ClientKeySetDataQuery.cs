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
    public class ClientKeySetDataQuery: Query<ClientKeySetDataColumns, ClientKeySetData>
    { 
		public ClientKeySetDataQuery(){}
		public ClientKeySetDataQuery(WhereDelegate<ClientKeySetDataColumns> where, OrderBy<ClientKeySetDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public ClientKeySetDataQuery(Func<ClientKeySetDataColumns, QueryFilter<ClientKeySetDataColumns>> where, OrderBy<ClientKeySetDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public ClientKeySetDataQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static ClientKeySetDataQuery Where(WhereDelegate<ClientKeySetDataColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static ClientKeySetDataQuery Where(WhereDelegate<ClientKeySetDataColumns> where, OrderBy<ClientKeySetDataColumns> orderBy = null!, Database db = null!)
        {
            return new ClientKeySetDataQuery(where, orderBy, db);
        }

		public ClientKeySetDataCollection Execute()
		{
			return new ClientKeySetDataCollection(this, true);
		}
    }
}