/*
	This file was generated and should not be modified directly
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Server.Dao
{
    public class AccountDataQuery: Query<AccountDataColumns, AccountData>
    { 
		public AccountDataQuery(){}
		public AccountDataQuery(WhereDelegate<AccountDataColumns> where, OrderBy<AccountDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public AccountDataQuery(Func<AccountDataColumns, QueryFilter<AccountDataColumns>> where, OrderBy<AccountDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public AccountDataQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static AccountDataQuery Where(WhereDelegate<AccountDataColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static AccountDataQuery Where(WhereDelegate<AccountDataColumns> where, OrderBy<AccountDataColumns> orderBy = null!, Database db = null!)
        {
            return new AccountDataQuery(where, orderBy, db);
        }

		public AccountDataCollection Execute()
		{
			return new AccountDataCollection(this, true);
		}
    }
}