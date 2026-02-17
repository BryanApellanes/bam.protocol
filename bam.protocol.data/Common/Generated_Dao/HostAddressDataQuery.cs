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
    public class HostAddressDataQuery: Query<HostAddressDataColumns, HostAddressData>
    { 
		public HostAddressDataQuery(){}
		public HostAddressDataQuery(WhereDelegate<HostAddressDataColumns> where, OrderBy<HostAddressDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public HostAddressDataQuery(Func<HostAddressDataColumns, QueryFilter<HostAddressDataColumns>> where, OrderBy<HostAddressDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public HostAddressDataQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static HostAddressDataQuery Where(WhereDelegate<HostAddressDataColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static HostAddressDataQuery Where(WhereDelegate<HostAddressDataColumns> where, OrderBy<HostAddressDataColumns> orderBy = null!, Database db = null!)
        {
            return new HostAddressDataQuery(where, orderBy, db);
        }

		public HostAddressDataCollection Execute()
		{
			return new HostAddressDataCollection(this, true);
		}
    }
}