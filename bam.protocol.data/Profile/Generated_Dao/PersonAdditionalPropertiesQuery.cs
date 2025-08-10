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
    public class PersonAdditionalPropertiesQuery: Query<PersonAdditionalPropertiesColumns, PersonAdditionalProperties>
    { 
		public PersonAdditionalPropertiesQuery(){}
		public PersonAdditionalPropertiesQuery(WhereDelegate<PersonAdditionalPropertiesColumns> where, OrderBy<PersonAdditionalPropertiesColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public PersonAdditionalPropertiesQuery(Func<PersonAdditionalPropertiesColumns, QueryFilter<PersonAdditionalPropertiesColumns>> where, OrderBy<PersonAdditionalPropertiesColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public PersonAdditionalPropertiesQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static PersonAdditionalPropertiesQuery Where(WhereDelegate<PersonAdditionalPropertiesColumns> where)
        {
            return Where(where, null, null);
        }

        public static PersonAdditionalPropertiesQuery Where(WhereDelegate<PersonAdditionalPropertiesColumns> where, OrderBy<PersonAdditionalPropertiesColumns> orderBy = null, Database db = null)
        {
            return new PersonAdditionalPropertiesQuery(where, orderBy, db);
        }

		public PersonAdditionalPropertiesCollection Execute()
		{
			return new PersonAdditionalPropertiesCollection(this, true);
		}
    }
}