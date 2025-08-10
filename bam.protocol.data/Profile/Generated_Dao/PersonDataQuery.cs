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
    public class PersonDataQuery: Query<PersonDataColumns, PersonData>
    { 
		public PersonDataQuery(){}
		public PersonDataQuery(WhereDelegate<PersonDataColumns> where, OrderBy<PersonDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public PersonDataQuery(Func<PersonDataColumns, QueryFilter<PersonDataColumns>> where, OrderBy<PersonDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public PersonDataQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static PersonDataQuery Where(WhereDelegate<PersonDataColumns> where)
        {
            return Where(where, null, null);
        }

        public static PersonDataQuery Where(WhereDelegate<PersonDataColumns> where, OrderBy<PersonDataColumns> orderBy = null, Database db = null)
        {
            return new PersonDataQuery(where, orderBy, db);
        }

		public PersonDataCollection Execute()
		{
			return new PersonDataCollection(this, true);
		}
    }
}