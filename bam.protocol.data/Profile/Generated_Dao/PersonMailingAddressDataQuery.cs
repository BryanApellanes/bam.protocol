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
    public class PersonMailingAddressDataQuery: Query<PersonMailingAddressDataColumns, PersonMailingAddressData>
    { 
		public PersonMailingAddressDataQuery(){}
		public PersonMailingAddressDataQuery(WhereDelegate<PersonMailingAddressDataColumns> where, OrderBy<PersonMailingAddressDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public PersonMailingAddressDataQuery(Func<PersonMailingAddressDataColumns, QueryFilter<PersonMailingAddressDataColumns>> where, OrderBy<PersonMailingAddressDataColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public PersonMailingAddressDataQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static PersonMailingAddressDataQuery Where(WhereDelegate<PersonMailingAddressDataColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static PersonMailingAddressDataQuery Where(WhereDelegate<PersonMailingAddressDataColumns> where, OrderBy<PersonMailingAddressDataColumns> orderBy = null!, Database db = null!)
        {
            return new PersonMailingAddressDataQuery(where, orderBy, db);
        }

		public PersonMailingAddressDataCollection Execute()
		{
			return new PersonMailingAddressDataCollection(this, true);
		}
    }
}