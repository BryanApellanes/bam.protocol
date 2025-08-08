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
    public class ProfileQuery: Query<ProfileColumns, Profile>
    { 
		public ProfileQuery(){}
		public ProfileQuery(WhereDelegate<ProfileColumns> where, OrderBy<ProfileColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public ProfileQuery(Func<ProfileColumns, QueryFilter<ProfileColumns>> where, OrderBy<ProfileColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public ProfileQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static ProfileQuery Where(WhereDelegate<ProfileColumns> where)
        {
            return Where(where, null, null);
        }

        public static ProfileQuery Where(WhereDelegate<ProfileColumns> where, OrderBy<ProfileColumns> orderBy = null, Database db = null)
        {
            return new ProfileQuery(where, orderBy, db);
        }

		public ProfileCollection Execute()
		{
			return new ProfileCollection(this, true);
		}
    }
}