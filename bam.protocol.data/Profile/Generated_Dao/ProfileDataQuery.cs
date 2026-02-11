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
    public class ProfileDataQuery: Query<ProfileDataColumns, ProfileData>
    { 
		public ProfileDataQuery(){}
		public ProfileDataQuery(WhereDelegate<ProfileDataColumns> where, OrderBy<ProfileDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public ProfileDataQuery(Func<ProfileDataColumns, QueryFilter<ProfileDataColumns>> where, OrderBy<ProfileDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public ProfileDataQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static ProfileDataQuery Where(WhereDelegate<ProfileDataColumns> where)
        {
            return Where(where, null, null);
        }

        public static ProfileDataQuery Where(WhereDelegate<ProfileDataColumns> where, OrderBy<ProfileDataColumns> orderBy = null, Database db = null)
        {
            return new ProfileDataQuery(where, orderBy, db);
        }

		public ProfileDataCollection Execute()
		{
			return new ProfileDataCollection(this, true);
		}
    }
}