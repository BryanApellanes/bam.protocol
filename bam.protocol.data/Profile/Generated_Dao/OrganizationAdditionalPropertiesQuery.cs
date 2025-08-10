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
    public class OrganizationAdditionalPropertiesQuery: Query<OrganizationAdditionalPropertiesColumns, OrganizationAdditionalProperties>
    { 
		public OrganizationAdditionalPropertiesQuery(){}
		public OrganizationAdditionalPropertiesQuery(WhereDelegate<OrganizationAdditionalPropertiesColumns> where, OrderBy<OrganizationAdditionalPropertiesColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public OrganizationAdditionalPropertiesQuery(Func<OrganizationAdditionalPropertiesColumns, QueryFilter<OrganizationAdditionalPropertiesColumns>> where, OrderBy<OrganizationAdditionalPropertiesColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public OrganizationAdditionalPropertiesQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static OrganizationAdditionalPropertiesQuery Where(WhereDelegate<OrganizationAdditionalPropertiesColumns> where)
        {
            return Where(where, null, null);
        }

        public static OrganizationAdditionalPropertiesQuery Where(WhereDelegate<OrganizationAdditionalPropertiesColumns> where, OrderBy<OrganizationAdditionalPropertiesColumns> orderBy = null, Database db = null)
        {
            return new OrganizationAdditionalPropertiesQuery(where, orderBy, db);
        }

		public OrganizationAdditionalPropertiesCollection Execute()
		{
			return new OrganizationAdditionalPropertiesCollection(this, true);
		}
    }
}