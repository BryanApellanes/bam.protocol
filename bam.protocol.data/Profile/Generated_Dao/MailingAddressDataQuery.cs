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
    public class MailingAddressDataQuery: Query<MailingAddressDataColumns, MailingAddressData>
    { 
		public MailingAddressDataQuery(){}
		public MailingAddressDataQuery(WhereDelegate<MailingAddressDataColumns> where, OrderBy<MailingAddressDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public MailingAddressDataQuery(Func<MailingAddressDataColumns, QueryFilter<MailingAddressDataColumns>> where, OrderBy<MailingAddressDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public MailingAddressDataQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static MailingAddressDataQuery Where(WhereDelegate<MailingAddressDataColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static MailingAddressDataQuery Where(WhereDelegate<MailingAddressDataColumns> where, OrderBy<MailingAddressDataColumns> orderBy = null!, Database db = null!)
        {
            return new MailingAddressDataQuery(where, orderBy, db);
        }

		public MailingAddressDataCollection Execute()
		{
			return new MailingAddressDataCollection(this, true);
		}
    }
}