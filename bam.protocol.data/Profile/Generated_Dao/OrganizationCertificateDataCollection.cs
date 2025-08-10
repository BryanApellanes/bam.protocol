using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class OrganizationCertificateDataCollection: DaoCollection<OrganizationCertificateDataColumns, OrganizationCertificateData>
    { 
		public OrganizationCertificateDataCollection(){}
		public OrganizationCertificateDataCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public OrganizationCertificateDataCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public OrganizationCertificateDataCollection(IQuery<OrganizationCertificateDataColumns, OrganizationCertificateData> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public OrganizationCertificateDataCollection(IDatabase db, IQuery<OrganizationCertificateDataColumns, OrganizationCertificateData> q, bool load) : base(db, q, load) { }
		public OrganizationCertificateDataCollection(IQuery<OrganizationCertificateDataColumns, OrganizationCertificateData> q, bool load) : base(q, load) { }
    }
}