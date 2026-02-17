using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class OrganizationMailingAddressCollection: DaoCollection<OrganizationMailingAddressColumns, OrganizationMailingAddress>
    { 
		public OrganizationMailingAddressCollection(){}
		public OrganizationMailingAddressCollection(IDatabase db, DataTable table, IDao dao = null!, string rc = null!) : base(db, table, dao, rc) { }
		public OrganizationMailingAddressCollection(DataTable table, IDao dao = null!, string rc = null!) : base(table, dao, rc) { }
		public OrganizationMailingAddressCollection(IQuery<OrganizationMailingAddressColumns, OrganizationMailingAddress> q, Bam.Data.Dao dao = null!, string rc = null!) : base(q, dao, rc) { }
		public OrganizationMailingAddressCollection(IDatabase db, IQuery<OrganizationMailingAddressColumns, OrganizationMailingAddress> q, bool load) : base(db, q, load) { }
		public OrganizationMailingAddressCollection(IQuery<OrganizationMailingAddressColumns, OrganizationMailingAddress> q, bool load) : base(q, load) { }
    }
}