using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Bam.Data;

namespace Bam.Protocol.Data.Profile.Dao
{
    public class PersonDataOrganizationDataCollection: DaoCollection<PersonDataOrganizationDataColumns, PersonDataOrganizationData>
    { 
		public PersonDataOrganizationDataCollection(){}
		public PersonDataOrganizationDataCollection(IDatabase db, DataTable table, IDao dao = null, string rc = null) : base(db, table, dao, rc) { }
		public PersonDataOrganizationDataCollection(DataTable table, IDao dao = null, string rc = null) : base(table, dao, rc) { }
		public PersonDataOrganizationDataCollection(IQuery<PersonDataOrganizationDataColumns, PersonDataOrganizationData> q, Bam.Data.Dao dao = null, string rc = null) : base(q, dao, rc) { }
		public PersonDataOrganizationDataCollection(IDatabase db, IQuery<PersonDataOrganizationDataColumns, PersonDataOrganizationData> q, bool load) : base(db, q, load) { }
		public PersonDataOrganizationDataCollection(IQuery<PersonDataOrganizationDataColumns, PersonDataOrganizationData> q, bool load) : base(q, load) { }
    }
}