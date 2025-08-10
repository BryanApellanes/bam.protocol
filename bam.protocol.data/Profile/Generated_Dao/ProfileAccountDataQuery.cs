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
    public class ProfileAccountDataQuery: Query<ProfileAccountDataColumns, ProfileAccountData>
    { 
		public ProfileAccountDataQuery(){}
		public ProfileAccountDataQuery(WhereDelegate<ProfileAccountDataColumns> where, OrderBy<ProfileAccountDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public ProfileAccountDataQuery(Func<ProfileAccountDataColumns, QueryFilter<ProfileAccountDataColumns>> where, OrderBy<ProfileAccountDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public ProfileAccountDataQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static ProfileAccountDataQuery Where(WhereDelegate<ProfileAccountDataColumns> where)
        {
            return Where(where, null, null);
        }

        public static ProfileAccountDataQuery Where(WhereDelegate<ProfileAccountDataColumns> where, OrderBy<ProfileAccountDataColumns> orderBy = null, Database db = null)
        {
            return new ProfileAccountDataQuery(where, orderBy, db);
        }

		public ProfileAccountDataCollection Execute()
		{
			return new ProfileAccountDataCollection(this, true);
		}
    }
}