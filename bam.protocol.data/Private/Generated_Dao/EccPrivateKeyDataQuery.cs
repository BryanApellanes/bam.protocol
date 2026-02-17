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
    public class EccPrivateKeyDataQuery: Query<EccPrivateKeyDataColumns, EccPrivateKeyData>
    { 
		public EccPrivateKeyDataQuery(){}
		public EccPrivateKeyDataQuery(WhereDelegate<EccPrivateKeyDataColumns> where, OrderBy<EccPrivateKeyDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public EccPrivateKeyDataQuery(Func<EccPrivateKeyDataColumns, QueryFilter<EccPrivateKeyDataColumns>> where, OrderBy<EccPrivateKeyDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public EccPrivateKeyDataQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static EccPrivateKeyDataQuery Where(WhereDelegate<EccPrivateKeyDataColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static EccPrivateKeyDataQuery Where(WhereDelegate<EccPrivateKeyDataColumns> where, OrderBy<EccPrivateKeyDataColumns> orderBy = null!, Database db = null!)
        {
            return new EccPrivateKeyDataQuery(where, orderBy, db);
        }

		public EccPrivateKeyDataCollection Execute()
		{
			return new EccPrivateKeyDataCollection(this, true);
		}
    }
}