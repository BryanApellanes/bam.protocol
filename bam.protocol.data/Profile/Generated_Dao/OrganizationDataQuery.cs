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
    public class OrganizationDataQuery: Query<OrganizationDataColumns, OrganizationData>
    { 
		public OrganizationDataQuery(){}
		public OrganizationDataQuery(WhereDelegate<OrganizationDataColumns> where, OrderBy<OrganizationDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public OrganizationDataQuery(Func<OrganizationDataColumns, QueryFilter<OrganizationDataColumns>> where, OrderBy<OrganizationDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public OrganizationDataQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static OrganizationDataQuery Where(WhereDelegate<OrganizationDataColumns> where)
        {
            return Where(where, null, null);
        }

        public static OrganizationDataQuery Where(WhereDelegate<OrganizationDataColumns> where, OrderBy<OrganizationDataColumns> orderBy = null, Database db = null)
        {
            return new OrganizationDataQuery(where, orderBy, db);
        }

		public OrganizationDataCollection Execute()
		{
			return new OrganizationDataCollection(this, true);
		}
    }
}