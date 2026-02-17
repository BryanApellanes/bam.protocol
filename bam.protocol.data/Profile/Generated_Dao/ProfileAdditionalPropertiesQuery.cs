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
    public class ProfileAdditionalPropertiesQuery: Query<ProfileAdditionalPropertiesColumns, ProfileAdditionalProperties>
    { 
		public ProfileAdditionalPropertiesQuery(){}
		public ProfileAdditionalPropertiesQuery(WhereDelegate<ProfileAdditionalPropertiesColumns> where, OrderBy<ProfileAdditionalPropertiesColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public ProfileAdditionalPropertiesQuery(Func<ProfileAdditionalPropertiesColumns, QueryFilter<ProfileAdditionalPropertiesColumns>> where, OrderBy<ProfileAdditionalPropertiesColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public ProfileAdditionalPropertiesQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static ProfileAdditionalPropertiesQuery Where(WhereDelegate<ProfileAdditionalPropertiesColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static ProfileAdditionalPropertiesQuery Where(WhereDelegate<ProfileAdditionalPropertiesColumns> where, OrderBy<ProfileAdditionalPropertiesColumns> orderBy = null!, Database db = null!)
        {
            return new ProfileAdditionalPropertiesQuery(where, orderBy, db);
        }

		public ProfileAdditionalPropertiesCollection Execute()
		{
			return new ProfileAdditionalPropertiesCollection(this, true);
		}
    }
}