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
    public class PersonQuery: Query<PersonColumns, Person>
    { 
		public PersonQuery(){}
		public PersonQuery(WhereDelegate<PersonColumns> where, OrderBy<PersonColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public PersonQuery(Func<PersonColumns, QueryFilter<PersonColumns>> where, OrderBy<PersonColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public PersonQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static PersonQuery Where(WhereDelegate<PersonColumns> where)
        {
            return Where(where, null, null);
        }

        public static PersonQuery Where(WhereDelegate<PersonColumns> where, OrderBy<PersonColumns> orderBy = null, Database db = null)
        {
            return new PersonQuery(where, orderBy, db);
        }

		public PersonCollection Execute()
		{
			return new PersonCollection(this, true);
		}
    }
}