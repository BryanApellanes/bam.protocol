/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Private.Dao
{
    public class RsaPrivateKeyDataQuery: Query<RsaPrivateKeyDataColumns, RsaPrivateKeyData>
    { 
		public RsaPrivateKeyDataQuery(){}
		public RsaPrivateKeyDataQuery(WhereDelegate<RsaPrivateKeyDataColumns> where, OrderBy<RsaPrivateKeyDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public RsaPrivateKeyDataQuery(Func<RsaPrivateKeyDataColumns, QueryFilter<RsaPrivateKeyDataColumns>> where, OrderBy<RsaPrivateKeyDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public RsaPrivateKeyDataQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static RsaPrivateKeyDataQuery Where(WhereDelegate<RsaPrivateKeyDataColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static RsaPrivateKeyDataQuery Where(WhereDelegate<RsaPrivateKeyDataColumns> where, OrderBy<RsaPrivateKeyDataColumns> orderBy = null!, Database db = null!)
        {
            return new RsaPrivateKeyDataQuery(where, orderBy, db);
        }

		public RsaPrivateKeyDataCollection Execute()
		{
			return new RsaPrivateKeyDataCollection(this, true);
		}
    }
}