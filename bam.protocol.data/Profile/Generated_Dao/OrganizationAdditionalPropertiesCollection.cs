using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class OrganizationAdditionalPropertiesCollection: DaoCollection<OrganizationAdditionalPropertiesColumns, OrganizationAdditionalProperties>
    { 
		public OrganizationAdditionalPropertiesCollection(){}
		public OrganizationAdditionalPropertiesCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public OrganizationAdditionalPropertiesCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public OrganizationAdditionalPropertiesCollection(IQuery<OrganizationAdditionalPropertiesColumns, OrganizationAdditionalProperties> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public OrganizationAdditionalPropertiesCollection(IDatabase db, IQuery<OrganizationAdditionalPropertiesColumns, OrganizationAdditionalProperties> q, bool load) : base(db, q, load) { }
		public OrganizationAdditionalPropertiesCollection(IQuery<OrganizationAdditionalPropertiesColumns, OrganizationAdditionalProperties> q, bool load) : base(q, load) { }
    }
}