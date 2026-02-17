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
    public class GroupDataPersonDataQuery: Query<GroupDataPersonDataColumns, GroupDataPersonData>
    { 
		public GroupDataPersonDataQuery(){}
		public GroupDataPersonDataQuery(WhereDelegate<GroupDataPersonDataColumns> where, OrderBy<GroupDataPersonDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public GroupDataPersonDataQuery(Func<GroupDataPersonDataColumns, QueryFilter<GroupDataPersonDataColumns>> where, OrderBy<GroupDataPersonDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public GroupDataPersonDataQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static GroupDataPersonDataQuery Where(WhereDelegate<GroupDataPersonDataColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static GroupDataPersonDataQuery Where(WhereDelegate<GroupDataPersonDataColumns> where, OrderBy<GroupDataPersonDataColumns> orderBy = null!, Database db = null!)
        {
            return new GroupDataPersonDataQuery(where, orderBy, db);
        }

		public GroupDataPersonDataCollection Execute()
		{
			return new GroupDataPersonDataCollection(this, true);
		}
    }
}