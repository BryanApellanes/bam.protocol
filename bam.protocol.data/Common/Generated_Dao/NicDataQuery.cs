/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Common.Dao
{
    public class NicDataQuery: Query<NicDataColumns, NicData>
    { 
		public NicDataQuery(){}
		public NicDataQuery(WhereDelegate<NicDataColumns> where, OrderBy<NicDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public NicDataQuery(Func<NicDataColumns, QueryFilter<NicDataColumns>> where, OrderBy<NicDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public NicDataQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static NicDataQuery Where(WhereDelegate<NicDataColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static NicDataQuery Where(WhereDelegate<NicDataColumns> where, OrderBy<NicDataColumns> orderBy = null!, Database db = null!)
        {
            return new NicDataQuery(where, orderBy, db);
        }

		public NicDataCollection Execute()
		{
			return new NicDataCollection(this, true);
		}
    }
}