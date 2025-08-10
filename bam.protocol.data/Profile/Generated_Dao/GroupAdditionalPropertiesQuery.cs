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
    public class GroupAdditionalPropertiesQuery: Query<GroupAdditionalPropertiesColumns, GroupAdditionalProperties>
    { 
		public GroupAdditionalPropertiesQuery(){}
		public GroupAdditionalPropertiesQuery(WhereDelegate<GroupAdditionalPropertiesColumns> where, OrderBy<GroupAdditionalPropertiesColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public GroupAdditionalPropertiesQuery(Func<GroupAdditionalPropertiesColumns, QueryFilter<GroupAdditionalPropertiesColumns>> where, OrderBy<GroupAdditionalPropertiesColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public GroupAdditionalPropertiesQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static GroupAdditionalPropertiesQuery Where(WhereDelegate<GroupAdditionalPropertiesColumns> where)
        {
            return Where(where, null, null);
        }

        public static GroupAdditionalPropertiesQuery Where(WhereDelegate<GroupAdditionalPropertiesColumns> where, OrderBy<GroupAdditionalPropertiesColumns> orderBy = null, Database db = null)
        {
            return new GroupAdditionalPropertiesQuery(where, orderBy, db);
        }

		public GroupAdditionalPropertiesCollection Execute()
		{
			return new GroupAdditionalPropertiesCollection(this, true);
		}
    }
}