/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class GroupDataQuery: Query<GroupDataColumns, GroupData>
    { 
		public GroupDataQuery(){}
		public GroupDataQuery(WhereDelegate<GroupDataColumns> where, OrderBy<GroupDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public GroupDataQuery(Func<GroupDataColumns, QueryFilter<GroupDataColumns>> where, OrderBy<GroupDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public GroupDataQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static GroupDataQuery Where(WhereDelegate<GroupDataColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static GroupDataQuery Where(WhereDelegate<GroupDataColumns> where, OrderBy<GroupDataColumns> orderBy = null!, Database db = null!)
        {
            return new GroupDataQuery(where, orderBy, db);
        }

		public GroupDataCollection Execute()
		{
			return new GroupDataCollection(this, true);
		}
    }
}