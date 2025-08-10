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
    public class OrganizationCertificateDataQuery: Query<OrganizationCertificateDataColumns, OrganizationCertificateData>
    { 
		public OrganizationCertificateDataQuery(){}
		public OrganizationCertificateDataQuery(WhereDelegate<OrganizationCertificateDataColumns> where, OrderBy<OrganizationCertificateDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }
		public OrganizationCertificateDataQuery(Func<OrganizationCertificateDataColumns, QueryFilter<OrganizationCertificateDataColumns>> where, OrderBy<OrganizationCertificateDataColumns> orderBy = null, Database db = null) : base(where, orderBy, db) { }		
		public OrganizationCertificateDataQuery(Delegate where, Database db = null) : base(where, db) { }
		
        public static OrganizationCertificateDataQuery Where(WhereDelegate<OrganizationCertificateDataColumns> where)
        {
            return Where(where, null, null);
        }

        public static OrganizationCertificateDataQuery Where(WhereDelegate<OrganizationCertificateDataColumns> where, OrderBy<OrganizationCertificateDataColumns> orderBy = null, Database db = null)
        {
            return new OrganizationCertificateDataQuery(where, orderBy, db);
        }

		public OrganizationCertificateDataCollection Execute()
		{
			return new OrganizationCertificateDataCollection(this, true);
		}
    }
}