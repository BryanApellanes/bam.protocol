/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Private.Dao
{
    public class PrivateKeySetDataQuery: Query<PrivateKeySetDataColumns, PrivateKeySetData>
    { 
		public PrivateKeySetDataQuery(){}
		public PrivateKeySetDataQuery(WhereDelegate<PrivateKeySetDataColumns> where, OrderBy<PrivateKeySetDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public PrivateKeySetDataQuery(Func<PrivateKeySetDataColumns, QueryFilter<PrivateKeySetDataColumns>> where, OrderBy<PrivateKeySetDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public PrivateKeySetDataQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static PrivateKeySetDataQuery Where(WhereDelegate<PrivateKeySetDataColumns> where)
        {
            return Where(where, null, null);
        }

        public static PrivateKeySetDataQuery Where(WhereDelegate<PrivateKeySetDataColumns> where, OrderBy<PrivateKeySetDataColumns> orderBy = null, Database db = null)
        {
            return new PrivateKeySetDataQuery(where, orderBy, db);
        }

		public PrivateKeySetDataCollection Execute()
		{
			return new PrivateKeySetDataCollection(this, true);
		}
    }
}