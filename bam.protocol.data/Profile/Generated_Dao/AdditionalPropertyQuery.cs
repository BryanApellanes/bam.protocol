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
    public class AdditionalPropertyQuery: Query<AdditionalPropertyColumns, AdditionalProperty>
    { 
		public AdditionalPropertyQuery(){}
		public AdditionalPropertyQuery(WhereDelegate<AdditionalPropertyColumns> where, OrderBy<AdditionalPropertyColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public AdditionalPropertyQuery(Func<AdditionalPropertyColumns, QueryFilter<AdditionalPropertyColumns>> where, OrderBy<AdditionalPropertyColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public AdditionalPropertyQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static AdditionalPropertyQuery Where(WhereDelegate<AdditionalPropertyColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static AdditionalPropertyQuery Where(WhereDelegate<AdditionalPropertyColumns> where, OrderBy<AdditionalPropertyColumns> orderBy = null!, Database db = null!)
        {
            return new AdditionalPropertyQuery(where, orderBy, db);
        }

		public AdditionalPropertyCollection Execute()
		{
			return new AdditionalPropertyCollection(this, true);
		}
    }
}