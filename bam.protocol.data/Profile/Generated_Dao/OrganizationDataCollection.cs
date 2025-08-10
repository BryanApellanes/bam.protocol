using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class OrganizationDataCollection: DaoCollection<OrganizationDataColumns, OrganizationData>
    { 
		public OrganizationDataCollection(){}
		public OrganizationDataCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public OrganizationDataCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public OrganizationDataCollection(IQuery<OrganizationDataColumns, OrganizationData> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public OrganizationDataCollection(IDatabase db, IQuery<OrganizationDataColumns, OrganizationData> q, bool load) : base(db, q, load) { }
		public OrganizationDataCollection(IQuery<OrganizationDataColumns, OrganizationData> q, bool load) : base(q, load) { }
    }
}