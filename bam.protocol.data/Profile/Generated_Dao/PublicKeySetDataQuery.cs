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
    public class PublicKeySetDataQuery: Query<PublicKeySetDataColumns, PublicKeySetData>
    { 
		public PublicKeySetDataQuery(){}
		public PublicKeySetDataQuery(WhereDelegate<PublicKeySetDataColumns> where, OrderBy<PublicKeySetDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public PublicKeySetDataQuery(Func<PublicKeySetDataColumns, QueryFilter<PublicKeySetDataColumns>> where, OrderBy<PublicKeySetDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public PublicKeySetDataQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static PublicKeySetDataQuery Where(WhereDelegate<PublicKeySetDataColumns> where)
        {
            return Where(where, null, null);
        }

        public static PublicKeySetDataQuery Where(WhereDelegate<PublicKeySetDataColumns> where, OrderBy<PublicKeySetDataColumns> orderBy = null, Database db = null)
        {
            return new PublicKeySetDataQuery(where, orderBy, db);
        }

		public PublicKeySetDataCollection Execute()
		{
			return new PublicKeySetDataCollection(this, true);
		}
    }
}