using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class OrganizationCollection: DaoCollection<OrganizationColumns, Organization>
    { 
		public OrganizationCollection(){}
		public OrganizationCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public OrganizationCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public OrganizationCollection(IQuery<OrganizationColumns, Organization> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public OrganizationCollection(IDatabase db, IQuery<OrganizationColumns, Organization> q, bool load) : base(db, q, load) { }
		public OrganizationCollection(IQuery<OrganizationColumns, Organization> q, bool load) : base(q, load) { }
    }
}