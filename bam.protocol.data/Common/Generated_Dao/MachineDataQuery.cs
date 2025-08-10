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
    public class MachineDataQuery: Query<MachineDataColumns, MachineData>
    { 
		public MachineDataQuery(){}
		public MachineDataQuery(WhereDelegate<MachineDataColumns> where, OrderBy<MachineDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public MachineDataQuery(Func<MachineDataColumns, QueryFilter<MachineDataColumns>> where, OrderBy<MachineDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public MachineDataQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static MachineDataQuery Where(WhereDelegate<MachineDataColumns> where)
        {
            return Where(where, null, null);
        }

        public static MachineDataQuery Where(WhereDelegate<MachineDataColumns> where, OrderBy<MachineDataColumns> orderBy = null, Database db = null)
        {
            return new MachineDataQuery(where, orderBy, db);
        }

		public MachineDataCollection Execute()
		{
			return new MachineDataCollection(this, true);
		}
    }
}