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
    public class PersonDataOrganizationDataQuery: Query<PersonDataOrganizationDataColumns, PersonDataOrganizationData>
    { 
		public PersonDataOrganizationDataQuery(){}
		public PersonDataOrganizationDataQuery(WhereDelegate<PersonDataOrganizationDataColumns> where, OrderBy<PersonDataOrganizationDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public PersonDataOrganizationDataQuery(Func<PersonDataOrganizationDataColumns, QueryFilter<PersonDataOrganizationDataColumns>> where, OrderBy<PersonDataOrganizationDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public PersonDataOrganizationDataQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static PersonDataOrganizationDataQuery Where(WhereDelegate<PersonDataOrganizationDataColumns> where)
        {
            return Where(where, null, null);
        }

        public static PersonDataOrganizationDataQuery Where(WhereDelegate<PersonDataOrganizationDataColumns> where, OrderBy<PersonDataOrganizationDataColumns> orderBy = null, Database db = null)
        {
            return new PersonDataOrganizationDataQuery(where, orderBy, db);
        }

		public PersonDataOrganizationDataCollection Execute()
		{
			return new PersonDataOrganizationDataCollection(this, true);
		}
    }
}