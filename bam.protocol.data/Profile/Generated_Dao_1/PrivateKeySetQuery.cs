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
    public class PrivateKeySetQuery: Query<PrivateKeySetColumns, PrivateKeySet>
    { 
		public PrivateKeySetQuery(){}
		public PrivateKeySetQuery(WhereDelegate<PrivateKeySetColumns> where, OrderBy<PrivateKeySetColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public PrivateKeySetQuery(Func<PrivateKeySetColumns, QueryFilter<PrivateKeySetColumns>> where, OrderBy<PrivateKeySetColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public PrivateKeySetQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static PrivateKeySetQuery Where(WhereDelegate<PrivateKeySetColumns> where)
        {
            return Where(where, null, null);
        }

        public static PrivateKeySetQuery Where(WhereDelegate<PrivateKeySetColumns> where, OrderBy<PrivateKeySetColumns> orderBy = null, Database db = null)
        {
            return new PrivateKeySetQuery(where, orderBy, db);
        }

		public PrivateKeySetCollection Execute()
		{
			return new PrivateKeySetCollection(this, true);
		}
    }
}