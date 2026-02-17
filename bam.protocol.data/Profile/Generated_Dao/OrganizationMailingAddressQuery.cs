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
    public class OrganizationMailingAddressQuery: Query<OrganizationMailingAddressColumns, OrganizationMailingAddress>
    { 
		public OrganizationMailingAddressQuery(){}
		public OrganizationMailingAddressQuery(WhereDelegate<OrganizationMailingAddressColumns> where, OrderBy<OrganizationMailingAddressColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }
		public OrganizationMailingAddressQuery(Func<OrganizationMailingAddressColumns, QueryFilter<OrganizationMailingAddressColumns>> where, OrderBy<OrganizationMailingAddressColumns> orderBy = null!, Database db = null!) : base(where, orderBy, db) { }		
		public OrganizationMailingAddressQuery(Delegate where, Database db = null!) : base(where, db) { }
		
        public static OrganizationMailingAddressQuery Where(WhereDelegate<OrganizationMailingAddressColumns> where)
        {
            return Where(where, null!, null!);
        }

        public static OrganizationMailingAddressQuery Where(WhereDelegate<OrganizationMailingAddressColumns> where, OrderBy<OrganizationMailingAddressColumns> orderBy = null!, Database db = null!)
        {
            return new OrganizationMailingAddressQuery(where, orderBy, db);
        }

		public OrganizationMailingAddressCollection Execute()
		{
			return new OrganizationMailingAddressCollection(this, true);
		}
    }
}