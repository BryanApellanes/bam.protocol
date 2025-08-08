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
    public class PublicKeySetQuery: Query<PublicKeySetColumns, PublicKeySet>
    { 
		public PublicKeySetQuery(){}
		public PublicKeySetQuery(WhereDelegate<PublicKeySetColumns> where, OrderBy<PublicKeySetColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public PublicKeySetQuery(Func<PublicKeySetColumns, QueryFilter<PublicKeySetColumns>> where, OrderBy<PublicKeySetColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public PublicKeySetQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static PublicKeySetQuery Where(WhereDelegate<PublicKeySetColumns> where)
        {
            return Where(where, null, null);
        }

        public static PublicKeySetQuery Where(WhereDelegate<PublicKeySetColumns> where, OrderBy<PublicKeySetColumns> orderBy = null, Database db = null)
        {
            return new PublicKeySetQuery(where, orderBy, db);
        }

		public PublicKeySetCollection Execute()
		{
			return new PublicKeySetCollection(this, true);
		}
    }
}